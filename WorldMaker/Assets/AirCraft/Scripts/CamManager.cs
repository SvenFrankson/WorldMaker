using UnityEngine;
using System.Collections;

public class CamManager : MonoBehaviour {

	public Transform aircraft;
	public Transform target;
	public int smoothness = 10;

	public CamMode camMode;
	public enum CamMode {
		Player,
		AirCraft
	};

	void FixedUpdate () {
		if (this.camMode == CamMode.AirCraft) {
			this.transform.position = (smoothness * this.transform.position + target.position) / (smoothness + 1);
			this.transform.LookAt (aircraft.position, target.up);
		}
	}

	public void GoPlayerMode (Player p) {
		this.camMode = CamMode.Player;
		this.transform.parent = p.transform;
		this.transform.localPosition = Vector3.zero + Vector3.up * 0.8f;
		this.transform.localRotation = Quaternion.identity;
	}

	public void GoAirCraftMode (AirCraft a) {
		this.camMode = CamMode.AirCraft;
		this.transform.parent = null;
		this.aircraft = a.transform;
		this.target = a.CamTarget;
	}
}
