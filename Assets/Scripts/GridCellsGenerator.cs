using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsGenerator : MonoBehaviour {

	public int amountX = 10;
	public int amountY = 10;
	public int amountZ = 10;

	private Scalar[,,] scalarField;
	public GridCell[,,] gridCells { get; set; }

	public float offset;
	public float coverage;

	[Range (0f, 20f)]
	public int height;

	private Scalar[,,] GenerateCavernsScalarField () {
		Scalar[,,] scalarField = new Scalar[amountX, amountY, amountZ];

		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					float perlin = Mathf3D.PerlinNoise3D (
						(float)x / (amountX - 1) * coverage + offset / (amountX - 1),
						(float)y / (amountY - 1) * coverage + 0f     / (amountY - 1), 
						(float)z / (amountZ - 1) * coverage + 0f     / (amountZ - 1)
					);

					if (y > height) {
						perlin = 0f;
					}

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z), perlin * 2f);
				}
			}
		}

		return scalarField;
	}

	private Scalar[,,] GenerateTerrainScalarField () {
		Scalar[,,] scalarField = new Scalar[amountX, amountY, amountZ];

		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					float perlin = Mathf.PerlinNoise (
						(float)x / (amountX - 1) * coverage + offset / (amountX - 1),
						(float)z / (amountZ - 1) * coverage + 0f / (amountZ - 1)
					);

					float output = perlin;

					if (y > amountY / 2) {
						output = 0f;
					}

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y * perlin, z), output);
				}
			}
		}

		return scalarField;
	}

	private Scalar[,,] GenerateScalarField () {
		Scalar[,,] scalarField = new Scalar[amountX, amountY, amountZ];

		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					float perlin = Mathf.PerlinNoise (
						(float)x / (amountX - 1) * coverage + offset / (amountX - 1),
						(float)z / (amountZ - 1) * coverage + 0f / (amountZ - 1)
					);

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z), perlin * 2f);
				}
			}
		}

		return scalarField;
	}

	private GridCell[,,] GenerateGridCells () {
		GridCell[,,] gridCells = new GridCell[amountX - 1, amountY - 1, amountZ - 1];

		for (int z = 0; z < amountZ - 1; z++) {
			for (int y = 0; y < amountY - 1; y++) {
				for (int x = 0; x < amountX - 1; x++) {
					Vector3[] positions = {
						scalarField[x    , y    , z + 1].position,
						scalarField[x + 1, y    , z + 1].position,
						scalarField[x + 1, y    , z    ].position,
						scalarField[x    , y    , z    ].position,
						scalarField[x    , y + 1, z + 1].position,
						scalarField[x + 1, y + 1, z + 1].position,
						scalarField[x + 1, y + 1, z    ].position,
						scalarField[x    , y + 1, z    ].position,
					};

					float[] values = {
						scalarField[x    , y    , z + 1].value,
						scalarField[x + 1, y    , z + 1].value,
						scalarField[x + 1, y    , z    ].value,
						scalarField[x    , y    , z    ].value,
						scalarField[x    , y + 1, z + 1].value,
						scalarField[x + 1, y + 1, z + 1].value,
						scalarField[x + 1, y + 1, z    ].value,
						scalarField[x    , y + 1, z    ].value,
					};

					gridCells[x, y, z] = new GridCell (positions, values);
				}
			}
		}

		return gridCells;
	}

	private void Update () {
		offset += Time.deltaTime * 4;

		scalarField = GenerateTerrainScalarField ();
		gridCells = GenerateGridCells ();
	}

	private void OnDrawGizmos () {
		if (scalarField != null) {
			for (int z = 0; z < amountZ; z++) {
				/*
				for (int y = 0; y < amountY; y++) {
					for (int x = 0; x < amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
				*/
				for (int y = 0; y < 1; y++) {
					for (int x = 0; x < amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
			}
		}
	}
}
