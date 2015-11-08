using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class MotherShipClosestPlanet : MonoBehaviour {


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
			TargetTextMesh.text += p.planetName + "\n";
			TargetTextMesh.text += Mathf.FloorToInt(d) + " m\n";
			TargetTextMesh.text += Mathf.FloorToInt(p.mass / 1000f) + " T\n";
			TargetTextMesh.text += (Mathf.FloorToInt(p.gravIntensity * 100f) / 1000f) + " g\n";
			TargetTextMesh.text += Mathf.FloorToInt(p.radius) + " m";
			//TargetTextMesh.text += Mathf.FloorToInt(Mathf.Sqrt(p.mass / d)) + " m/s";
		}
	}
}
