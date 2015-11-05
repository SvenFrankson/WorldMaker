using UnityEngine;
using System.Collections;

public class MaterialPosition : MonoBehaviour {

	public string positionName;
	public Material TargetMat;

	public void Update () {
		this.UpdateMaterialPosition ();
	}

	public void UpdateMaterialPosition () {
		this.TargetMat.SetVector (this.positionName, this.transform.position);
	}
}
