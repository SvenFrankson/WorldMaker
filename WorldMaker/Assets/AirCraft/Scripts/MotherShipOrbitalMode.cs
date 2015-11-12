using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class MotherShipOrbitalMode : MonoBehaviour {


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

	public void Update () {
		TargetTextMesh.text = "";
		if ((TargetMotherShip.pilotMode == MotherShip.PilotState.NoPilot) || (TargetMotherShip.pilotMode == MotherShip.PilotState.Pilot)) {
			TargetTextMesh.text += "AutoPilot Off\n";
			TargetTextMesh.text += "Hit (P) to activate.\n";
			TargetTextMesh.text += "Destination : " + TargetMotherShip.SelectedPlanet.planetName;
		}
		else {
			TargetTextMesh.text += "AutoPilot activated\n";
			TargetTextMesh.text += "Hit (P) to disable.\n";
			TargetTextMesh.text += "Destination : " + TargetMotherShip.SelectedPlanet.planetName;
		}
	}
}
