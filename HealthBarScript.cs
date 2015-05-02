using UnityEngine;
using System.Collections;

public class HealthBarScript : MonoBehaviour {
	public Transform target;

	// Use this for initialization
	void Start () {
		target = GetComponent<Transform>();
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 desiredPos = Camera.main.WorldToViewportPoint(target.position);
		transform.position = desiredPos;
	
	}
}
