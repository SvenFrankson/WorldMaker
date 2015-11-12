using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float speed;
	public float rotationSpeed;
	public float camSensivity;
	public Transform cam;

	public enum PlayerState
	{
		Move,
		PilotMotherShip,
		PilotAirCraft
	};

	public PlayerState playerMode;
	public MotherShipSeat shipSeat;
	public AirCraft airCraft;
	
	public GravitationalObject grav;

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
		this.GetComponent<Collider> ().enabled = !k;
		this.CRigidbody.isKinematic = k;
	}

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update () {
		if (!(this.playerMode == PlayerState.PilotAirCraft)) {
			if (this.playerMode == PlayerState.Move) {
				if (Input.GetKey (KeyCode.W)) {
					this.transform.position += this.transform.forward * this.speed * Time.deltaTime;
				}
				if (Input.GetKey (KeyCode.S)) {
					this.transform.position -= this.transform.forward * this.speed * Time.deltaTime;
				}
				if (Input.GetKey (KeyCode.A)) {
					this.transform.position -= this.transform.right * this.speed * Time.deltaTime;
				}
				if (Input.GetKey (KeyCode.D)) {
					this.transform.position += this.transform.right * this.speed * Time.deltaTime;
				}
			}
			
			if (!Input.GetKey (KeyCode.LeftAlt)) {
				float rotation = Input.GetAxis ("Mouse X");
				float look = Input.GetAxis ("Mouse Y");
				
				this.transform.RotateAround (this.transform.position, this.transform.up, rotation * rotationSpeed * Time.deltaTime);
				this.cam.RotateAround (this.cam.transform.position, this.transform.right, - look * this.camSensivity * Time.deltaTime);
			}
			this.transform.RotateAround (this.transform.position, Vector3.Cross (this.transform.up, (this.transform.position - this.grav.transform.position).normalized), Vector3.Angle ((this.transform.position - this.grav.transform.position).normalized, this.transform.up));
		}
		
		if (this.playerMode == PlayerState.Move) {
			if (Input.GetKeyDown (KeyCode.E)) {
				this.Activate ();
			}
		}
		else if (this.playerMode == PlayerState.PilotMotherShip) {
			if (Input.GetKeyDown (KeyCode.E)) {
				this.shipSeat.DropControl (this);
			}
		}
		else if (this.playerMode == PlayerState.PilotAirCraft) {
			if (Input.GetKeyDown (KeyCode.E)) {
				Debug.Log ("Try land");
				this.airCraft.TryLand ();
			}
		}
	}

	
	void FixedUpdate () {
		if (!(this.playerMode == PlayerState.PilotAirCraft)) {
			if (this.grav == null) {
				Debug.Log ("Oups ?");
			}
			else {
				this.CRigidbody.AddForce (this.grav.GetAttractionFor (this.gameObject));
			}
		}
	}

	public void Activate () {
		Ray hit = new Ray (this.cam.position, this.cam.forward);
		RaycastHit hitInfo;

		Physics.Raycast (hit, out hitInfo, 5f);

		if (hitInfo.collider.GetComponent<MotherShipSeat> () != null) {
			hitInfo.collider.GetComponent<MotherShipSeat> ().TakeControl (this);
		}
		if (SvenFranksonTools.GetComponentInAllParents <AirCraft> (hitInfo.collider.gameObject) != null) {
			this.TakeAirCraftControl (SvenFranksonTools.GetComponentInAllParents <AirCraft> (hitInfo.collider.gameObject));
		}
	}

	public void TakeAirCraftControl (AirCraft a) {
		this.airCraft = a;

		this.SetKinematic (true);
		this.transform.SetParent (a.transform, true);
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
		this.playerMode = Player.PlayerState.PilotAirCraft;

		a.TakeOff (this);
	}

	public void DropAirCraftControl (GravitationalObject g) {
		
		this.transform.SetParent (g.transform, true);
		this.grav = g;

		this.airCraft = null;

		this.playerMode = Player.PlayerState.Move;
		this.SetKinematic (false);
	}
}
