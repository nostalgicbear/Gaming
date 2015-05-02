using UnityEngine;
using System.Collections;

/*
 * The goal for the player is to collect dinosaur eggs. This script is attached to those eggs. If the player collides with
 * an egg, they collect it. It is destroyed from the game world, and the amount of eggs remaining to be found is updated.
 */ 

public class CollectEgg : MonoBehaviour {

	/*
	 * When the player walks in to the egg, its collected, and I update the eggsRemaining variable on the EggTextureScript().
	 * The EggTextureScript controls the egg textures displayed via the GUI.
	 */ 
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			GameObject.Find("GUI_Controller").GetComponent<EggTextureScript>().eggsRemaining -= 1;
			Destroy(gameObject);
		}
	}
}
