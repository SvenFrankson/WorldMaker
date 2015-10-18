using UnityEngine;
using System.Collections;

public class RandomSeed {

	private static int RANDOMLENGTH = 1991;
	private static int[] rands;
	private static int[] Rands {
		get {
			if (RandomSeed.rands == null) {
				Random.seed = 0;
				RandomSeed.rands = new int[RANDOMLENGTH];
				for (int i = 0; i < RANDOMLENGTH; i++) {
					RandomSeed.rands[i] = Random.Range (0, 512);
				}
			}

			return rands;
		}
	}

	public static int Rand (int i, int j, int d) {
		if (i < 0) {
			i = -i + d;
		}
		if (j < 0) {
			j = -j + d;
		}

		string nS = "" + i + j + d;
		int n = -1;

		if (int.TryParse (nS, out n)) {
			n = n % RANDOMLENGTH;
		}
		else {
			Debug.Log ("Ouille");
			return n;
		}

		return Rands [n];
	}

	public static int Rand (int i, int j, int k, int d) {
		int index = i * 41 + j * 42 + k * 43 + d;
		
		return Rands [index % RANDOMLENGTH];
	}
}
