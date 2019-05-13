using System.Collections.Generic;
using UnityEngine;

public class LowLevelRenderer : MonoBehaviour {

	public Triangle[] triangles;

	public Color color;

	static Material lineMaterial;
	static void CreateLineMaterial () {
		if (!lineMaterial) {
			Shader shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Front);
			lineMaterial.SetInt ("_ZWrite", 0);
		}
	}

	private void DrawLine (Vector3 v1, Vector3 v2, Color color) {
		GL.Color (color);
		GL.Vertex (v1);
		GL.Vertex (v2);
	}
	
	/*
	public void OnRenderObject () {
		CreateLineMaterial ();
		lineMaterial.SetPass (0);

		GL.Begin (GL.LINES);
		for (int i = 0; i < triangles.Length; i++) {
			GL.Color (color);
			GL.Vertex (triangles[i].positions[0]);
			GL.Vertex (triangles[i].positions[1]);
		}
		GL.End ();

		GL.Begin (GL.LINES);
		for (int i = 0; i < triangles.Length; i++) {
			GL.Color (color);
			GL.Vertex (triangles[i].positions[1]);
			GL.Vertex (triangles[i].positions[2]);
		}
		GL.End ();

		GL.Begin (GL.LINES);
		for (int i = 0; i < triangles.Length; i++) {
			GL.Color (color);
			GL.Vertex (triangles[i].positions[2]);
			GL.Vertex (triangles[i].positions[0]);
		}
		GL.End ();
	}
	*/
}