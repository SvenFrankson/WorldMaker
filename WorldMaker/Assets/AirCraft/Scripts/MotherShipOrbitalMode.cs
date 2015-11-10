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
		Planet p = TargetMotherShip.closestPlanet;
		float d = TargetMotherShip.closestPlanetDist;
		if (p != null) {
			if (TargetMotherShip.pilotMode == MotherShip.PilotState.Orbit) {
				TargetTextMesh.text += "on\n";
			}
			else if (TargetMotherShip.pilotMode == MotherShip.PilotState.OrbitAutoPilot) {
				TargetTextMesh.text += "...\n";
			}
			else if (TargetMotherShip.CanEnterOrbitalAutoPilotMode ()) {
				TargetTextMesh.text += "ok\n";
			}
			else {
				TargetTextMesh.text += "no\n";
			}
			TargetTextMesh.text += TargetMotherShip.RollFor (p) + "\n";
			TargetTextMesh.text += TargetMotherShip.PitchFor (p) + "\n";
			TargetTextMesh.text += Mathf.FloorToInt(Mathf.Sqrt(p.Grav.mass / d)) + " m/s";
		}
	}
}
