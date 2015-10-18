using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetManager : MonoBehaviour {

	static private PlanetManager manager;
	static public PlanetManager Manager {
		get {
			if (PlanetManager.manager == null) {
				PlanetManager.manager = GameObject.FindObjectOfType<PlanetManager> ();
				if (PlanetManager.manager == null) {
					Debug.LogWarning ("No PlanetManager instance has been found. You should add one to the scene to get expected behaviour.");
					GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
					PlanetManager.manager = g.AddComponent<PlanetManager> ();

				}
			}

			return PlanetManager.manager;
		}
	}

	public int SsquareLength;
	public int SsquareLengthPow = 6;
	public int SheightMapSize;
	
	static public int squareLength {
		get {
			return Manager.SsquareLength;
		}
	}
	static public int squareLengthPow {
		get {
			return Manager.SsquareLengthPow;
		}
	}
	static public int heightMapSize {
		get {
			return Manager.SheightMapSize;
		}
	}

	private List<PlanetSquare>[] buffer = null;
	private List<PlanetSquare>[] Buffer {
		get {
			if (buffer == null) {
				buffer = new List<PlanetSquare>[10];
				for (int i = 0; i < 10; i++) {
					buffer [i] = new List<PlanetSquare> ();
				}
			}

			return buffer;
		}
	}

	public void Start () {

	}

	public void Update () {
		for (int i = 9; i >= 0; i--) {
			if (Buffer [i].Count > 0) {
				PlanetSquare ps = Buffer [i] [Buffer [i].Count - 1];
				Buffer [i].RemoveAt (Buffer [i].Count - 1);

				if (ps != null) {
					ps.Initialize ();
					return;
				}
			}
		}
	}

	public void Add (PlanetSquare ps) {
		int sub = ps.subDegree;

		if (!this.Buffer [sub].Contains (ps)) {
			this.Buffer [sub].Add (ps);
		}
	}
}
