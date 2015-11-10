using UnityEngine;
using System.Collections;

public class GravitationalObject : MonoBehaviour {

	public float mass;

	public Vector3 GetAttractionFor (GameObject g) {
		return this.mass / (this.transform.position - g.transform.position).sqrMagnitude * (this.transform.position - g.transform.position).normalized;
	}
}
