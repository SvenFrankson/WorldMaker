using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class MotherShipPlanet : MonoBehaviour {


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
		Planet p = TargetMotherShip.SelectedPlanet;
		float d = TargetMotherShip.SelectedPlanetDist;
		if (p != null) {
			TargetTextMesh.text += p.planetName + "\n";
			TargetTextMesh.text += Mathf.FloorToInt(d) + " m\n";
			TargetTextMesh.text += Mathf.FloorToInt(p.Grav.mass / 1000f) + " T\n";
			TargetTextMesh.text += (Mathf.FloorToInt(p.gravIntensity * 100f) / 1000f) + " g\n";
			TargetTextMesh.text += Mathf.FloorToInt(p.radius) + " m";
		}
	}
}
