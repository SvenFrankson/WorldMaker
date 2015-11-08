using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(StellarObjectCenter))]
[RequireComponent(typeof(Rigidbody))]
public class MotherShip : MonoBehaviour {
	
	private StellarObjectCenter truePos;
	public StellarObjectCenter TruePos {
		get {
			if (truePos == null) {
				this.truePos = this.GetComponent<StellarObjectCenter> ();
			}
			
			return truePos;
		}
	}

	private Rigidbody cRigidbody;
	private Rigidbody CRigidbody {
		get {
			if (cRigidbody == null) {
				this.cRigidbody = this.GetComponent<Rigidbody> ();
			}

			return cRigidbody;
		}
	}

	public float engineInc = 50f;
	public float enginePow = 0f;
	public float enginePowMax = 50f;
	public float enginePowMin = -50f;

	public float cForward;
	public float cRight;
	public float cUp;

	public float yawSpeed;
	public float pitchSpeed;
	public float rollSpeed;
	
	public float forwardVelocity;
	private float rightVelocity;
	private float upVelocity;
	
	private float yawInput;
	private float pitchInput;
	
	public PilotState pilotMode = PilotState.Pilot;
	public Planet orbitPlanet = null;
	public float orbitalPlanetDist = 0f;
	public float orbitVelocity = 0f;

	public enum PilotState
	{
		NoPilot,
		Pilot,
		OrbitAutoPilot
	};

	private List<KeyValuePair<Planet, float>> planets = null;
	public List<KeyValuePair<Planet, float>> Planets {
		get {
			if (this.planets == null) {
				this.planets = new List<KeyValuePair<Planet, float>> ();
				foreach (Planet p in FindObjectsOfType <Planet> ()) {
					this.planets.Add (new KeyValuePair<Planet, float> (p, float.MaxValue));
				}
			}

			return this.planets;
		}
	}

	public Planet closestPlanet {
		get {
			if (this.Planets.Count > 0) {
				return this.Planets [0].Key;
			}
			return null;
		}
	}
	public float closestPlanetDist {
		get {
			if (this.Planets.Count > 0) {
				return this.Planets [0].Value;
			}
			return float.MaxValue;
		}
	}

	private float localAtm = 0f;

