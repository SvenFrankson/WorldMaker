using UnityEngine;
using System.Collections;

public class MotherShipHangar : MonoBehaviour {

	public Transform anchor;
	public float anchorStrength;
	public float anchorDist;
	
	public MotherShip targetMotherShip;
	private MotherShip TargetMotherShip {
		get {
			if (this.targetMotherShip == null) {
				this.targetMotherShip = this.GetComponentInParent<MotherShip> ();
			}
			return targetMotherShip;
		}
	}

	public void Update () {

	}
}
