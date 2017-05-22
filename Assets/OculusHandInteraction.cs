using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandInteraction : MonoBehaviour {
	private OVRInput.Controller thisController;
	public bool leftHand; // true if left hand controller

	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;

	public float throwForce = 1.5f;

	private LineRenderer laser;



	public float yNudgeAmount = 1f; //specific to teleportAimerObject height

	public enum VibrationForce
	{
		Light,
		Medium,
		Hard,
	}

	[SerializeField]
	OVRInput.Controller controllerMask;

	private OVRHapticsClip clipLight;
	private OVRHapticsClip clipMedium;
	private OVRHapticsClip clipHard;

	private float menuStickX;

	// Use this for initialization
	void Start () {
		if (leftHand)
		{
			thisController = OVRInput.Controller.LTouch;
		} else
		{
			thisController = OVRInput.Controller.RTouch;
		}
		laser = GetComponentInChildren<LineRenderer> ();
		InitializeOVRHaptics();
	}
	
	// Update is called once per frame
	void Update () {
		menuStickX = OVRInput.Get (OVRInput.Axis2D.PrimaryThumbstick, thisController).x;
		if (leftHand && menuStickX < 0.45f && menuStickX > -0.45f)
		{
			laser.gameObject.SetActive (true);
			teleportAimerObject.SetActive (true);

			laser.SetPosition (0, gameObject.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit, 15, laserMask))
			{
				teleportLocation = hit.point;
				laser.SetPosition (1, teleportLocation);
				//aimer position
				teleportAimerObject.transform.position = new Vector3 (teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
			} else
			{
				teleportLocation = new Vector3 (transform.forward.x * 15 + transform.forward.x, transform.forward.y * 15 + transform.position.y, transform.forward.z * 15 + transform.position.z);
				RaycastHit groundRay;
				if (Physics.Raycast (teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
				{
					teleportLocation = new Vector3 (transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);

				}
				laser.SetPosition (1, transform.forward * 15 + transform.position);
				//aimer position
				teleportAimerObject.transform.position = teleportLocation + new Vector3 (0, yNudgeAmount, 0);
			}
		}
			
		if (leftHand && menuStickX >= 0.45f && menuStickX <= -0.45f)
		{
			laser.gameObject.SetActive (false);
			teleportAimerObject.SetActive (false);
			player.transform.position = teleportLocation;
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.CompareTag ("Throwable"))
		{
			if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) < 0.1f)
			{
				ThrowObject (col);
			} else if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, thisController) > 0.1f)
			{
				GrabObject (col);
			}
		}
	}

	void GrabObject(Collider coli)
	{
		coli.transform.SetParent (gameObject.transform);
		coli.GetComponent<Rigidbody> ().isKinematic = true;
		Vibrate (VibrationForce.Light);
		Debug.Log ("You are touching down the trigger on an object");
	}

	void ThrowObject(Collider coli)
	{
		coli.transform.SetParent (null);
		if (coli.tag == "Structure")
		{
			coli.enabled = false;
		} else
		{
			Rigidbody rigidBody = coli.GetComponent<Rigidbody> ();
			rigidBody.isKinematic = false;
			rigidBody.velocity = OVRInput.GetLocalControllerVelocity (thisController) * throwForce;
			rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController);
		}
		Debug.Log ("You have released the trigger");
	}

	private void InitializeOVRHaptics()
	{
		int cnt = 10;
		clipLight = new OVRHapticsClip(cnt);
		clipMedium = new OVRHapticsClip(cnt);
		clipHard = new OVRHapticsClip(cnt);
		for (int i = 0; i < cnt; i++)
		{
			clipLight.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)75;
			clipMedium.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)150;
			clipHard.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)255;
		}

		clipLight = new OVRHapticsClip(clipLight.Samples, clipLight.Samples.Length);
		clipMedium = new OVRHapticsClip(clipMedium.Samples, clipMedium.Samples.Length);
		clipHard = new OVRHapticsClip(clipHard.Samples, clipHard.Samples.Length);
	}

	public void Vibrate(VibrationForce vibrationForce)
	{
		var channel = OVRHaptics.RightChannel;
		if (leftHand)
			channel = OVRHaptics.LeftChannel;

		switch (vibrationForce)
		{
		case VibrationForce.Light:
			channel.Preempt(clipLight);
			break;
		case VibrationForce.Medium:
			channel.Preempt(clipMedium);
			break;
		case VibrationForce.Hard:
			channel.Preempt(clipHard);
			break;
		}
	}
}
