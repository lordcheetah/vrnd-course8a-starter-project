using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandInteraction : MonoBehaviour {
	private OVRInput.Controller thisController;
	public bool leftHand; // true if left hand controller

	public float throwForce = 1.5f;

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

	// Use this for initialization
	void Start () {
		if (leftHand)
		{
			thisController = OVRInput.Controller.LTouch;
		} else
		{
			thisController = OVRInput.Controller.RTouch;
		}
		InitializeOVRHaptics();
	}
	
	// Update is called once per frame
	void Update () {
		
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
		Rigidbody rigidBody = coli.GetComponent<Rigidbody> ();
		rigidBody.isKinematic = false;
		rigidBody.velocity = OVRInput.GetLocalControllerVelocity (thisController) * throwForce;
		rigidBody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity (thisController);
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
