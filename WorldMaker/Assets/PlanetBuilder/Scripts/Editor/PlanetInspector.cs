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

	private bool showAtmHandles = false;

	public override void OnInspectorGUI () {
		Target.planetName = EditorGUILayout.TextField ("Planet Name", Target.planetName);

		Target.maxSubDegree = EditorGUILayout.IntSlider ("Size", Target.maxSubDegree, 0, 10);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Radius");
		EditorGUILayout.LabelField (Target.radius + " m");
		EditorGUILayout.EndHorizontal ();

		Target.heightRangePerCent = EditorGUILayout.IntSlider ("HeightRange (%radius)", Target.heightRangePerCent, 0, 50);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("HeightRange");
		EditorGUILayout.LabelField (Target.heightRange + " m");
		EditorGUILayout.EndHorizontal ();

		Target.atmDensity = EditorGUILayout.FloatField ("Atm Density", Target.atmDensity);
		Target.atmRangePerCent = EditorGUILayout.IntSlider ("Atm Range (%radius)", Target.atmRangePerCent, 0, 200);
		this.showAtmHandles = EditorGUILayout.Toggle ("Show", this.showAtmHandles);

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Atm Range");
		EditorGUILayout.LabelField (Target.atmRange + " m");
		EditorGUILayout.EndHorizontal ();

		Target.gravIntensity = EditorGUILayout.FloatField ("Grav Intensity", Target.gravIntensity);
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.PrefixLabel ("Mass");
		EditorGUILayout.LabelField ((Target.Grav.mass / 1000f) + " tons");
		EditorGUILayout.EndHorizontal ();
		
		if (GUI.changed) {
			GUI.changed = false;
			SceneView.RepaintAll ();
			Target.ReCompute ();
		}

		Target.squareTemplate = EditorGUILayout.ObjectField ("Square Template", Target.squareTemplate, typeof(GameObject), false) as GameObject;
		Target.waterTemplate = EditorGUILayout.ObjectField ("Water Template", Target.waterTemplate, typeof(GameObject), false) as GameObject;
		Target.atmTemplate = EditorGUILayout.ObjectField ("Atm Template", Target.atmTemplate, typeof(GameObject), false) as GameObject;

		if (GUILayout.Button ("Initialize")) {
			Target.Initialize ();
		}
	}

	public void DrawWireSphere (Vector3 position, Vector3 normal, float radius, Color color, int wires) {
		Handles.color = color;
		wires = Mathf.Max (wires, 3);
		float alphaStep = Mathf.PI / (wires + 1);

		normal = normal.normalized;
		for (int i = 1; i <= wires; i++) {
			Vector3 tmpPosition = position + radius * Mathf.Cos (i * alphaStep) * normal;
			float tmpRadius = radius * Mathf.Sin (i * alphaStep);
			Handles.DrawWireDisc(tmpPosition, normal, tmpRadius);
		}
	}

	public void OnSceneGUI () {
		if (this.showAtmHandles) {
			DrawWireSphere (Target.transform.position, Target.transform.up, Target.radius + Target.atmRange, Color.green, 5 * Target.maxSubDegree);
		}
	}
}
