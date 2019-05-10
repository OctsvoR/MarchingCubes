using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldRenderer : MonoBehaviour {

	public List<Triangle> trianglesList = new List<Triangle> ();

	private Vector3[] vertices;
	private int[] triangles;
	private Vector2[] uv;

	public LowLevelRenderer llr;

	public void Render () {
		/*
		Mesh mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		vertices = new Vector3[trianglesList.Count * 3];
		triangles = new int[trianglesList.Count * 3];
		uv = new Vector2[trianglesList.Count * 3];

		for (int i = 0, k = 0; i < trianglesList.Count; i++) {
			for (int j = 0; j < 3; j++, k++) {
				vertices[k] = trianglesList[i].positions[j];
				uv[k] = vertices[k];
				triangles[k] = k;
			}
		}

		mesh.Clear ();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;

		mesh.RecalculateNormals ();
		*/

		Triangle[] ts = new Triangle[trianglesList.Count];
		for (int i = 0; i < trianglesList.Count; i++) {
			ts[i] = trianglesList[i];
		}
		llr.triangles = ts;
	}
}
