using UnityEngine;
using System.Collections;

public class StellarObjectCenter : MonoBehaviour {

	public Vector3 truePos;
	public Vector3 TruePos {
		get {
			return this.truePos + this.transform.position;
		}
	}

	public float maxDistance = 100000f;

	public StellarObject[] stellarObjects;

	public void FindStellarObjects () {
		this.stellarObjects = GameObject.FindObjectsOfType<StellarObject> ();
	}

	public void Start () {
		this.FindStellarObjects ();
	}

	public void Update () {
		this.truePos += this.transform.position;
		this.transform.position = Vector3.zero;

		foreach (StellarObject sO in this.stellarObjects) {
			float dist = (sO.truePos - this.truePos).magnitude;
			if (dist < this.maxDistance) {
				sO.transform.position = sO.truePos - this.truePos;
				sO.transform.localScale = Vector3.one;
			}
			else {
				sO.transform.position = (sO.truePos - this.truePos).normalized * maxDistance;
				float scale = this.maxDistance / dist;
				sO.transform.localScale = Vector3.one * scale;
			}
		}
	}
}
