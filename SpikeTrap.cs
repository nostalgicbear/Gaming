using UnityEngine;
using System.Collections;

/*
 * This script is attached to the giant spikes that can be activated when the player steps on a corresponding button.
 * If the player times it correctly and stands on the button as the enemy passes under the spikes, they will fall and
 * impale the enemy, instantly killing it.
 */ 

public class SpikeTrap : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "enemy")
		{
			other.gameObject.GetComponent<EnemyHealth>().enemyHealth -= 500;
		}
	}
}
