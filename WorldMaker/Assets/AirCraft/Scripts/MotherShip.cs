using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(StellarObjectCenter))]
[RequireComponent(typeof(GravitationalObject))]
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

	private GravitationalObject grav;
	public GravitationalObject Grav {
		get {
			if (grav == null) {
				this.grav = this.GetComponent<GravitationalObject> ();
			}
			
			return grav;
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

	public Vector3 speed = Vector3.zero;
	public Vector3 rotationSpeed = Vector3.zero;
	public float enginePow = 0f;
	public float enginePowMin = -10f;
	public float enginePowMax = 10f;
	public float targetSpeed = 0f;
	public float maxSpeed = 500f;
	private float speedInc = 0f;

	public float cForward;
	public float cRight;
	public float cUp;

	public float yawSpeed;
	public float pitchSpeed;
	public float rollSpeed;

	public float cYaw;
	public float cPitch;
	public float cRoll;
	
	public float forwardVelocity;
	private float rightVelocity;
	private float upVelocity;
	
	private float yawInput;
	private float pitchInput;
	private float rollInput;

	public bool playerInput = false;
	public PilotState pilotMode = PilotState.Pilot;
	public Planet orbitPlanet = null;
	public float orbitalPlanetDist = 0f;

	public enum PilotState
	{
		NoPilot,
		Pilot,
		AutoPilot,
		Orbit,
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
	
	private int selectedPlanetIndex = 0;
	public int SelectedPlanetIndex {
		get {
			return this.selectedPlanetIndex;
		}
		set {
			this.selectedPlanetIndex = value;
			this.selectedPlanetIndex = Mathf.Max (0, this.selectedPlanetIndex);
			this.selectedPlanetIndex = Mathf.Min (this.Planets.Count - 1, this.selectedPlanetIndex);
		}
	}

	public Planet SelectedPlanet {
		get {
			return this.Planets [this.SelectedPlanetIndex].Key;
		}
	}

	public float SelectedPlanetDist {
		get {
			return this.Planets [this.SelectedPlanetIndex].Value;
		}
	}

	private float localAtm = 0f;

	void Start () {
		this.enginePow = 0f;
		this.SwitchModeTo (this.pilotMode);
	}

	void InputUpdate () {
		if (Input.GetKeyDown (KeyCode.Keypad8)) {
			Time.timeScale *= 2f;
			Time.timeScale = Mathf.Min (Time.timeScale, 8f);
		}
		if (Input.GetKeyDown (KeyCode.Keypad2)) {
			Time.timeScale /= 2f;
			Time.timeScale = Mathf.Max (Time.timeScale, 1/8f);
		}

		this.enginePow = 0f;
		if (this.forwardVelocity < this.targetSpeed * 0.99f) {
			this.enginePow = this.enginePowMax;
		}
		if (this.forwardVelocity > this.targetSpeed * 1.1f) {
			this.enginePow = this.enginePowMin;
		}

		if (this.pilotMode == PilotState.Pilot) {
			if (Input.GetKeyDown (KeyCode.W)) {
				this.targetSpeed += 1f;
			}
			if (Input.GetKey (KeyCode.W)) {
				this.speedInc += Time.deltaTime * 5f;
				this.targetSpeed += this.speedInc * Time.deltaTime;
			}
			if (Input.GetKeyUp (KeyCode.W)) {
				this.speedInc = 0f;
				this.targetSpeed = Mathf.Floor (this.targetSpeed);
			}
			
			if (Input.GetKeyDown (KeyCode.S)) {
				this.targetSpeed -= 1f;
			}
			if (Input.GetKey (KeyCode.S)) {
				this.speedInc += Time.deltaTime * 5f;
				this.targetSpeed -= this.speedInc * Time.deltaTime;
			}
			if (Input.GetKeyUp (KeyCode.S)) {
				this.speedInc = 0f;
				this.targetSpeed = Mathf.Floor (this.targetSpeed);
			}

			this.targetSpeed = Mathf.Max (this.targetSpeed, 0f);
			this.targetSpeed = Mathf.Min (this.targetSpeed, this.maxSpeed);
		}
		else if (this.pilotMode == PilotState.AutoPilot) {
			this.targetSpeed = this.maxSpeed;
			if (this.SelectedPlanetDist < this.SelectedPlanet.radius * 10f) {
				this.targetSpeed = this.maxSpeed / 2f;
			}
			this.SwitchModeTo (PilotState.OrbitAutoPilot);
		}
		else if (this.pilotMode == PilotState.OrbitAutoPilot) {
			this.targetSpeed = Mathf.Sqrt (this.SelectedPlanet.Grav.mass / this.SelectedPlanetDist);
			this.SwitchModeTo (PilotState.Orbit);
		}

		if (this.playerInput) {
			if (Input.GetKeyDown (KeyCode.P)) {
				if (this.pilotMode == PilotState.Pilot) {
					this.SwitchModeTo (PilotState.AutoPilot);
				}
				else {
					this.SwitchModeTo (PilotState.Pilot);
				}
			}
			
			if (this.pilotMode == PilotState.Pilot) {
				if (Input.GetKeyDown (KeyCode.R)) {
					this.SelectedPlanetIndex = this.SelectedPlanetIndex + 1;
				}
				else if (Input.GetKeyDown (KeyCode.F)) {
					this.SelectedPlanetIndex = this.SelectedPlanetIndex - 1;
				}
			}
		}
	}

	public Action YawAndPitchInput;

	public void YawAndPitchPlayerInput () {
		if (Input.GetKey (KeyCode.LeftAlt)) {
			yawInput = Input.GetAxis ("Mouse X");
			pitchInput = Input.GetAxis ("Mouse Y");
		}
		if (Input.GetKey (KeyCode.A)) {
			rollInput = 1f;
		}
		if (Input.GetKey (KeyCode.D)) {
			rollInput = -1f;
		}
	}
	
	public void YawAndPitchAutoPilotInput () {
		pitchInput = (PitchFor (this.SelectedPlanet) - 85f) / 36f;
		rollInput = RollFor (this.SelectedPlanet) / 36f;
	}
	
	public void YawAndPitchOrbitAutoPilotInput () {
		pitchInput = PitchFor (this.SelectedPlanet) / 36f;
		rollInput = RollFor (this.SelectedPlanet) / 36f;
	}

	public void YawAndPitchOrbitInput () {
		pitchInput = PitchFor (orbitPlanet) / 36f;
		rollInput = RollFor (orbitPlanet) / 36f;
	}

	void Update () {
		InputUpdate ();

		yawInput = 0f;
		pitchInput = 0f;
		rollInput = 0f;

		if (this.YawAndPitchInput != null) {
			this.YawAndPitchInput ();
		}

		forwardVelocity = Vector3.Dot (this.speed, this.transform.forward);
		rightVelocity = Vector3.Dot (this.speed, this.transform.right);
		upVelocity = Vector3.Dot (this.speed, this.transform.up);

		float sqrForwardVelocity = forwardVelocity * Mathf.Abs (forwardVelocity);
		float sqrRightVelocity = rightVelocity * Mathf.Abs (rightVelocity);
		float sqrUpVelocity = upVelocity * Mathf.Abs (upVelocity);

		this.speed += ((enginePow - sqrForwardVelocity * this.cForward * this.localAtm) * this.transform.forward) * Time.deltaTime;

		this.speed += (- sqrForwardVelocity * this.cForward * this.transform.forward * this.localAtm) * Time.deltaTime;
		this.speed += (- sqrRightVelocity * this.cRight * this.transform.right) * Time.deltaTime;
		this.speed += (- sqrUpVelocity * this.cUp * this.transform.up) * Time.deltaTime;

		this.rotationSpeed.x += - this.pitchSpeed * pitchInput * Time.deltaTime;
		this.rotationSpeed.y += this.yawSpeed * yawInput * Time.deltaTime;
		this.rotationSpeed.z += this.rollSpeed * rollInput * Time.deltaTime;

		this.rotationSpeed.x *= (1f - this.cPitch * Time.deltaTime);
		this.rotationSpeed.y *= (1f - this.cYaw * Time.deltaTime);
		this.rotationSpeed.z *= (1f - this.cRoll * Time.deltaTime);

		this.speed += (this.CRigidbody.mass * this.UpdatePlanets ()) * Time.deltaTime;

		if (this.pilotMode == PilotState.Orbit) {
			this.transform.position = this.transform.position + this.transform.up * (this.orbitalPlanetDist - this.DistFor (this.orbitPlanet));
		}

		this.transform.position += this.speed * Time.deltaTime;
		this.transform.RotateAround (this.transform.position, this.transform.right, this.rotationSpeed.x * Time.deltaTime);
		this.transform.RotateAround (this.transform.position, this.transform.up, this.rotationSpeed.y * Time.deltaTime);
		this.transform.RotateAround (this.transform.position, this.transform.forward, this.rotationSpeed.z * Time.deltaTime);
	}

	public bool CanEnterOrbitalAutoPilotMode () {
		if (this.SelectedPlanetDist > this.SelectedPlanet.radius * 3f) {
			return false;
		}

		return true;
	}

	public bool CanEnterOrbitalMode () {
		if (!this.CanEnterOrbitalAutoPilotMode ()) {
			return false;
		}
		if (Mathf.Abs(this.RollFor (this.SelectedPlanet)) > 10f) {
			return false;
		}
		if (Mathf.Abs(this.PitchFor (this.SelectedPlanet)) > 10f) {
			return false;
		}

		float orbitalSpeedClosest = Mathf.Sqrt (this.SelectedPlanet.Grav.mass / this.SelectedPlanetDist);

		if (Mathf.Abs ((this.forwardVelocity - orbitalSpeedClosest) / orbitalSpeedClosest) > 0.1f) {
			return false;
		}
		return true;
	}

	public void SwitchModeTo (PilotState newPilotMode) {
		if (newPilotMode == PilotState.NoPilot) {
			if (this.pilotMode == PilotState.Pilot) {
				this.YawAndPitchInput = null;
				this.orbitPlanet = null;
				this.orbitalPlanetDist = 0f;
				this.pilotMode = newPilotMode;
			}
		} 
		else if (newPilotMode == PilotState.Pilot) {
			this.YawAndPitchInput = this.YawAndPitchPlayerInput;
			this.orbitPlanet = null;
			this.orbitalPlanetDist = 0f;
			this.pilotMode = newPilotMode;
		}
		else if (newPilotMode == PilotState.AutoPilot) {
			this.YawAndPitchInput = this.YawAndPitchAutoPilotInput;
			this.orbitPlanet = null;
			this.orbitalPlanetDist = 0f;
			this.pilotMode = newPilotMode;
		}
		else if (newPilotMode == PilotState.OrbitAutoPilot) {
			if (this.pilotMode == PilotState.AutoPilot) {
				if (this.CanEnterOrbitalAutoPilotMode ()) {
					this.YawAndPitchInput = this.YawAndPitchOrbitAutoPilotInput;
					this.orbitPlanet = null;
					this.orbitalPlanetDist = 0f;
					this.pilotMode = newPilotMode;
				}
			}
		}
		else if (newPilotMode == PilotState.Orbit) {
			if (this.pilotMode == PilotState.OrbitAutoPilot) {
				if (this.CanEnterOrbitalMode ()) {
					this.orbitPlanet = this.SelectedPlanet;
					this.orbitalPlanetDist = this.SelectedPlanetDist;
					this.targetSpeed = Mathf.Sqrt (this.SelectedPlanet.Grav.mass / this.SelectedPlanetDist) / 10f;
					this.YawAndPitchInput = this.YawAndPitchOrbitInput;
					this.pilotMode = newPilotMode;
				}
			}
		}
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
					if (this.SelectedPlanetIndex == i) {
						this.SelectedPlanetIndex = i - 1;
					}
					else if (this.SelectedPlanetIndex == i - 1) {
						this.SelectedPlanetIndex = i;
					}
				}
			}
		}
		
		Vector3 gravity = this.Planets [0].Key.Grav.GetAttractionFor (this.gameObject);

		float altitude = this.Planets [0].Value - this.Planets [0].Key.radius;
		float a = (this.Planets [0].Key.atmRange - altitude) / this.Planets [0].Key.atmRange * this.Planets [0].Key.atmDensity;;
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