	void Start () {
		this.enginePow = 0f;
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Keypad8)) {
			Time.timeScale += 10f;
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			Time.timeScale -= 10f;
			Time.timeScale = Mathf.Max (Time.timeScale, 1f);
		}

		if (this.pilotMode == PilotState.Pilot) {
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
		else if (this.pilotMode == PilotState.OrbitAutoPilot) {
			this.enginePow = 0f;
			if (this.forwardVelocity < this.orbitVelocity * 0.99f) {
				this.enginePow += this.engineInc;
				if (this.enginePow > this.enginePowMax) {
					this.enginePow = this.enginePowMax;
				}
			}
			if (this.forwardVelocity > this.orbitVelocity * 1.1f) {
				this.enginePow -= this.engineInc;
				if (this.enginePow < this.enginePowMin) {
					this.enginePow = this.enginePowMin;
				}
			}
		}
	}

	void FixedUpdate () {
		yawInput = 0f;
		pitchInput = 0f;

		if (this.pilotMode == PilotState.Pilot) {
			yawInput = (Input.mousePosition.x - (Screen.width / 2f)) / (Screen.width / 2f);
			pitchInput = (Input.mousePosition.y - (Screen.height / 2f)) / (Screen.height / 2f);
			
			yawInput = yawInput * Mathf.Abs (yawInput);
			pitchInput = pitchInput * Mathf.Abs (pitchInput);
		} 
		else if (this.pilotMode == PilotState.OrbitAutoPilot) {
			pitchInput = PitchFor (orbitPlanet) / 18f;
		}

		forwardVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.forward);
		rightVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.right);
		upVelocity = Vector3.Dot (this.CRigidbody.velocity, this.transform.up);

		float sqrForwardVelocity = forwardVelocity * Mathf.Abs (forwardVelocity);
		float sqrRightVelocity = rightVelocity * Mathf.Abs (rightVelocity);
		float sqrUpVelocity = upVelocity * Mathf.Abs (upVelocity);

		this.CRigidbody.AddForce ((enginePow - sqrForwardVelocity * this.cForward * this.localAtm) * this.transform.forward);

		this.cRigidbody.AddForce (- sqrForwardVelocity * this.cForward * this.transform.forward * this.localAtm);
		this.cRigidbody.AddForce (- sqrRightVelocity * this.cRight * this.transform.right);
		this.cRigidbody.AddForce (- sqrUpVelocity * this.cUp * this.transform.up);

		this.CRigidbody.AddTorque (this.yawSpeed * yawInput * this.transform.up);
		this.CRigidbody.AddTorque (- this.pitchSpeed * pitchInput * this.transform.right);

		this.CRigidbody.AddForce (this.CRigidbody.mass * this.UpdatePlanets ());

		float roll = 0;
		if (this.pilotMode == PilotState.Pilot) {
			if (Input.GetKey (KeyCode.A)) {
				roll ++;
			}
			if (Input.GetKey (KeyCode.D)) {
				roll --;
			}
		}
		else if (this.pilotMode == PilotState.OrbitAutoPilot) {
			roll = RollFor (orbitPlanet) / 18f;
		}

		if (this.pilotMode == PilotState.OrbitAutoPilot) {
			this.CRigidbody.MovePosition (this.transform.position + this.transform.up * (this.orbitalPlanetDist - this.DistFor (this.orbitPlanet)));
		}

		this.CRigidbody.AddTorque (this.rollSpeed * roll * this.transform.forward);

		if (Input.GetKeyDown (KeyCode.O)) {
			if (this.pilotMode == PilotState.Pilot) {
				this.EnterOrbitalMode ();
			}
			else if (this.pilotMode == PilotState.OrbitAutoPilot) {
				this.ExitOrbitalMode ();
			}
		}
	}

	void OnGUI () {
		GUILayout.TextArea ("EnginePow = " + this.enginePow);
		GUILayout.TextArea ("ForwardVelocity = " + this.forwardVelocity);
		GUILayout.TextArea ("RightdVelocity = " + this.rightVelocity);
		GUILayout.TextArea ("UpVelocity = " + this.upVelocity);
		GUILayout.TextArea ("MouseX = " + this.yawInput);
		GUILayout.TextArea ("MouseY = " + this.pitchInput);
		GUILayout.TextArea ("Local Atm = " + this.localAtm);
		foreach (KeyValuePair<Planet, float> p in this.Planets) {
			GUILayout.TextArea (p.Key.planetName + " : " + p.Value + " m");
		}
		GUILayout.TextArea ("Closest = " + this.closestPlanet);
		GUILayout.TextArea ("Dist = " + this.closestPlanetDist);
		GUILayout.TextArea ("TimeScale = " + Time.timeScale);
	}

	public bool CanEnterOrbitalMode () {
		if (Mathf.Abs(this.RollFor (this.closestPlanet)) > 10f) {
			return false;
		}
		if (Mathf.Abs(this.PitchFor (this.closestPlanet)) > 10f) {
			return false;
		}

		float orbitalSpeedClosest = Mathf.Sqrt (this.closestPlanet.mass / this.closestPlanetDist);
		if (Mathf.Abs ((this.forwardVelocity - orbitalSpeedClosest) / orbitalSpeedClosest) > 0.1f) {
			return false;
		}
		return true;
	}

	private void EnterOrbitalMode () {
		if (this.CanEnterOrbitalMode ()) {
			this.pilotMode = PilotState.OrbitAutoPilot;

			this.orbitPlanet = this.closestPlanet;
			this.orbitalPlanetDist = this.closestPlanetDist;
			this.orbitVelocity = Mathf.Sqrt (this.closestPlanet.mass / this.closestPlanetDist);
		}
	}

	private void ExitOrbitalMode () {
		this.pilotMode = PilotState.Pilot;

		this.orbitPlanet = null;
		this.orbitalPlanetDist = 0f;
		this.orbitVelocity = 0f;
	}

	public Vector3 UpdatePlanets () {
		for (int i = 0; i < this.Planets.Count; i++) {
			KeyValuePair<Planet, float> p = this.Planets [i];
			float dist = (p.Key.TruePos.TruePos - this.TruePos.TruePos).magnitude;
			this.Planets [i] = new KeyValuePair<Planet, float> (p.Key, dist);

			if (i - 1 >= 0) {
				KeyValuePair<Planet, float> pPrev = this.Planets [i - 1];
				if (p.Value < pPrev.Value) {
					this.Planets [i] = pPrev;
					this.Planets [i - 1] = p;
				}
			}
		}
		
		Vector3 gravity = this.closestPlanet.mass / (this.closestPlanetDist * this.closestPlanetDist) * (this.closestPlanet.TruePos.TruePos - this.TruePos.TruePos).normalized;

		float altitude = this.closestPlanetDist - this.closestPlanet.radius;
		float a = (this.closestPlanet.atmRange - altitude) / this.closestPlanet.atmRange * this.closestPlanet.atmDensity;;
		if (a > 0) {
			this.localAtm += a;
		}

		return gravity;
	}

	public float DistFor (Planet p) {
		foreach (KeyValuePair<Planet, float> pd in this.Planets) {
			if (p == pd.Key) {
				return pd.Value;
			}
		}
		
		return 0f;
	}

	public float RollFor (Planet p) {
		Vector3 zero = Vector3.Cross (this.transform.forward, (p.transform.position - this.transform.position));
		float rollAngle = Vector3.Angle (this.transform.right, zero);
		if (Vector3.Dot (this.transform.up, zero) < 0) {
			rollAngle = - rollAngle;
		}
		return rollAngle;
	}
	
	public float PitchFor (Planet p) {
		Vector3 zero = Vector3.Cross (this.transform.right, (this.transform.position - p.transform.position));
		float pitchAngle = Vector3.Angle (this.transform.forward, zero);
		if (Vector3.Dot (this.transform.up, zero) < 0) {
			pitchAngle = - pitchAngle;
		}
		return pitchAngle;
	}
}
