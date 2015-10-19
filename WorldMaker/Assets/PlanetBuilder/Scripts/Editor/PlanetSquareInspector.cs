using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlanetSquare))]
public class PlanetSquareInspector : Editor {

	private PlanetSquare iTarget;
	private PlanetSquare Target {
		get {
			if (this.iTarget == null) {
				this.iTarget = (PlanetSquare) target;
			}
			return this.iTarget;
		}
	}

	public override void OnInspectorGUI () {
		Target.planet = EditorGUILayout.ObjectField ("Planet", Target.planet, typeof(Planet), true) as Planet;
		EditorGUILayout.FloatField ("IndexSize", Target.indexSize);
		EditorGUILayout.IntField ("Child Depth", Target.childDepth);
		EditorGUILayout.Vector3Field ("Center", Target.center);
		EditorGUILayout.FloatField ("SubLimit", Target.subLimit);
		EditorGUILayout.FloatField ("UnSubLimit", Target.unSubLimit);
		if (GUILayout.Button ("Find Planet")) {
			Target.FindPlanet ();
		}
		if (GUILayout.Button ("UnSubdivide")) {
			Target.UnSubdivide (false);
		}
		if (GUILayout.Button ("Subdivide")) {
			Target.Subdivide (false, 0f);
		}
	}
}
