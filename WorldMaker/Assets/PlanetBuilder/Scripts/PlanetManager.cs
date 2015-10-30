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

	public int count;
	public Planet[] planets;
	public int[] orbits;
	public float[] alphas;

	public void UpdateCount () {
		if (this.planets == null) {
			this.planets = new Planet[this.count];
			this.orbits = new int[this.count];
			this.alphas = new float[this.count];
		}
		else {
			Planet[] newPlanets = new Planet[this.count];
			int[] newOrbits = new int[this.count];
			float[] newAlphas = new float[this.count];

			for (int i = 0; i < Mathf.Min (planets.Length, this.count); i++) {
				newPlanets [i] = this.planets [i];
				newOrbits [i] = this.orbits [i];
				newAlphas [i] = this.alphas [i];
			}

			this.planets = newPlanets;
			this.orbits = newOrbits;
			this.alphas = newAlphas;
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

	public void Awake () {
		this.Initialize ();
	}

	public void Initialize () {
		for (int i = 0; i < this.count; i++) {
			Planet p = GameObject.Instantiate<Planet> (this.planets [i]);
			StellarObject sO = p.GetComponent <StellarObject> ();
			sO.truePos = Quaternion.AngleAxis (this.alphas [i], Vector3.up) * Vector3.forward * this.orbits [i];
			p.Initialize ();
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
