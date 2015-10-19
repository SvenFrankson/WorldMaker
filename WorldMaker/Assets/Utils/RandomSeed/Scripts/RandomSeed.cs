using UnityEngine;
using System.Collections;

public class RandomSeed {

	private int seed = 42;
	private int RANDOMLENGTH = 1991;

	private float[] rands;
	private float[] Rands {
		get {
			if (this.rands == null) {
				Random.seed = this.seed;
				this.rands = new float[RANDOMLENGTH];
				for (int i = 0; i < RANDOMLENGTH; i++) {
					this.rands[i] = Random.Range (-1f, 1f);
				}
			}

			return rands;
		}
	}

	public RandomSeed (int seed) {
		this.seed = seed;
	}

	public float Rand (int i, int j, int k, int d) {
		int index = i * 41 + j * 42 + k * 43 + d;
		
		return Rands [index % RANDOMLENGTH];
	}
}
