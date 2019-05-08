using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathf3D {
	
	public static float PerlinNoise3D (float x, float y, float z) {
		float ab = Mathf.PerlinNoise (x, y);
		float bc = Mathf.PerlinNoise (y, z);
		float ca = Mathf.PerlinNoise (z, x);

		float ba = Mathf.PerlinNoise (y, x);
		float cb = Mathf.PerlinNoise (z, y);
		float ac = Mathf.PerlinNoise (x, z);

		float abc = ab + bc + ca + ba + cb + ac;
		
		return abc / 6f;
	}
}
