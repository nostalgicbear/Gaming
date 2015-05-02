using UnityEngine;
using System.Collections;

/*
 * This script is on every single enemy. It controls the enemys health.
 */ 

public class EnemyHealth : MonoBehaviour {
	public float enemyHealth = 100.0f; //This was altered in the inspector for different enemies.
	private Animator anim;
	private CapsuleCollider collider;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		collider = GetComponent<CapsuleCollider>();
	}

	/*
	 * The player and the players buddy call this function when they are dealing damage to the enemies in the game. This
	 * function takes in a float value, and then subtracts it from the enemies health
	 */ 
	public void TakeDamage(float damage)
	{
		enemyHealth -= damage;
	}
}
