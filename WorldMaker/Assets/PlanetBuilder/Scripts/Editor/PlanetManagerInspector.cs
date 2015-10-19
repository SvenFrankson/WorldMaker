using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlanetManager))]
public class PlanetManagerInspector : Editor {

	private PlanetManager iTarget;
	private PlanetManager Target {
		get {
			if (this.iTarget == null) {
				this.iTarget = (PlanetManager) target;
			}
			return this.iTarget;
		}
	}

	public override void OnInspectorGUI () {
		Target.SsquareLengthPow = EditorGUILayout.IntSlider ("SquareLengthPow", Target.SsquareLengthPow, 4, 6);
		Target.StileSize = EditorGUILayout.FloatField ("Tile Size", Target.StileSize);
		EditorGUILayout.IntField ("SquareLength", Target.SsquareLength);

		if (GUI.changed) {
			GUI.changed = false;
			Target.SsquareLength = Mathf.FloorToInt (Mathf.Pow (2f, Target.SsquareLengthPow));
		}
	}
}
