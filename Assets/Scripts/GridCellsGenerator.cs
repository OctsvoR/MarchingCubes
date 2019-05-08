using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsGenerator : MonoBehaviour {

	public int amountX = 10;
	public int amountY = 10;
	public int amountZ = 10;

	private Scalar[,,] scalarField;
	public GridCell[,,] gridCells { get; private set; }

	private Scalar[,,] GenerateScalarField () {
		Scalar[,,] scalarField = new Scalar[amountX, amountY, amountZ];

		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z), Random.Range (0f, 1f));
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
						scalarField[x + 1, y    , z + 1].position,
						scalarField[x + 1, y    , z    ].position,
						scalarField[x    , y    , z    ].position,
					};

					float[] values = {
						scalarField[x    , y    , z + 1].value,
						scalarField[x + 1, y    , z + 1].value,
						scalarField[x + 1, y    , z    ].value,
						scalarField[x    , y    , z    ].value,
						scalarField[x    , y + 1, z + 1].value,
						scalarField[x + 1, y    , z + 1].value,
						scalarField[x + 1, y    , z    ].value,
						scalarField[x    , y    , z    ].value,
					};

					gridCells[x, y, z] = new GridCell (positions, values);
				}
			}
		}

		return gridCells;
	}

	private void Start () {
		scalarField = GenerateScalarField ();
		gridCells = GenerateGridCells ();
	}

	private void OnDrawGizmos () {
		if (scalarField != null) {
			for (int z = 0; z < amountZ; z++) {
				for (int y = 0; y < amountY; y++) {
					for (int x = 0; x < amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
			}
		}
	}
}
