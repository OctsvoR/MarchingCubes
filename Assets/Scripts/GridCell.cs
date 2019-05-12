using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a grid cell.
/// </summary>
public struct GridCell {

	/// <summary>
	/// An array of vertices positions of the current grid cell.
	/// </summary>
	public Vector3[] positions;

	/// <summary>
	/// An array of vertices values of the current grid cell.
	/// </summary>
	public float[] values;

	/// <summary>
	/// Creates a grid cell.
	/// </summary>
	public GridCell (Vector3[] positions, float[] values) {
		this.positions = positions;
		this.values = values;
	}
}
