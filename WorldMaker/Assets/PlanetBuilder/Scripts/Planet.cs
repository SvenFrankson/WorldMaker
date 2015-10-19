using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

	public string planetName;
	public int maxSubDegree = 8;
	public int heightMapRange;
	public float radius;
	public float heightRangePerCent = 10f;
	public float heightRange = 2048f;

	public float atmDensity;
	public float atmRangePerCent = 10f;
	public float atmRange;

	public float gravIntensity;
	public float gravRangePerCent = 100f;
	public float gravRange;

	public GameObject squareTemplate;

	private Transform subTarget;
	public Transform SubTarget {
		get {
			if (this.subTarget == null) {
				this.subTarget = FindObjectOfType<Camera> ().transform;
			}

			return this.subTarget;
		}
	}

	private PlanetSquare[] squares;
	public PlanetSquare[] Squares {
		get {
			if (this.squares == null) {
				this.squares = new PlanetSquare[6];
				this.FindSquares ();
			}
			else if (this.squares.Length != 6) {
				this.squares = new PlanetSquare[6];
				this.FindSquares ();
			}

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
		this.gravRange = this.radius * this.gravRangePerCent / 100f;
	}

	public int SeedFromName () {
		int seed = 0;
		foreach (char c in this.planetName) {
			seed += (int) c;
		}

		return Mathf.Abs (seed);
	}

	public void FindSquares () {
		PlanetSquare[] safeSquares = this.GetComponentsInChildren<PlanetSquare> ();

		for (int i = 0; i < Mathf.Min (this.squares.Length, safeSquares.Length); i++) {
			this.squares [i] = safeSquares [i];
		}
	}

	public void Start () {
		this.Initialize ();
	}

	public void Initialize () {
		this.ReCompute ();

		foreach (PlanetSquare ps in Squares) {
			if (ps != null) {
				ps.Initialize ();
			}
		}
	}
}
