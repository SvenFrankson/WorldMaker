using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {

	public string planetName;
	public int maxSubDegree = 8;
	public int heightMapRange;
	public float radius;
	public int heightRange = 2048;

	public GameObject squareTemplate;

	public Transform subTarget;

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
		foreach (PlanetSquare ps in Squares) {
			if (ps != null) {
				ps.Initialize ();
			}
		}
	}
}
