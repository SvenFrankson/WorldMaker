using UnityEngine;
using System.Collections;

public class AirCraftCam : MonoBehaviour {

	public Transform aircraft;
	public Transform target;
	
	void FixedUpdate () {
		this.transform.position = (19f * this.transform.position + target.position) / 20f;
		this.transform.LookAt (aircraft.position, target.up);
	}
}
