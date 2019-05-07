using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MarchingCube : MonoBehaviour {

	[Space ()]
	public Detector bdl;
	public Detector bdr;
	public Detector bul;
	public Detector bur;
	public Detector fdl;
	public Detector fdr;
	public Detector ful;
	public Detector fur;

	Vector3[] vertices = new Vector3[12];
	Triangle[] triangles = new Triangle[5];
	GridCell gridCell = new GridCell ();

	private Vector3 VertexInterp (float isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2) {
		float mu;
		Vector3 p;

		if (Mathf.Abs (isolevel - valp1) < 0.00001)
			return (p1);
		if (Mathf.Abs (isolevel - valp2) < 0.00001)
			return (p2);
		if (Mathf.Abs (valp1 - valp2) < 0.00001)
			return (p1);

		mu = (isolevel - valp1) / (valp2 - valp1);
		p.x = p1.x + mu * (p2.x - p1.x);
		p.y = p1.y + mu * (p2.y - p1.y);
		p.z = p1.z + mu * (p2.z - p1.z);

		return (p);
	}

	private int Polygonise (GridCell grid, float isolevel, Triangle[] triangles) {
		int i, ntriang;
		int cubeindex;

		cubeindex = 0;
		if (grid.val[0] < isolevel) cubeindex |= 1;
		if (grid.val[1] < isolevel) cubeindex |= 2;
		if (grid.val[2] < isolevel) cubeindex |= 4;
		if (grid.val[3] < isolevel) cubeindex |= 8;
		if (grid.val[4] < isolevel) cubeindex |= 16;
		if (grid.val[5] < isolevel) cubeindex |= 32;
		if (grid.val[6] < isolevel) cubeindex |= 64;
		if (grid.val[7] < isolevel) cubeindex |= 128;

		if (Tables.edgeTable[cubeindex] == 0)
			return (0);

		if ((Tables.edgeTable[cubeindex] & 1) != 0)
			vertices[0] =
			   VertexInterp (isolevel, grid.p[0], grid.p[1], grid.val[0], grid.val[1]);
		if ((Tables.edgeTable[cubeindex] & 2) != 0)
			vertices[1] =
			   VertexInterp (isolevel, grid.p[1], grid.p[2], grid.val[1], grid.val[2]);
		if ((Tables.edgeTable[cubeindex] & 4) != 0)
			vertices[2] =
			   VertexInterp (isolevel, grid.p[2], grid.p[3], grid.val[2], grid.val[3]);
		if ((Tables.edgeTable[cubeindex] & 8) != 0)
			vertices[3] =
			   VertexInterp (isolevel, grid.p[3], grid.p[0], grid.val[3], grid.val[0]);
		if ((Tables.edgeTable[cubeindex] & 16) != 0)
			vertices[4] =
			   VertexInterp (isolevel, grid.p[4], grid.p[5], grid.val[4], grid.val[5]);
		if ((Tables.edgeTable[cubeindex] & 32) != 0)
			vertices[5] =
			   VertexInterp (isolevel, grid.p[5], grid.p[6], grid.val[5], grid.val[6]);
		if ((Tables.edgeTable[cubeindex] & 64) != 0)
			vertices[6] =
			   VertexInterp (isolevel, grid.p[6], grid.p[7], grid.val[6], grid.val[7]);
		if ((Tables.edgeTable[cubeindex] & 128) != 0)
			vertices[7] =
			   VertexInterp (isolevel, grid.p[7], grid.p[4], grid.val[7], grid.val[4]);
		if ((Tables.edgeTable[cubeindex] & 256) != 0)
			vertices[8] =
			   VertexInterp (isolevel, grid.p[0], grid.p[4], grid.val[0], grid.val[4]);
		if ((Tables.edgeTable[cubeindex] & 512) != 0)
			vertices[9] =
			   VertexInterp (isolevel, grid.p[1], grid.p[5], grid.val[1], grid.val[5]);
		if ((Tables.edgeTable[cubeindex] & 1024) != 0)
			vertices[10] =
			   VertexInterp (isolevel, grid.p[2], grid.p[6], grid.val[2], grid.val[6]);
		if ((Tables.edgeTable[cubeindex] & 2048) != 0)
			vertices[11] =
			   VertexInterp (isolevel, grid.p[3], grid.p[7], grid.val[3], grid.val[7]);

		ntriang = 0;
		for (i = 0; Tables.triTable[cubeindex, i] != -1; i += 3) {
			triangles[ntriang].p[0] = vertices[Tables.triTable[cubeindex, i]];
			triangles[ntriang].p[1] = vertices[Tables.triTable[cubeindex, i + 1]];
			triangles[ntriang].p[2] = vertices[Tables.triTable[cubeindex, i + 2]];
			Debug.DrawLine (triangles[ntriang].p[0], triangles[ntriang].p[1]);
			Debug.DrawLine (triangles[ntriang].p[1], triangles[ntriang].p[2]);
			Debug.DrawLine (triangles[ntriang].p[2], triangles[ntriang].p[0]);
			ntriang++;
		}

		return (ntriang);
	}

	private int EvaluateBool (bool value) {
		if (value) {
			return 1;
		} else {
			return 0;
		}
	}

	private void DrawLines () {
		Debug.DrawLine (gridCell.p[3], gridCell.p[7], Color.black);
		Debug.DrawLine (gridCell.p[7], gridCell.p[6], Color.black);
		Debug.DrawLine (gridCell.p[6], gridCell.p[2], Color.black);
		Debug.DrawLine (gridCell.p[2], gridCell.p[3], Color.black);

		Debug.DrawLine (gridCell.p[2], gridCell.p[6], Color.black);
		Debug.DrawLine (gridCell.p[6], gridCell.p[5], Color.black);
		Debug.DrawLine (gridCell.p[5], gridCell.p[1], Color.black);
		Debug.DrawLine (gridCell.p[1], gridCell.p[2], Color.black);

		Debug.DrawLine (gridCell.p[1], gridCell.p[5], Color.black);
		Debug.DrawLine (gridCell.p[5], gridCell.p[4], Color.black);
		Debug.DrawLine (gridCell.p[4], gridCell.p[0], Color.black);
		Debug.DrawLine (gridCell.p[0], gridCell.p[1], Color.black);

		Debug.DrawLine (gridCell.p[0], gridCell.p[4], Color.black);
		Debug.DrawLine (gridCell.p[4], gridCell.p[7], Color.black);
		Debug.DrawLine (gridCell.p[7], gridCell.p[3], Color.black);
		Debug.DrawLine (gridCell.p[3], gridCell.p[0], Color.black);

		Debug.DrawLine (gridCell.p[7], gridCell.p[4], Color.black);
		Debug.DrawLine (gridCell.p[4], gridCell.p[5], Color.black);
		Debug.DrawLine (gridCell.p[5], gridCell.p[6], Color.black);
		Debug.DrawLine (gridCell.p[6], gridCell.p[7], Color.black);

		Debug.DrawLine (gridCell.p[0], gridCell.p[3], Color.black);
		Debug.DrawLine (gridCell.p[3], gridCell.p[2], Color.black);
		Debug.DrawLine (gridCell.p[2], gridCell.p[1], Color.black);
		Debug.DrawLine (gridCell.p[1], gridCell.p[0], Color.black);
	}

	private void UpdateGridCell () {
		gridCell.p = new Vector3[] {
			transform.position + new Vector3 (0, 0, 1) - Vector3.one * 0.5f,
			transform.position + new Vector3 (1, 0, 1) - Vector3.one * 0.5f,
			transform.position + new Vector3 (1, 0, 0) - Vector3.one * 0.5f,
			transform.position + new Vector3 (0, 0, 0) - Vector3.one * 0.5f,
			transform.position + new Vector3 (0, 1, 1) - Vector3.one * 0.5f,
			transform.position + new Vector3 (1, 1, 1) - Vector3.one * 0.5f,
			transform.position + new Vector3 (1, 1, 0) - Vector3.one * 0.5f,
			transform.position + new Vector3 (0, 1, 0) - Vector3.one * 0.5f
		};

		gridCell.val[0] = EvaluateBool (fdl.condition);
		gridCell.val[1] = EvaluateBool (fdr.condition);
		gridCell.val[2] = EvaluateBool (bdr.condition);
		gridCell.val[3] = EvaluateBool (bdl.condition);
		gridCell.val[4] = EvaluateBool (ful.condition);
		gridCell.val[5] = EvaluateBool (fur.condition);
		gridCell.val[6] = EvaluateBool (bur.condition);
		gridCell.val[7] = EvaluateBool (bul.condition);
	}

	private void Update () {
		UpdateGridCell ();

		Polygonise (gridCell, 0.5f, triangles);

		DrawLines ();
	}
}
