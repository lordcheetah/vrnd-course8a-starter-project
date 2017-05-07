using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusHandInteraction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider target)
	{
		if (target.tag == "Throwable")
		{
			// throw ball
		}
	}
}
