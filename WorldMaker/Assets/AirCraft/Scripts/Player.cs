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
	public MotherShipChair shipSeat;
	public AirCraft airCraft;

	public Transform gravityCenter;

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
		this.GetComponent<Collider> ().enabled = !k;
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
			
			this.transform.RotateAround (this.transform.position, Vector3.Cross (this.transform.up, (this.transform.position - this.gravityCenter.position).normalized), Vector3.Angle ((this.transform.position - this.gravityCenter.position).normalized, this.transform.up));
		}
	}

	
	void FixedUpdate () {
		if (!(this.playerMode == PlayerState.PilotAirCraft)) {
			this.CRigidbody.AddForce (- (this.transform.position - this.gravityCenter.position).normalized * 10f);
		}
	}

	public void Activate () {
		Debug.Log ("Activate");
		Ray hit = new Ray (this.cam.position, this.cam.forward);
		RaycastHit hitInfo;

		Physics.Raycast (hit, out hitInfo, 5f);

		if (hitInfo.collider.GetComponent<MotherShipChair> () != null) {
			Debug.Log ("MotherShipChair Activate");
			hitInfo.collider.GetComponent<MotherShipChair> ().TakeControl (this);
		}
		if (hitInfo.collider.GetComponent<AirCraft> () != null) {
			Debug.Log ("AirCraft Activate");
			hitInfo.collider.GetComponent<AirCraft> ().TakeControl (this);
		}
	}
}
