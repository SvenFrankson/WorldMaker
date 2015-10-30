using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MaterialPosition))]
public class MaterialPositionInspector : Editor {
	
	private MaterialPosition iTarget;
	private MaterialPosition Target {
		get {
			if (this.iTarget == null) {
				this.iTarget = (MaterialPosition) target;
			}
			return this.iTarget;
		}
	}
	
	public override void OnInspectorGUI () {
		base.DrawDefaultInspector ();
		if (GUILayout.Button ("Update")) {
			Target.Update ();
		}
	}
}
