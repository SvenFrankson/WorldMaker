using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StellarObjectCenter : MonoBehaviour {

	public Vector3 truePos;
	public Vector3 TruePos {
		get {
			return this.truePos + this.transform.position;
		}
	}

	public float lowLimit = 50000f;
	public float smoothness = 100f;

	public StellarObject[] stellarObjects;

	public bool Lock = false;

	public Action firstUpdate;

	public void FindStellarObjects () {
		this.stellarObjects = GameObject.FindObjectsOfType<StellarObject> ();
	}

	public void Start () {
		this.FindStellarObjects ();
		this.firstUpdate = this.FirstUpdate;
	}

	public void LateUpdate () {
		if (this.firstUpdate != null) {
			this.firstUpdate ();
		}

		if (!Lock) {
			this.truePos += this.transform.position;
			this.transform.position = Vector3.zero;
			
			foreach (StellarObject sO in this.stellarObjects) {
				if (sO.transform.parent == null) {
					float dist = (sO.truePos - this.truePos).magnitude;
					if (dist < this.lowLimit) {
						sO.transform.position = sO.truePos - this.truePos;
						sO.transform.localScale = Vector3.one;
					}
					else {
						sO.transform.position = ((sO.truePos - this.truePos).normalized * lowLimit + this.smoothness * sO.transform.position) / (this.smoothness + 1f);
						float scale = lowLimit / dist;
						sO.transform.localScale = Vector3.one * scale;
					}
				}
			}
		}
	}

	public void FirstUpdate () {
		this.truePos += this.transform.position;
		this.transform.position = Vector3.zero;
		
		foreach (StellarObject sO in this.stellarObjects) {
			float dist = (sO.truePos - this.truePos).magnitude;
			if (dist < this.lowLimit) {
				sO.transform.position = sO.truePos - this.truePos;
				sO.transform.localScale = Vector3.one;
			}
			else {
				float viewDist = lowLimit;
				sO.transform.position = (sO.truePos - this.truePos).normalized * viewDist;
				float scale = viewDist / dist;
				sO.transform.localScale = Vector3.one * scale;
			}
		}

		this.firstUpdate = null;
	}
}
