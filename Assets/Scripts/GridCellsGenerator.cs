using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsGenerator : MonoBehaviour {

	public WorldRenderer worldRenderer;

	[Space ()]
	public int sizeX = 10;
	public int sizeY = 10;
	public int sizeZ = 10;

	public GridCell[,,] gridCells;

	[Space ()]
	public float offsetX;
	public float offsetZ;

	[Space ()]
	public int resolution;

	public float coverage;

	[HideInInspector]
	public int height;

	[Space ()]
	public float amplitude;

	[Space ()]
	public Transform diggingTool;

	private bool isDigging;

	private Scalar[,,] scalarField;

	private MarchingCube marchingCube;

	private void GenerateCavesScalarPoint (int x, int y, int z) {
		
	}

	/// <summary>
	/// Generates a scalar field of caves.
	/// </summary>
	private Scalar[,,] GenerateCavesScalarField () {
		for (int z = 0; z < sizeZ + 1; z++) {
			for (int y = 0; y < sizeY + 1; y++) {
				for (int x = 0; x < sizeX + 1; x++) {
					float perlin = Mathf3D.PerlinNoise3D (
						(float)x * (1f / (sizeX)) * coverage + offsetX / (sizeX),
						(float)y * (1f / (sizeY)) * coverage + 0f / (sizeY),
						(float)z * (1f / (sizeZ)) * coverage + offsetZ / (sizeZ)
					);

					if (y <= height - 1) {
						scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z) + transform.position, perlin * 1.2f);
					}
				}
			}
		}

		return scalarField;
	}

	/// <summary>
	/// Generates a scalar field of terrain.
	/// </summary>
	private void GenerateTerrainScalarField () {
		for (int z = 0; z < sizeZ + 1; z++) {
			for (int y = 0; y < sizeY + 1; y++) {
				for (int x = 0; x < sizeX + 1; x++) {
					float perlin = Mathf.PerlinNoise (
						(float)x * (1f / (sizeX)) * coverage + offsetX / (sizeX),
						(float)z * (1f / (sizeZ)) * coverage + offsetZ / (sizeZ)
					);

					float output = perlin * amplitude;
					int outputInteger = (int)output;
					float outputLeftover = output - outputInteger;

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z) + transform.position, 0f);
					if (y > height) {
						if (height + outputInteger < sizeY)
							scalarField[x, height + outputInteger, z] = new Scalar (new Vector3 (x, height + outputInteger, z) + transform.position, 1f);

						if (height + outputInteger + 1 < sizeY)
							scalarField[x, height + outputInteger + 1, z] = new Scalar (new Vector3 (x, height + outputInteger + 1, z) + transform.position, outputLeftover);
					}

					if (y <= height + outputInteger) {
						scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z) + transform.position, 1f);
					}
				}
			}
		}
	}

	/// <summary>
	/// Generates grid cells.
	/// </summary>
	private void GenerateGridCells () {
		for (int z = 0; z < sizeZ; z++) {
			for (int y = 0; y < sizeY; y++) {
				for (int x = 0; x < sizeX; x++) {
					Vector3[] positions = {
						scalarField[x    , y    , z + 1].position,
						scalarField[x + 1, y    , z + 1].position,
						scalarField[x + 1, y    , z    ].position,
						scalarField[x    , y    , z    ].position,
						scalarField[x    , y + 1, z + 1].position,
						scalarField[x + 1, y + 1, z + 1].position,
						scalarField[x + 1, y + 1, z    ].position,
						scalarField[x    , y + 1, z    ].position
					};

					float[] values = {
						scalarField[x    , y    , z + 1].value,
						scalarField[x + 1, y    , z + 1].value,
						scalarField[x + 1, y    , z    ].value,
						scalarField[x    , y    , z    ].value,
						scalarField[x    , y + 1, z + 1].value,
						scalarField[x + 1, y + 1, z + 1].value,
						scalarField[x + 1, y + 1, z    ].value,
						scalarField[x    , y + 1, z    ].value
					};

					gridCells[x, y, z] = new GridCell (positions, values);
				}
			}
		}
	}

	/// <summary>
	/// Generates a digger sphere.
	/// </summary>
	/// <param name="position">The centre point of the digger sphere.</param>
	/// <param name="radius">The radius of the digger sphere.</param>
	private void GenerateDigger (Vector3 position, int radius) {
		for (int z = -radius; z < radius; z++) {
			for (int y = -radius; y < radius; y++) {
				for (int x = -radius; x < radius; x++) {
					Vector3 delta = position + new Vector3 (x, y, z) - transform.position;

					if (
						Mathf.RoundToInt (delta.x) < sizeX + 1 &&
						Mathf.RoundToInt (delta.y) < sizeY + 1 &&
						Mathf.RoundToInt (delta.z) < sizeZ + 1 &&
						Mathf.RoundToInt (delta.x) >= 0 &&
						Mathf.RoundToInt (delta.y) >= 0 &&
						Mathf.RoundToInt (delta.z) >= 0
					) {
						scalarField[
							Mathf.RoundToInt (delta.x),
							Mathf.RoundToInt (delta.y),
							Mathf.RoundToInt (delta.z)
						]
						=
						new Scalar (
							new Vector3 (
								Mathf.RoundToInt (delta.x + transform.position.x),
								Mathf.RoundToInt (delta.y + transform.position.y),
								Mathf.RoundToInt (delta.z + transform.position.z)
							),
							0f
						);
					}
				}
			}
		}
	}

	private void Update () {
		isDigging = false;

		if (Input.GetKey (KeyCode.Space)) {
			isDigging = true;
		}

		marchingCube = transform.parent.GetComponent<MarchingCube> ();

		if (isDigging) {
			float distance = Vector3.Distance (diggingTool.position, transform.position + new Vector3 (sizeX, 0, sizeZ) * 0.5f);

			if (distance <= 10f) {
				GenerateDigger (diggingTool.position, 3);
				GenerateGridCells ();
				marchingCube.doMarch = true;
			} else {
				marchingCube.doMarch = false;
			}
		} else {
			marchingCube.doMarch = false;
		}
	}

	private void Start () {
		marchingCube = transform.parent.GetComponent<MarchingCube> ();

		GenerateTerrainScalarField ();
		//GenerateCavesScalarField ();
		GenerateGridCells ();

		marchingCube.doMarch = true;
	}

	private void Init () {
		scalarField = new Scalar[sizeX + 1, sizeY + 1, sizeZ + 1];

		for (int z = 0; z < sizeZ + 1; z++) {
			for (int y = 0; y < sizeY + 1; y++) {
				for (int x = 0; x < sizeX + 1; x++) {
					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z), 1f);
				}
			}
		}

		gridCells = new GridCell[sizeX, sizeY, sizeZ];

		for (int z = 0; z < sizeZ; z++) {
			for (int y = 0; y < sizeY; y++) {
				for (int x = 0; x < sizeX; x++) {
					gridCells[x, y, z] = new GridCell {
						positions = new Vector3[8],
						values = new float[8]
					};
				}
			}
		}
	}

	private void Awake () {
		Init ();
	}

	// Draws scalar field gizmos.
	private void DrawScalarField () {
		if (scalarField != null) {
			for (int z = 0; z <= sizeZ; z++) {
				for (int y = 0; y <= sizeY; y++) {
					for (int x = 0; x <= sizeX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
				for (int y = 0; y <= 1; y++) {
					for (int x = 0; x <= sizeX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
			}
		}
	}

	private void OnDrawGizmos () {
	}
}
