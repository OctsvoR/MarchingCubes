using UnityEngine;

public class LowLevelRenderer : MonoBehaviour {

	public Vector3[] vertices;

	static Material lineMaterial;
	static void CreateLineMaterial () {
		if (!lineMaterial) {
			Shader shader = Shader.Find ("Hidden/Internal-Colored");
			lineMaterial = new Material (shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			lineMaterial.SetInt ("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			lineMaterial.SetInt ("_ZWrite", 0);
		}
	}

	public void OnRenderObject () {
		CreateLineMaterial ();
		lineMaterial.SetPass (0);

		GL.PushMatrix ();
		GL.MultMatrix (transform.localToWorldMatrix);

		for (int i = 0; i < vertices.Length - 1; i++) {
			GL.Begin (GL.LINES);
			GL.Color (new Color (1, 1, 1, 0.8f));
			GL.Vertex (vertices[i]);
			GL.Vertex (vertices[i + 1]);
			GL.End ();
		}

		GL.PopMatrix ();
	}
}