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
	public float StileSize = 1f;
	
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
	static public float TileSize {
		get {
			return Manager.StileSize;
		}
	}

	private List<KeyValuePair<float, PlanetSquare>> bufferKVP = null;
	private List<KeyValuePair<float, PlanetSquare>> BufferKVP {
		get {
			if (bufferKVP == null) {
				bufferKVP = new List<KeyValuePair<float, PlanetSquare>> ();

			}
			
			return bufferKVP;
		}
	}

	public void Update () {
		while (BufferKVP.Count > 0) {
			PlanetSquare ps = BufferKVP [BufferKVP.Count - 1].Value;
			BufferKVP.RemoveAt (BufferKVP.Count - 1);
			
			if (ps != null) {
				ps.Initialize ();
				return;
			}
		}
	}

	public void Add (PlanetSquare ps, float priority) {
		for (int i = 0; i < BufferKVP.Count; i++) {
			if (BufferKVP [i].Key > priority) {
				BufferKVP.Insert (i, new KeyValuePair<float, PlanetSquare> (priority, ps));
				return;
			}
		}
		BufferKVP.Add (new KeyValuePair<float, PlanetSquare> (priority, ps));
	}
}
