using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is attached to the healthpacks in the game.
 * I think thats its not enough to just have dynamic enemies. THe game needed the health packs to be randomly positioned 
 * too, so I do that via this script.
 */ 

public class RandomlyPlaceHealth : MonoBehaviour {
	public GameObject healthPack; //the healthpack prefab that will be instantiated
	private List<GameObject> waypoints = new List<GameObject>(); // a list of waypoints located randomly throughout the level

	// Use this for initialization
	void Start () {

		/*
		 * Here I add all the waypoints in the level to a list
		 */ 
		foreach(GameObject waypoint in GameObject.FindGameObjectsWithTag("waypoint"))
		{
			waypoints.Add(waypoint);
		}

		/*
		 * Here I place 10 health packs at random waypoint locations throughout the level
		 */ 
		for(int i = 0; i < 10; i++)
		{
			Instantiate(healthPack, waypoints[Random.Range(0, waypoints.Count - 1)].transform.position, Quaternion.identity);
		}
	}
}
