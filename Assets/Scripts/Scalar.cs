using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Scalar {
	public Vector3 position;
	public float value;

	public Scalar (Vector3 position, float value) {
		this.position = position;
		this.value = value;
	}
}