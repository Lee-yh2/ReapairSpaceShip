using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveA : MonoBehaviour {
	Vector3 pos;
	float max = 2.0f;
	float direction = 1.0f;
	// Use this for initialization
	void Start () {
		pos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = pos;
		v.y = max * Mathf.Sin (Time.time * direction);
		transform.position = v;
}
}
