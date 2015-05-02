using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This script is on the Mage enemy in Level 3. A chimera is a type of alien, hence the script name. This script controls
 * the enemies state machine, and thus controls its behaviour.
 */ 

public class ChimeraScript : MonoBehaviour {
	private GameObject player;
	private Animator anim;
	private NavMeshAgent agent;
	private List<GameObject> dinosaurs = new List<GameObject>(); // A list of all the dinosaurs that help the player
	private float distanceToPlayer; //distance to the player
	private float distanceToBuddy; // distance to each dinosaur. I iterate through this in a foreach loop later
	private GameObject healthManager; //A reference to the script that holds this enemies health
	public float damage = 1.0f; //The amout of damage done to the player or the players buddies if in range
	private float chimeraCurrentHealth;

	/*
	 * This enemy constantly stalks the player and so it only has 3 states. It is never idle, as it is always moving
	 */ 
	public enum CHIMERA_STATE {
		WALKING,
		ATTACKING,
		DEAD
	};

	public CHIMERA_STATE state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		state = CHIMERA_STATE.WALKING;
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");
		chimeraCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;

		/*
		 * This level has multiple AI dinosaurs helping the player and so I add them all to a list so I can later
		 * calculate the distance to each one.
		 */ 
		foreach(GameObject dino in GameObject.FindGameObjectsWithTag("buddy"))
		{
			dinosaurs.Add(dino);
		}
	}
	
	// Update is called once per frame
	void Update () {

		/*
		 * If health falls below 0, then the enemy is dead
		 */ 
		if(chimeraCurrentHealth <= 0)
		{
			state = CHIMERA_STATE.DEAD;
		}

		/*
		 * This is the distance to the player
		 */ 
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

		/*
		 * This calculates the distance to each dinosaur AI helping the player
		 */ 
		foreach(GameObject buddy in dinosaurs)
		{
			if(buddy != null)
			{
				distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
			}
		}

	switch(state) {
			/*
			 * If the enemy is close enough to the player or a dinosaur, it changes its state and attacks
			 */ 
		case CHIMERA_STATE.WALKING:
			agent.SetDestination(player.transform.position);

			if(distanceToPlayer<= 4 || distanceToBuddy <= 4)
			{
				state = CHIMERA_STATE.ATTACKING;
			}
			break;

		case CHIMERA_STATE.ATTACKING:

			/*
			 * If the enemy is in range of the player, it attacks the player
			 */ 
			if(distanceToPlayer <=4)
			{
				anim.SetTrigger("Attack");
				healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}

			/*
			 * Here I iterate through all the dinosaurs and see if the enemy is in range to attack. If the enemy IS in
			 * range, I tell him to look at his target and attack, however I tell it to keep moving towards its initial
			 * destination, which is the players position. This is intentional as I wanted it to be different from other 
			 * AI in the game. This enemy stalks the player relentlessly dynamically changing its path to get to the 
			 * player
			 */ 
			foreach(GameObject buddy in dinosaurs)
			{
				if(buddy != null)
				{
					if(Vector3.Distance(transform.position, buddy.transform.position) <=4)
					{
						if(buddy != null)
						{
							anim.SetTrigger("Attack");
							transform.LookAt(buddy.transform.position);
							healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
						}
					}
					else{
						state = CHIMERA_STATE.WALKING;
					}
				}
			}
			/*
			 * If the enemy isnt near the player, it continues walking instead of attacking
			 */ 
			if(distanceToPlayer>= 4)
			{
				state = CHIMERA_STATE.WALKING;
			}
			break;

			/*
			 * If the enemy dies, I play the death animation, and then call the Die() function
			 */
		case CHIMERA_STATE.DEAD:
			anim.SetTrigger("Die");
			StartCoroutine("Die");
			break;
		}
	}

	/*
	 * THus waits a few seconds to let the death animation play, and then destroys the object
	 */ 
	IEnumerator Die()
	{
		yield return new WaitForSeconds(2.0f);
		Destroy(gameObject);
	}
}
