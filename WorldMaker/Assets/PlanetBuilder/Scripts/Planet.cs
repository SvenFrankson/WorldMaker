using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(StellarObject))]
public class Planet : MonoBehaviour {

	public string planetName;
	public int maxSubDegree = 8;
	public int heightMapRange;
	public float radius;
	public int heightRangePerCent = 10;
	public float heightRange = 2048f;

	public float atmDensity;
	public int atmRangePerCent = 10;
	public float atmRange;

	public float gravIntensity;
	public float mass;

	public GameObject squareTemplate;
	public GameObject waterTemplate;
	public GameObject atmTemplate;
	
	private StellarObject truePos;
	public StellarObject TruePos {
		get {
			if (this.truePos == null) {
				this.truePos = this.GetComponent<StellarObject> ();
			}
			
			return this.truePos;
		}
	}

	private Transform subTarget;
	public Transform SubTarget {
		get {
			if (this.subTarget == null) {
				this.subTarget = FindObjectOfType<Camera> ().transform;
			}

			return this.subTarget;
		}
	}

	private PlanetSquare[] squares = null;
	public PlanetSquare[] Squares {
		get {
			return this.squares;
		}
	}

	private RandomSeed randomizer = null;
	public RandomSeed Randomizer {
		get {
			if (this.randomizer == null) {
				this.randomizer = new RandomSeed (this.SeedFromName ());
			}

			return this.randomizer;
		}
	}

	public void ReCompute () {
		this.randomizer = null;
		this.heightMapRange = PlanetManager.squareLength * Mathf.FloorToInt(Mathf.Pow (2f, this.maxSubDegree)) + 1;
		this.radius = 2f * this.heightMapRange / Mathf.PI * PlanetManager.TileSize;
		this.heightRange = this.radius * this.heightRangePerCent / 100f;
		this.atmRange = this.radius * this.atmRangePerCent / 100f;
		this.mass = this.gravIntensity * this.radius * this.radius;
	}

	public int SeedFromName () {
		int seed = 0;
		foreach (char c in this.planetName) {
			seed += (int) c;
		}

		return Mathf.Abs (seed);
	}

	public void FlushPlanet () {
		Transform[] children = this.transform.GetComponentsInChildren<Transform> ();

		foreach (Transform t in children) {
			if (t != this.transform) {
				DestroyImmediate (t.gameObject);
			}
		}
	}

	public void CreateSquares () {
		this.squares = new PlanetSquare[6];

		this.squares[0] = this.AddChildPlanetSquare (0, 1, 1);
		this.squares[1] = this.AddChildPlanetSquare (2, 1, 1);
		this.squares[2] = this.AddChildPlanetSquare (1, 0, 1);
		this.squares[3] = this.AddChildPlanetSquare (1, 2, 1);
		this.squares[4] = this.AddChildPlanetSquare (1, 1, 0);
		this.squares[5] = this.AddChildPlanetSquare (1, 1, 2);
	}

	public PlanetSquare AddChildPlanetSquare (int iPos, int jPos, int kPos) {
		GameObject g = GameObject.Instantiate<GameObject> (this.squareTemplate);
		g.transform.parent = this.transform;
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;
		PlanetSquare ps = g.GetComponent <PlanetSquare> ();
		ps.subDegree = 0;
		ps.parent = null;
		ps.planet = this;
		ps.iPos = iPos;
		ps.jPos = jPos;
		ps.kPos = kPos;
		ps.name = "Square " + ps.subDegree + "." + ps.iPos + ":" + ps.jPos + ":" + ps.kPos;

		return ps;
	}

	public void Start () {
		this.Initialize ();
	}

	public void Initialize () {
		this.ReCompute ();

		this.FlushPlanet ();
		this.CreateSquares ();
		this.SetWater ();
		this.SetAtm ();

		foreach (PlanetSquare ps in Squares) {
			if (ps != null) {
				ps.Initialize ();
			}
		}
	}

	public void SetWater () {
		if (this.waterTemplate != null) {
			GameObject water = GameObject.Instantiate<GameObject> (this.waterTemplate);
			if (water != null) {
				water.transform.parent = this.transform;
				water.transform.localScale = Vector3.one * this.radius;
				water.transform.localPosition = Vector3.zero;
				water.name = "Water";
			}
		}
	}
	
	public void SetAtm () {
		if (this.atmTemplate != null) {
			GameObject atm = GameObject.Instantiate<GameObject> (this.atmTemplate);
			if (atm != null) {
				atm.transform.parent = this.transform;
				atm.transform.localScale = Vector3.one * (this.radius + this.atmRange);
				atm.transform.localPosition = Vector3.zero;
				atm.name = "Atm";
			}
		}
	}
}
