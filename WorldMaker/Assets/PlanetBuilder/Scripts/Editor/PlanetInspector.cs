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
		Target.heightRange = EditorGUILayout.IntField ("HeightRange", Target.heightRange);

		if (GUI.changed) {
			GUI.changed = false;
			Target.ResetRandomizer ();
			Target.heightMapRange = PlanetManager.squareLength * Mathf.FloorToInt(Mathf.Pow (2f, Target.maxSubDegree)) + 1;
			Target.radius = 2f * Target.heightMapRange / Mathf.PI * PlanetManager.TileSize;
		}

		Target.atmDensity = EditorGUILayout.FloatField ("Atm Density", Target.atmDensity);
		Target.atmRange = EditorGUILayout.FloatField ("Atm Range", Target.atmRange);

		Target.gravIntensity = EditorGUILayout.FloatField ("Grav Intensity", Target.gravIntensity);
		Target.gravRange = EditorGUILayout.FloatField ("Grav Range", Target.gravRange);

		Target.squareTemplate = EditorGUILayout.ObjectField ("Square Template", Target.squareTemplate, typeof(GameObject), true) as GameObject;
		for (int i = 0; i < 6; i++) {
			Target.Squares [i] = EditorGUILayout.ObjectField ("Square " + i, Target.Squares [i], typeof(PlanetSquare), true) as PlanetSquare;
		}
		if (GUILayout.Button ("Initialize")) {
			Target.Initialize ();
		}
	}
}
