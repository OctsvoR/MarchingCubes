using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellsGenerator : MonoBehaviour {

	public WorldRenderer worldRenderer;

	/// <summary>
	/// The amount of vertices at z axis.
	/// </summary>
	[Space ()]
	public int amountX = 10;

	/// <summary>
	/// The amount of vertices at z axis.
	/// </summary>
	public int amountY = 10;

	/// <summary>
	/// The amount of vertices at z axis.
	/// </summary>
	public int amountZ = 10;

	/// <summary>
	/// A 3D array of scalar field.
	/// </summary>
	private Scalar[,,] scalarField;

	/// <summary>
	/// A 3D array of grid cells.
	/// </summary>
	public GridCell[,,] gridCells;

	/// <summary>
	/// The noise map offset.
	/// </summary>>
	[Space ()]
	public float offsetX;
	public float offsetZ;

	[Space ()]
	public int resolution;

	/// <summary>
	/// Determines how far does our terrain cover noise map.
	/// </summary>
	public float coverage;

	/// <summary>
	/// The height of our terrain surface.
	/// </summary>
	[Space ()]
	public int height;

	/// <summary>
	/// The amplitude of our terrain surface curvature.
	/// </summary>
	[Range (0f, 10f)]
	public float amplitude;

	/// <summary>
	/// A reference to the digging tool.
	/// </summary>
	[Space ()]
	public Transform diggingTool;
	private bool isDigging;

	private MarchingCube marchingCube;

	/// <summary>
	/// Generates a scalar field of caves.
	/// </summary>
	private Scalar[,,] GenerateCavesScalarField () {
		for (int z = 0; z < amountZ + 1; z++) {
			for (int y = 0; y < amountY + 1; y++) {
				for (int x = 0; x < amountX + 1; x++) {
					float perlin = Mathf3D.PerlinNoise3D (
						(float)x * (1f / (amountX)) * coverage + offsetX / (amountX),
						(float)y * (1f / (amountY)) * coverage + 0f / (amountY),
						(float)z * (1f / (amountZ)) * coverage + offsetZ / (amountZ)
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
		for (int z = 0; z < amountZ + 1; z++) {
			for (int y = 0; y < amountY + 1; y++) {
				for (int x = 0; x < amountX + 1; x++) {
					float perlin = Mathf.PerlinNoise (
						(float)x * (1f / (amountX)) * coverage + offsetX / (amountX),
						(float)z * (1f / (amountZ)) * coverage + offsetZ / (amountZ)
					);

					float output = perlin * amplitude;
					int outputInteger = (int)output;
					float outputLeftover = output - outputInteger;

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z) + transform.position, 0f);
					if (y > height) {
						if (height + outputInteger < amountY)
							scalarField[x, height + outputInteger, z] = new Scalar (new Vector3 (x, height + outputInteger, z) + transform.position, 1f);

						if (height + outputInteger + 1 < amountY)
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
		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
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
						Mathf.RoundToInt (delta.x) < amountX + 1 &&
						Mathf.RoundToInt (delta.y) < amountY + 1 &&
						Mathf.RoundToInt (delta.z) < amountZ + 1 &&
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
			float distance = Vector3.Distance (diggingTool.position, transform.position + new Vector3 (amountX, 0, amountZ) * 0.5f);

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
		scalarField = new Scalar[amountX + 1, amountY + 1, amountZ + 1];

		for (int z = 0; z < amountZ + 1; z++) {
			for (int y = 0; y < amountY + 1; y++) {
				for (int x = 0; x < amountX + 1; x++) {
					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z), 1f);
				}
			}
		}

		gridCells = new GridCell[amountX, amountY, amountZ];

		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
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
			for (int z = 0; z <= amountZ; z++) {
				for (int y = 0; y <= amountY; y++) {
					for (int x = 0; x <= amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
				for (int y = 0; y <= 1; y++) {
					for (int x = 0; x <= amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
			}
		}
	}

	private void OnDrawGizmos () {
		DrawScalarField ();
	}
}
