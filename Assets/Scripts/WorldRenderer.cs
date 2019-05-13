using System.Collections;
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
	private Vector3[] previousMeshVertices;

	/// <summary>
	/// An array of mesh triangles.
	/// </summary>
	private int[] meshTriangles;
	private int[] previousMeshTriangles;

	/// <summary>
	/// An array of mesh UVs.
	/// </summary>
	private Vector2[] meshUV;
	private Vector2[] previousMeshUV;

	/// <summary>
	/// An array of wireframe triangles.
	/// </summary>
	private Triangle[] triangles;

	/// <summary>
	/// A reference to the LowLevelRenderer class.
	/// </summary>
	public LowLevelRenderer lowLevelRenderer;

	private bool hasFirstRendered;

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
		if (trianglesList.Count > 0 && !hasFirstRendered) {
			meshVertices = new Vector3[trianglesList.Count * 3];
			meshTriangles = new int[trianglesList.Count * 3];
			meshUV = new Vector2[trianglesList.Count * 3];

			previousMeshVertices = new Vector3[trianglesList.Count * 3];
			previousMeshTriangles = new int[trianglesList.Count * 3];
			previousMeshUV = new Vector2[trianglesList.Count * 3];

			hasFirstRendered = true;
		}

		if (meshVertices != previousMeshVertices || meshTriangles != previousMeshTriangles || meshUV != previousMeshUV) {
			meshVertices = new Vector3[trianglesList.Count * 3];
			meshTriangles = new int[trianglesList.Count * 3];
			meshUV = new Vector2[trianglesList.Count * 3];

			previousMeshVertices = meshVertices;
			previousMeshTriangles = meshTriangles;
			previousMeshUV = meshUV;
		}

		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		for (int i = 0, k = 0; i < trianglesList.Count; i++) {
			for (int j = 0; j < 3; j++, k++) {
					meshVertices[k] = trianglesList[i].positions[j] - transform.position;
					meshTriangles[k] = k;
					meshUV[k] = meshVertices[k];
			}
		}

		mesh.Clear ();

		mesh.vertices = meshVertices;
		mesh.triangles = meshTriangles;
		mesh.uv = meshUV;

		mesh.RecalculateNormals ();
	}

	private void Awake () {
	}
}
