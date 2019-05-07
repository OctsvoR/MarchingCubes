using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

	public bool condition;

	private void OnTriggerEnter (Collider other) {
		condition = true;
	}
}
