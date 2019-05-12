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
	/// The noise map offset at x axis.
	/// </summary>
	public float offset;

	/// <summary>
	/// Determines how far does our terrain cover noise map.
	/// </summary>
	public float coverage;

	/// <summary>
	/// The height of our terrain surface.
	/// </summary>
	[Range (0f, 8f)]
	public int height;

	/// <summary>
	/// The amplitude of our terrain surface curvature.
	/// </summary>
	[Range (0f, 5f)]
	public int amplitude;

	public Transform digger;


	/// <summary>
	/// Generates a scalar field of caves.
	/// </summary>
	private Scalar[,,] GenerateCavesScalarField () {
		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					float perlin = Mathf3D.PerlinNoise3D (
						(float)x / (amountX - 1) * coverage + offset / (amountX - 1),
						(float)y / (amountY - 1) * coverage + 0f / (amountY - 1),
						(float)z / (amountZ - 1) * coverage + 0f / (amountZ - 1)
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

	/// <summary>
	/// Generates a scalar field of terrain.
	/// </summary>
	private void GenerateTerrainScalarField () {
		for (int z = 0; z < amountZ; z++) {
			for (int y = 0; y < amountY; y++) {
				for (int x = 0; x < amountX; x++) {
					float perlin = Mathf.PerlinNoise (
						(float)x * (1f / (amountX - 1)) * coverage + offset / (amountX - 1),
						(float)z * (1f / (amountZ - 1)) * coverage + 0f / (amountZ - 1)
					);

					float output = perlin;

					scalarField[x, y, z] = new Scalar (new Vector3 (x, y, z) + transform.position, 0f);

					if (y > height) {
						int outputInteger = (int)output;
						float outputLeftover = output - outputInteger;

						if (height + outputInteger < amountY)
							scalarField[x, height + outputInteger, z] = new Scalar (new Vector3 (x, height + outputInteger, z) + transform.position, 1f);

						if (height + outputInteger + 1 < amountY)
							scalarField[x, height + outputInteger + 1, z] = new Scalar (new Vector3 (x, height + outputInteger + 1, z) + transform.position, 1f - outputLeftover);
					} else {
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
	}

	private void GenerateDigger () {
			scalarField[Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z)] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z)), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z) + 1] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z) + 1), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z)] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z)), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z) + 1] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x), Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z) + 1), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z)] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z)), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z) + 1] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y), Mathf.RoundToInt (digger.position.z) + 1), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z)] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z)), 0f);
			scalarField[Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z) + 1] = new Scalar (new Vector3 (Mathf.RoundToInt (digger.position.x) + 1, Mathf.RoundToInt (digger.position.y) + 1, Mathf.RoundToInt (digger.position.z) + 1), 0f);
	}

	private void Update () {
		offset += 0;

		GenerateTerrainScalarField ();
		GenerateDigger ();
		GenerateGridCells ();
	}

	private void Awake () {
		scalarField = new Scalar[amountX, amountY, amountZ];
		gridCells = new GridCell[amountX - 1, amountY - 1, amountZ - 1];
	}

	// Draws scalar field gizmos.
	private void DrawScalarField () {
		if (scalarField != null) {
			for (int z = 0; z < amountZ; z++) {
				for (int y = 0; y < amountY; y++) {
					for (int x = 0; x < amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
				for (int y = 0; y < 1; y++) {
					for (int x = 0; x < amountX; x++) {
						Gizmos.color = new Color (scalarField[x, y, z].value, scalarField[x, y, z].value, scalarField[x, y, z].value);
						Gizmos.DrawSphere (scalarField[x, y, z].position, 0.1f);
					}
				}
			}
		}
	}

	private void OnDrawGizmos () {
		//DrawScalarField ();
	}
}
