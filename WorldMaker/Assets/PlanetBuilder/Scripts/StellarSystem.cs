using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StellarSystem : MonoBehaviour {

	static private StellarSystem manager;
	static public StellarSystem Manager {
		get {
			if (StellarSystem.manager == null) {
				StellarSystem.manager = GameObject.FindObjectOfType<StellarSystem> ();
				if (StellarSystem.manager == null) {
					Debug.LogWarning ("No PlanetManager instance has been found. You should add one to the scene to get expected behaviour.");
					GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
					StellarSystem.manager = g.AddComponent<StellarSystem> ();

				}
			}

			return StellarSystem.manager;
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

	public string stellarSystemName;
	public float planetRate;

	public Planet PlanetTemplate;

	private PlanetSquare[] squarePoll;
	public PlanetSquare[] SquarePoll {
		get {
			if (this.squarePoll == null) {
				this.squarePoll = Resources.LoadAll<PlanetSquare> ("Prefab/PlanetSquare");
			}

			return this.squarePoll;
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
	
	public int SeedFromName () {
		int seed = 0;
		foreach (char c in this.stellarSystemName) {
			seed += (int) c;
		}
		
		return Mathf.Abs (seed);
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
		this.GenerateRandom ();
	}

	public void GenerateRandom () {
		List<string> randomNames = new List<string> ();
		randomNames.Add ("Alpha");
		randomNames.Add ("Beta");
		randomNames.Add ("Gamma");
		randomNames.Add ("Delta");
		randomNames.Add ("Epsilon");
		randomNames.Add ("Zeta");
		randomNames.Add ("Eta");
		randomNames.Add ("Theta");
		randomNames.Add ("Iota");
		randomNames.Add ("Kappa");
		randomNames.Add ("Lambda");
		randomNames.Add ("Mu");
		randomNames.Add ("Nu");
		randomNames.Add ("Ksi");
		randomNames.Add ("Omicron");
		randomNames.Add ("Pi");
		randomNames.Add ("Rho");
		randomNames.Add ("Sigma");
		randomNames.Add ("Tau");
		randomNames.Add ("Upsilon");
		randomNames.Add ("Phi");
		randomNames.Add ("Khi");
		randomNames.Add ("Psi");
		randomNames.Add ("Omega");

		this.randomizer = null;
		this.squarePoll = null;

		List<PlanetSquare> tmpPoll = new List<PlanetSquare> (this.SquarePoll);

		for (int i = 1; i <= 10; i++) {
			if ((this.Randomizer.Rand (i) / 2f + 0.5f) < this.planetRate) {
				Planet p = GameObject.Instantiate<Planet> (this.PlanetTemplate);
				int nameIndex = Mathf.FloorToInt(Mathf.Abs(this.Randomizer.Rand (4 * i) * tmpPoll.Count * 42)) % randomNames.Count;
				string planetName = randomNames [nameIndex];
				randomNames.RemoveAt (nameIndex);

				int size = Mathf.RoundToInt (6.5f + 1.5f * this.Randomizer.Rand (5 * i));

				int pollIndex = Mathf.FloorToInt(Mathf.Abs(this.Randomizer.Rand (2 * i) * tmpPoll.Count * 42)) % tmpPoll.Count;
				PlanetSquare sTemplate = tmpPoll [pollIndex];
				tmpPoll.Remove (sTemplate);

				float orbitAlpha = (this.Randomizer.Rand (3 * i) + 1) * 180f;
				float orbitDist = i * 100000f;

				int heightRangePercent = Mathf.RoundToInt (10f + 5f * this.Randomizer.Rand (6 * i));
				float atmDensity = 1f + 1f * this.Randomizer.Rand (7 * i);
				int atmRangePercent = Mathf.RoundToInt (30f + 20f * this.Randomizer.Rand (8 * i));
				float gravIntensity = 10f + (size - 6) * 5 + 4f * this.Randomizer.Rand (9 * i);
				gravIntensity = Mathf.Max (gravIntensity, 1f);

				p.Setup (planetName, sTemplate, size, orbitDist, orbitAlpha, gravIntensity, atmDensity, atmRangePercent, heightRangePercent);

				p.Initialize ();
			}
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
