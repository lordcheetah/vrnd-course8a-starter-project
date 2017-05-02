using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device;
	public float throwForce = 1.5f;

	private LineRenderer laser;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;

	public float yNudgeAmount = 1f; //specific to teleportAimerObject height

	//swipe
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;
	//public ObjectMenuManager objectMenuManager;

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

		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Touchpad))
		{
			touchLast = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
		}

		if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad))
		{
			touchCurrent = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			swipeSum += distance;


			if (!hasSwipedLeft)
			{
				if (swipeSum < -0.5f)
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

			laser.gameObject.SetActive (false);
			teleportAimerObject.SetActive (false);
			player.transform.position = teleportLocation;
		}


	}

	void SwipedLeft()
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
		Debug.Log ("SwipeLeft");
	}

}
