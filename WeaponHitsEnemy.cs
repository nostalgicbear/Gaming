using UnityEngine;
using System.Collections;

/*
 * This script is attached to the spears that are thrown by the player. They have a collider attached to their tip. If
 * the spear collides with an enemy, that means the player was accurate with their throw, and damage is done to the 
 * enemy that was hit
 */ 

public class WeaponHitsEnemy : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "enemy")
		{
			/*
			 * When hit with the spear, the enemys health is reduced by a random amount varying from 25-70
			 */ 
			other.gameObject.GetComponent<EnemyHealth>().enemyHealth -= Random.Range(25, 70);
		}
	}
}
