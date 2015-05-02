using UnityEngine;
using System.Collections;

/*
 * This script is placed on the ClawBug enemies back. Every game has that weak yet annoying enemy. In this game, that
 * is the clawbug. This script is attached to a collider on the bugs back. It enables the player to be able to kill the 
 * bug by jumping on the bugs back.
 */ 

public class ClawbugsBack : MonoBehaviour {
	private EnemyHealth healthScript;
	private float damage; //the amount of damage taken when the player jumps on the bugs back
	private GameObject healthManager;
	private float increasePlayerHealth = -0.1f; //This cancels out any damage the bug might do while the player is on the bugs back


	// Use this for initialization
	void Start () {
		damage= 15.0f;
		healthScript = GetComponent<EnemyHealth>(); //enemies health
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller"); //this is reference to the player health 
	}

	void OnTriggerEnter(Collider other)
	{
		/*
		 * If the player is triggering this collider, it means that the player is on the bugs back. I send a message to the
		 * parent object, which contains all the logic for the bug, and call the TakeDamage() function to damage the 
		 * bug. Hence, the player can kill the bug by jumping on its back
		 */
		if(other.gameObject.tag == "Player")
		{
			SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			healthManager.SendMessage("PlayerTakeDamage", increasePlayerHealth, SendMessageOptions.DontRequireReceiver);

		}
	}
}
