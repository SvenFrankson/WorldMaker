using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Planet))]
public class PlanetInspector : Editor {

	private Planet iTarget;
	private Planet Target {
		get {
			if (this.iTarget == null) {
				this.iTarget = (Planet) target;
			}
			return this.iTarget;
		}
	}

	public override void OnInspectorGUI () {
		Target.planetName = EditorGUILayout.TextField ("Planet Name", Target.planetName);
		Target.maxSubDegree = EditorGUILayout.IntSlider ("MaxSubDegree", Target.maxSubDegree, 0, 10);
		EditorGUILayout.IntField ("HeightMapRange", Target.heightMapRange);
		EditorGUILayout.FloatField ("Radius", Target.radius);
		Target.heightRangePerCent = EditorGUILayout.Slider ("HeightRange", Target.heightRangePerCent, 0f, 50f);

		Target.atmDensity = EditorGUILayout.FloatField ("Atm Density", Target.atmDensity);
		Target.atmRangePerCent = EditorGUILayout.Slider ("Atm Range", Target.atmRangePerCent, 0f, 200f);

		Target.gravIntensity = EditorGUILayout.FloatField ("Grav Intensity", Target.gravIntensity);
		Target.gravRangePerCent = EditorGUILayout.Slider ("Grav Range", Target.gravRangePerCent, 0f, 500f);
		
		if (GUI.changed) {
			GUI.changed = false;
			Target.ReCompute ();
		}

		Target.squareTemplate = EditorGUILayout.ObjectField ("Square Template", Target.squareTemplate, typeof(GameObject), true) as GameObject;
		for (int i = 0; i < 6; i++) {
			Target.Squares [i] = EditorGUILayout.ObjectField ("Square " + i, Target.Squares [i], typeof(PlanetSquare), true) as PlanetSquare;
		}
		if (GUILayout.Button ("Initialize")) {
			Target.Initialize ();
		}
	}
}
