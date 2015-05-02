using UnityEngine;
using System.Collections;

/*
 * This script is attached to the buttons that open the Raptor Cages. Raptor cages are cages that have dinosaurs locked
 * in them. When the player steps on these buttons, it opens the cages, and the dinosaurs then join the player. 
 */ 

public class OpenRaptorCage : MonoBehaviour {

	/*
	 * Whatever object this is will be deleted when the player steps on the buton. I have it set to the cage that is 
	 * holding the dinosaurs hostage. When the player steps on the button, the cage is destroyed and the dinosaurs join
	 * the player and help him battle against the enemies
	 */ 
	public GameObject objectToDelete;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			Destroy(objectToDelete);
			Color green = gameObject.renderer.material.color;
			Color red = new Color(1,0,0);
			gameObject.renderer.material.color = red; //Changes the color once the player steps on the button so they know its been used
		}
	}
}
