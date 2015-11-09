using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class AirCraft : MonoBehaviour {
	
	private Rigidbody cRigidbody;
	private Rigidbody CRigidbody {
		get {
			if (cRigidbody == null) {
				this.cRigidbody = this.GetComponent<Rigidbody> ();
			}
			
			return cRigidbody;
		}
	}
	public void SetKinematic (bool k) {
		this.CRigidbody.isKinematic = k;
	}
	
	public bool engineBoost = false;
	public float engineBoostPow = 10f;
	public float engineInc = 5f;
	public float enginePow = 0f;
	public float enginePowMax = 20f;
	public float enginePowMin = -5f;
	
	public float lift;
	
	public float cForward;
	public float cRight;
	public float cUp;
	
	public float yawSpeed;
	public float pitchSpeed;
	public float rollSpeed;
		
	private float localAtm = 1f;
	
	public Planet planet = null;

	public PilotAirCraftState pilotMode;

	public enum PilotAirCraftState
	{
		NoPilot,
		Pilot
	};

	void Start () {
		this.enginePow = 0f;
		this.SwitchModeTo (this.pilotMode);
	}
	
	void Update () {
		if (this.pilotMode == PilotAirCraftState.Pilot) {
			if (Input.GetKeyDown (KeyCode.W)) {
				this.enginePow += this.engineInc;
				if (this.enginePow > this.enginePowMax) {
					this.enginePow = this.enginePowMax;
				}
			}
			
			if (Input.GetKeyDown (KeyCode.S)) {
				this.enginePow -= this.engineInc;
				if (this.enginePow < this.enginePowMin) {
					this.enginePow = this.enginePowMin;
				}
			}
		}
	}
	
	private float forwardVelocity;
	private float rightVelocity;
	private float upVelocity;
	
	private float pitchInput;
	private float yawInput;
	private float rollInput;
	
	private bool orbitalMode = false;
	
	public Action YawAndPitchInput;

	public void YawAndPitchPlayerInput () {
		yawInput = Input.GetAxis ("Mouse X");
		pitchInput = Input.GetAxis ("Mouse Y");;
		
		if (Input.GetKey (KeyCode.A)) {
			rollInput ++;
		}
		if (Input.GetKey (KeyCode.D)) {
			rollInput --;
		}
	}

	void FixedUpdate () {
		pitchInput = 0f;
		yawInput = 0f;
		rollInput = 0f;

		if (this.YawAndPitchInput != null) {
			this.YawAndPitchInput ();
		}
		
		forwardVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.forward);
		rightVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.right);
		upVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.up);
		
		float sqrForwardVelocity = forwardVelocity * Mathf.Abs (forwardVelocity);
		float sqrRightVelocity = rightVelocity * Mathf.Abs (rightVelocity);
		float sqrUpVelocity = upVelocity * Mathf.Abs (upVelocity);
		
		if (Input.GetKey (KeyCode.Space)) {
			this.engineBoost = true;
			this.CRigidbody.AddForce ((enginePow + engineBoostPow) * this.transform.forward);
		} 
		else {
			this.engineBoost = false;
			this.CRigidbody.AddForce (enginePow * this.transform.forward);
		}
		
		this.cRigidbody.AddForce (- sqrForwardVelocity * this.cForward * this.transform.forward * this.localAtm);
		this.cRigidbody.AddForce (- sqrRightVelocity * this.cRight * this.transform.right);
		this.cRigidbody.AddForce (- sqrUpVelocity * this.cUp * this.transform.up);
		
		this.CRigidbody.AddTorque (yawSpeed * yawInput * this.transform.up);
		this.CRigidbody.AddTorque (- pitchSpeed * pitchInput * this.transform.right);
		this.CRigidbody.AddTorque (rollSpeed * rollInput * this.transform.forward);
		
		this.CRigidbody.AddForce (this.CRigidbody.mass * this.ComputePlanetGravity ());
	}
	
	void OnGUI () {
		if (this.engineBoost) {
			GUILayout.TextArea ("EnginePow = " + (this.enginePow + this.engineBoostPow));
		} 
		else {
			GUILayout.TextArea ("EnginePow = " + this.enginePow);
		}
		GUILayout.TextArea ("ForwardVelocity = " + this.forwardVelocity);
		GUILayout.TextArea ("RightdVelocity = " + this.rightVelocity);
		GUILayout.TextArea ("UpVelocity = " + this.upVelocity);
		GUILayout.TextArea ("PitchInput = " + this.pitchInput);
		GUILayout.TextArea ("YawInput = " + this.yawInput);
		GUILayout.TextArea ("RollInput = " + this.rollInput);
		GUILayout.TextArea ("Local Atm = " + this.localAtm);
	}
	
	public Vector3 ComputePlanetGravity () {
		this.localAtm = 0f;
		Vector3 gravity = Vector3.zero;

		if (this.planet != null) {
			float dist = (this.planet.transform.position - this.transform.position).magnitude;
			dist = Mathf.Max (dist, 0f);
			
			gravity = this.planet.mass / (dist * dist) * (this.planet.transform.position - this.transform.position).normalized;
			
			float a = (this.planet.atmRange - Mathf.Max (dist - this.planet.radius, 0f)) / this.planet.atmRange * this.planet.atmDensity;;
			if (a > 0) {
				this.localAtm = a;
			}
		}
		
		return gravity;
	}

	public void SwitchModeTo (PilotAirCraftState newPilotMode) {
		if (newPilotMode == PilotAirCraftState.NoPilot) {
			this.YawAndPitchInput = null;
			this.pilotMode = newPilotMode;
		} 
		else if (newPilotMode == PilotAirCraftState.Pilot) {
			this.YawAndPitchInput = this.YawAndPitchPlayerInput;
			this.pilotMode = newPilotMode;
		}
	}

	public void TakeControl (Player p) {
		p.SetKinematic (true);
		p.transform.parent = this.transform;
		p.transform.localPosition = Vector3.zero;
		p.transform.localRotation = Quaternion.identity;
		p.playerMode = Player.PlayerState.PilotAirCraft;
		this.SwitchModeTo (PilotAirCraftState.Pilot);
		this.SetKinematic (false);
	}
}