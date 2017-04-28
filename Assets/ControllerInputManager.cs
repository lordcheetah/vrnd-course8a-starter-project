using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device;
	//public float throwForce = 1.5f;

	private LineRenderer laser;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;

	public float yNudgeAmount = 1f; //specific to teleportAimerObject height

	//dash
	public float dashSpeed = 0.2f;
	private bool isDashing = false;
	private float lerpTime;
	private Vector3 dashStartPosition;

	//Walking
	public Transform playerCam;
	public float moveSpeed = 4f;
	private Vector3 movementDirection;

	/*//swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	public ObjectMenuManager objectMenuManager;*/

	// Use this for initialization
	void Start ()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
		laser = GetComponentInChildren<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		device = SteamVR_Controller.Input ((int)trackedObj.index);

		if (device.GetPress (SteamVR_Controller.ButtonMask.Grip))
		{
			movementDirection = playerCam.transform.forward;
			movementDirection = new Vector3 (movementDirection.x, 0, movementDirection.z);
			movementDirection *= moveSpeed * Time.deltaTime;
			player.transform.position += movementDirection;
		}

		if (isDashing)
		{
			lerpTime = 1 * dashSpeed;
			player.transform.position = Vector3.Lerp (dashStartPosition, teleportLocation, lerpTime);
			if (lerpTime >= 1)
			{
				isDashing = false;
				lerpTime = 0;
			}
		}

		if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger))
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
				teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
			}
			else
			{
				teleportLocation = new Vector3 (transform.forward.x * 15 + transform.forward.x, transform.forward.y * 15 + transform.position.y, transform.forward.z * 15 + transform.position.z);
				RaycastHit groundRay;
				if (Physics.Raycast (teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
				{
					teleportLocation = new Vector3 (transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);

				}
				laser.SetPosition (1, transform.forward * 15 + transform.position);
				//aimer position
				teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
			}
		}

		if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger))
		{
			laser.gameObject.SetActive (false);
			teleportAimerObject.SetActive (false);
			//player.transform.position = teleportLocation;
			dashStartPosition = player.transform.position;
			isDashing = true;
		}

		/*if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Touchpad))
		{
			//SteamVR_LoadLevel.Begin (Scene1);
			touchLast = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
		}
		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad))
		{
			touchCurrent = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			swipeSum += distance;

			if (!hasSwipedRight)
			{
				if (swipeSum > 0.5f)
				{
					swipeSum = 0;
					SwipedRight ();
					hasSwipedRight = true;
					hasSwipedLeft = false;
				}
			}
			if (!hasSwipedLeft)
			{
				if(swipeSum < -0.5f)
				{
					swipeSum = 0;
					SwipedLeft ();
					hasSwipedLeft = true;
					hasSwipedRight = false;
				}
			}

		}
		if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Touchpad))
		{
			swipeSum = 0;
			touchCurrent = 0;
			touchLast = 0;
			hasSwipedLeft = false;
			hasSwipedRight = false;
		}
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Touchpad))
		{
			//spawn object currently selected by menu
			SpawnObject();
		}*/
	}

	/*void SpawnObject()
	{
		objectMenuManager.SpawnCurrentObject ();
	}

	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.CompareTag ("Throwable"))
		{
			if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger))
			{
				ThrowObject (col);
			} else if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger))
			{
				GrabObject (col);
			}
		}
	}

	void GrabObject(Collider coli)
	{
		coli.transform.SetParent (gameObject.transform);
		coli.GetComponent<Rigidbody> ().isKinematic = true;
		device.TriggerHapticPulse (2000);
		Debug.Log ("You are touching down the trigger on an object");
	}

	void ThrowObject(Collider coli)
	{
		coli.transform.SetParent (null);
		Rigidbody rigidBody = coli.GetComponent<Rigidbody> ();
		rigidBody.isKinematic = false;
		rigidBody.velocity = device.velocity * throwForce;
		rigidBody.angularVelocity = device.angularVelocity;
		Debug.Log ("You have released the trigger");
	}

	void SwipedLeft()
	{
		objectMenuManager.MenuLeft ();
		Debug.Log ("SwipeLeft");
	}

	void SwipedRight()
	{
		objectMenuManager.MenuRight ();
		Debug.Log ("SwipeRight");
	}*/
}
