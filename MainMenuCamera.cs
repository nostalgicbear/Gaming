using UnityEngine;
using System.Collections;

/*
 * This is placed on the camera in the main menu of the Dungeon Level. It tells the camer to move towards its specified
 * destination. This script just makes the gameobject the camera is attached to move.
 */ 

public class MainMenuCamera : MonoBehaviour {
	private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
	
	}
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination(GameObject.Find("Cage").transform.position);
	
	}
}
