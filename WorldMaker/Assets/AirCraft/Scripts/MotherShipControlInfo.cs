using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class MotherShipControlInfo : MonoBehaviour {


	public MotherShip targetMotherShip;
	private MotherShip TargetMotherShip {
		get {
			if (this.targetMotherShip == null) {
				this.targetMotherShip = this.GetComponentInParent<MotherShip> ();
			}
			return targetMotherShip;
		}
	}

	public TextMesh targetTextMesh;
	private TextMesh TargetTextMesh {
		get {
			if (this.targetTextMesh == null) {
				this.targetTextMesh = this.GetComponent<TextMesh> ();
			}
			return targetTextMesh;
		}
	}

	public MotherShip.PilotState targetPilotState;
	private string text;

	public void Start () {
		this.text = this.TargetTextMesh.text;
	}

	public void Update () {
		if (TargetMotherShip.pilotMode == targetPilotState) {
			this.TargetTextMesh.text = text;
		}
		else {
			this.TargetTextMesh.text = "";
		}
	}
}
