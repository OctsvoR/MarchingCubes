using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {

	public GameObject tool;

	[Space ()]
	public MarchingCube marchingCubePrefab;

	[Space ()]
	public Mesh tangentMesh;

	private Vector3 hitPoint;
	private Vector3 hitNormal;
	private Vector3 hitTransformPoint;
	private Vector3 hitTransformNormal;

	private void GenerateMarchingCubes (Vector3 position, int size) {
		for (int z = 0; z < size; z++) {
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					MarchingCube marchingCube = Instantiate (marchingCubePrefab);
					marchingCube.transform.position = position + new Vector3 (x, y, z);
					marchingCube.tool = tool;
				}
			}
		}
	}
}
