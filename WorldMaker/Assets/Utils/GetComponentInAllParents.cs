using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SvenFranksonTools {

	public static T GetComponentInAllParents<T> (GameObject g) {
		T c = default(T);
		Transform p = g.transform;

		while (p != null) {
			c = p.GetComponent<T> ();
			if (c != null) {
				return c;
			}
			p = p.parent;
		}

		return  default(T);
	}
}
