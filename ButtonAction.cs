using UnityEngine;
using System.Collections;

/*
 * This script is on the buttons that activate traps. There are 2 types of traps. There is a cage that falls and prevents
 * enemies from attacking & moving, and there are also large spikes that fall from teh ceiling when the player steps on 
 * the button.
 */ 

public class ButtonAction : MonoBehaviour {
	public GameObject objectToBeAffected;

	void OnTriggerEnter(Collider other)
	{
		/*
		 * If the player stands on the button I change the button from green to red so they know its been pressed, and
		 * I enable gravity on the trap so it falls. If the trap is a spike, there is a trigger collider on the spikes so
		 * they kill the enemy when they fall on its head
		 */ 
		if(other.gameObject.tag == "Player")
		{
			objectToBeAffected.rigidbody.useGravity = true;
			Color green = gameObject.renderer.material.color;
			Color red = new Color(1,0,0);
			gameObject.renderer.material.color = red;
		}
	}
}
