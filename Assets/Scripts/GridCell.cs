using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridCell {

	public Vector3[] positions;
	public float[] values;

	public GridCell (Vector3[] positions, float[] values) {
		this.positions = positions;
		this.values = values;
	}
}
