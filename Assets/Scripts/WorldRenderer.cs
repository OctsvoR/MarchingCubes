﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldRenderer : MonoBehaviour {

	/// <summary>
	/// A list of triangles
	/// </summary>
	public List<Triangle> trianglesList = new List<Triangle> ();

	/// <summary>
	/// An array of mesh vertices.
	/// </summary>
	private Vector3[] meshVertices;

	/// <summary>
	/// An array of mesh triangles.
	/// </summary>
	private int[] meshTriangles;

	/// <summary>
	/// An array of mesh UVs.
	/// </summary>
	private Vector2[] meshUV;

	/// <summary>
	/// An array of wireframe triangles.
	/// </summary>
	private Triangle[] triangles;

	/// <summary>
	/// A reference to the LowLevelRenderer class.
	/// </summary>
	public LowLevelRenderer lowLevelRenderer;

	/// <summary>
	/// Renders the generated terrain using wireframe.
	/// </summary>
	public void RenderWireframe () {
		triangles = new Triangle[trianglesList.Count];

		for (int i = 0; i < trianglesList.Count; i++) {
			triangles[i] = trianglesList[i];
		}

		lowLevelRenderer.triangles = triangles;
	}

	/// <summary>
	/// Renders the generated terrain.
	/// </summary>
	public void Render () {
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		meshVertices = new Vector3[trianglesList.Count * 3];
		meshTriangles = new int[trianglesList.Count * 3];
		meshUV = new Vector2[trianglesList.Count * 3];

		for (int i = 0, k = 0; i < trianglesList.Count; i++) {
			for (int j = 0; j < 3; j++, k++) {
				meshVertices[k] = trianglesList[i].positions[j];
				meshUV[k] = meshVertices[k];
				meshTriangles[k] = k;
			}
		}

		mesh.Clear ();

		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.uv = meshUV;

		mesh.RecalculateNormals ();
	}
}
