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

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Less")) {
			Target.count --;
			Target.count = Mathf.Max (Target.count, 0);
			Target.UpdateCount ();
		}
		EditorGUILayout.IntField ("Planet Count", Target.count);
		if (GUILayout.Button ("More")) {
			Target.count ++;
			Target.UpdateCount ();
		}
		EditorGUILayout.EndHorizontal ();
		
		for (int i = 0; i < Target.count; i++) {
			Target.planets [i] = EditorGUILayout.ObjectField ("Planet " + i, Target.planets [i], typeof(Planet), false) as Planet;
			Target.orbits [i] = EditorGUILayout.IntField ("Orbit " + i, Target.orbits [i]);
			Target.alphas [i] = EditorGUILayout.FloatField ("Alpha " + i, Target.alphas [i]);
		}
	}
}
