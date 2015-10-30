using UnityEngine;
using System.Collections;

public class StellarObject : MonoBehaviour {

	public Vector3 truePos;

	private StellarObjectCenter center;
	private StellarObjectCenter Center {
		get {
			if (this.center == null) {
				this.center = GameObject.FindObjectOfType<StellarObjectCenter> ();
			}

			return this.center;
		}
	}
}
