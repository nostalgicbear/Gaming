using UnityEngine;
using System.Collections;

/*
 * This script is attached to the health packs located throughout the game. When a player collides with them, I erase
 * them from the game world, and alter the "holdingHealth" variable in the PlayerControlScript(). When this is true,
 * the player has collected a health pack.
 */ 

public class CollectHeath : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>().holdingHealth = true;
			Destroy(gameObject);
		}
	}
}
