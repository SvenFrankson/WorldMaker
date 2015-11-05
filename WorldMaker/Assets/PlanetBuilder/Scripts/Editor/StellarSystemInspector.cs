using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StellarSystem))]
public class StellarSystemInspector : Editor {

	private StellarSystem iTarget;
	private StellarSystem Target {
		get {
			if (this.iTarget == null) {
				this.iTarget = (StellarSystem) target;
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

		Target.PlanetTemplate = EditorGUILayout.ObjectField ("Planet Template", Target.PlanetTemplate, typeof(Planet), false) as Planet;

		Target.stellarSystemName = EditorGUILayout.TextField ("StellarSystem Name", Target.stellarSystemName);
		Target.planetRate = EditorGUILayout.Slider ("Planet Rate", Target.planetRate, 0f, 1f);
	}
}
