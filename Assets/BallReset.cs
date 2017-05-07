using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallReset : MonoBehaviour {
	Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		originalPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "Ground")
		{
			gameObject.transform.position = originalPosition;
		}
	}
}
