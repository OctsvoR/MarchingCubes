using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour {

	public MarchingCube chunkPrefab;

	[Space (10)]
	public int amountX;
	public int amountZ;

	[Space (10)]
	public Transform diggingTool;

	private void GenerateChunks (int amountX, int amountZ) {
		for (int z = 0; z < amountZ; z++) {
			for (int x = 0; x < amountX; x++) {
				MarchingCube chunk = Instantiate (chunkPrefab);
				chunk.transform.position = new Vector3 (x, chunk.transform.position.y, z) * 10f;
				GridCellsGenerator gcg = chunk.transform.GetChild (0).GetComponent<GridCellsGenerator> ();
				gcg.offsetX = x * gcg.amountX * gcg.coverage;
				gcg.offsetZ = z * gcg.amountZ * gcg.coverage;
				gcg.diggingTool = diggingTool;
			}
		}
		
	}

	private void Start () {
		GenerateChunks (amountX, amountZ);
	}
}
