using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour {

	public MarchingCube chunkPrefab;

	[Space ()]
	public int amountX;
	public int amountZ;

	[Space (), Range (-8, 8)]
	public int height;

	[Space ()]
	public Transform diggingTool;

	private void GenerateChunks (int amountX, int amountZ) {
		for (int z = 0; z < amountZ; z++) {
			for (int x = 0; x < amountX; x++) {
				MarchingCube chunk = Instantiate (chunkPrefab);
				chunk.transform.position = new Vector3 (x, chunk.transform.position.y, z) * chunkPrefab.gcg.sizeX;
				GridCellsGenerator gcg = chunk.transform.GetChild (0).GetComponent<GridCellsGenerator> ();
				gcg.offsetX = x * gcg.sizeX * gcg.coverage;
				gcg.offsetZ = z * gcg.sizeZ * gcg.coverage;
				gcg.diggingTool = diggingTool;
				gcg.height = height;
			}
		}
		
	}

	private void Start () {
		GenerateChunks (amountX, amountZ);
	}
}
