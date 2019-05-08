using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MarchingCube : MonoBehaviour {

	private Vector3[] vertices = new Vector3[12];
	private Triangle[] triangles = new Triangle[5];

	private GridCell gridCell;

	private List<Triangle> trianglesList = new List<Triangle> ();

	public GridCellsGenerator gcg;

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
		if (grid.values[0] < isolevel) cubeindex |= 1;
		if (grid.values[1] < isolevel) cubeindex |= 2;
		if (grid.values[2] < isolevel) cubeindex |= 4;
		if (grid.values[3] < isolevel) cubeindex |= 8;
		if (grid.values[4] < isolevel) cubeindex |= 16;
		if (grid.values[5] < isolevel) cubeindex |= 32;
		if (grid.values[6] < isolevel) cubeindex |= 64;
		if (grid.values[7] < isolevel) cubeindex |= 128;

		if (Tables.edgeTable[cubeindex] == 0)
			return (0);

		if ((Tables.edgeTable[cubeindex] & 1) != 0)
			vertices[0] =
			   VertexInterp (isolevel, grid.positions[0], grid.positions[1], grid.values[0], grid.values[1]);
		if ((Tables.edgeTable[cubeindex] & 2) != 0)
			vertices[1] =
			   VertexInterp (isolevel, grid.positions[1], grid.positions[2], grid.values[1], grid.values[2]);
		if ((Tables.edgeTable[cubeindex] & 4) != 0)
			vertices[2] =
			   VertexInterp (isolevel, grid.positions[2], grid.positions[3], grid.values[2], grid.values[3]);
		if ((Tables.edgeTable[cubeindex] & 8) != 0)
			vertices[3] =
			   VertexInterp (isolevel, grid.positions[3], grid.positions[0], grid.values[3], grid.values[0]);
		if ((Tables.edgeTable[cubeindex] & 16) != 0)
			vertices[4] =
			   VertexInterp (isolevel, grid.positions[4], grid.positions[5], grid.values[4], grid.values[5]);
		if ((Tables.edgeTable[cubeindex] & 32) != 0)
			vertices[5] =
			   VertexInterp (isolevel, grid.positions[5], grid.positions[6], grid.values[5], grid.values[6]);
		if ((Tables.edgeTable[cubeindex] & 64) != 0)
			vertices[6] =
			   VertexInterp (isolevel, grid.positions[6], grid.positions[7], grid.values[6], grid.values[7]);
		if ((Tables.edgeTable[cubeindex] & 128) != 0)
			vertices[7] =
			   VertexInterp (isolevel, grid.positions[7], grid.positions[4], grid.values[7], grid.values[4]);
		if ((Tables.edgeTable[cubeindex] & 256) != 0)
			vertices[8] =
			   VertexInterp (isolevel, grid.positions[0], grid.positions[4], grid.values[0], grid.values[4]);
		if ((Tables.edgeTable[cubeindex] & 512) != 0)
			vertices[9] =
			   VertexInterp (isolevel, grid.positions[1], grid.positions[5], grid.values[1], grid.values[5]);
		if ((Tables.edgeTable[cubeindex] & 1024) != 0)
			vertices[10] =
			   VertexInterp (isolevel, grid.positions[2], grid.positions[6], grid.values[2], grid.values[6]);
		if ((Tables.edgeTable[cubeindex] & 2048) != 0)
			vertices[11] =
			   VertexInterp (isolevel, grid.positions[3], grid.positions[7], grid.values[3], grid.values[7]);

		ntriang = 0;
		for (i = 0; Tables.triTable[cubeindex, i] != -1; i += 3) {
			triangles[ntriang].positions[0] = vertices[Tables.triTable[cubeindex, i]];
			triangles[ntriang].positions[1] = vertices[Tables.triTable[cubeindex, i + 1]];
			triangles[ntriang].positions[2] = vertices[Tables.triTable[cubeindex, i + 2]];

			Triangle triangle = new Triangle (
				new Vector3[] {
					triangles[ntriang].positions[0],
					triangles[ntriang].positions[1],
					triangles[ntriang].positions[2]
				}
			);

			trianglesList.Add (triangle);

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
		Debug.DrawLine (gridCell.positions[3], gridCell.positions[7], Color.black);
		Debug.DrawLine (gridCell.positions[7], gridCell.positions[6], Color.black);
		Debug.DrawLine (gridCell.positions[6], gridCell.positions[2], Color.black);
		Debug.DrawLine (gridCell.positions[2], gridCell.positions[3], Color.black);

		Debug.DrawLine (gridCell.positions[2], gridCell.positions[6], Color.black);
		Debug.DrawLine (gridCell.positions[6], gridCell.positions[5], Color.black);
		Debug.DrawLine (gridCell.positions[5], gridCell.positions[1], Color.black);
		Debug.DrawLine (gridCell.positions[1], gridCell.positions[2], Color.black);

		Debug.DrawLine (gridCell.positions[1], gridCell.positions[5], Color.black);
		Debug.DrawLine (gridCell.positions[5], gridCell.positions[4], Color.black);
		Debug.DrawLine (gridCell.positions[4], gridCell.positions[0], Color.black);
		Debug.DrawLine (gridCell.positions[0], gridCell.positions[1], Color.black);

		Debug.DrawLine (gridCell.positions[0], gridCell.positions[4], Color.black);
		Debug.DrawLine (gridCell.positions[4], gridCell.positions[7], Color.black);
		Debug.DrawLine (gridCell.positions[7], gridCell.positions[3], Color.black);
		Debug.DrawLine (gridCell.positions[3], gridCell.positions[0], Color.black);

		Debug.DrawLine (gridCell.positions[7], gridCell.positions[4], Color.black);
		Debug.DrawLine (gridCell.positions[4], gridCell.positions[5], Color.black);
		Debug.DrawLine (gridCell.positions[5], gridCell.positions[6], Color.black);
		Debug.DrawLine (gridCell.positions[6], gridCell.positions[7], Color.black);

		Debug.DrawLine (gridCell.positions[0], gridCell.positions[3], Color.black);
		Debug.DrawLine (gridCell.positions[3], gridCell.positions[2], Color.black);
		Debug.DrawLine (gridCell.positions[2], gridCell.positions[1], Color.black);
		Debug.DrawLine (gridCell.positions[1], gridCell.positions[0], Color.black);

		for (int i = 0; i < trianglesList.Count; i++) {
			Debug.DrawLine (trianglesList[i].positions[0], trianglesList[i].positions[1], Color.yellow);
			Debug.DrawLine (trianglesList[i].positions[1], trianglesList[i].positions[2], Color.yellow);
			Debug.DrawLine (trianglesList[i].positions[2], trianglesList[i].positions[0], Color.yellow);
		}
	}

	private void Update () {
		if (gcg.gridCells != null) {
			trianglesList.Clear ();

			for (int z = 0; z < gcg.amountZ - 1; z++) {
				for (int y = 0; y < gcg.amountY - 1; y++) {
					for (int x = 0; x < gcg.amountX - 1; x++) {
						gridCell = gcg.gridCells[x, y, z];

						Polygonise (gridCell, 0.8f, triangles);
					}
				}
			}

			DrawLines ();
		}
	}

	private void Init () {
		gridCell.positions = new Vector3[8];
		gridCell.values = new float[8];

		for (int i = 0; i < triangles.Length; i++) {
			triangles[i].positions = new Vector3[3];
		}
	}

	private void Start () {
		Init ();
	}
}
