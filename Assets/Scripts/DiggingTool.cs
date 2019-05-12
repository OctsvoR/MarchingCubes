using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiggingTool : MonoBehaviour {

	public float speed = 4f;

	private void Update () {
		transform.Translate (Input.GetAxisRaw ("Horizontal") * Time.deltaTime * speed, Input.GetAxisRaw ("Vertical") * Time.deltaTime * speed, Input.GetAxisRaw ("Lateral") * Time.deltaTime * speed);
	}
}
