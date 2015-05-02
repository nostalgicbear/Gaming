using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is on the Evil Slime enemies. These enemis patrol between 4 waypoints. These waypoints can be anywhere
 * on the level and these enemies will figure out a route to get to the waypoint dynamically. They will also react to
 * the presence of teh player or the players buddy. If the player and the players buddy runs away, then the enemy will
 * continue patrolling.
 */ 

public class PatrolWaypoints : MonoBehaviour {
	private GameObject player; 
	private GameObject buddy; //reference to the players sidekick
	public Transform[] waypoints; //An array containing the enemies waypoints
	private Animator anim;
	private NavMeshAgent agent;
	private int wayPointCounter = 0;
	private Transform targetWaypoint; //the next waypoint the enemy will move towards
	private float distanceToBuddy; //distance to the players sidekick
	private float distanceToPlayer; //distance to the player
	private float playerCurrentHealth;
	private GameObject healthManager;
	public float damage = 0.2f; //This is public so I can give enemies different strengths in the inspector

	/*
	 * These enemies can be either PATROLLING, which is when they are moving between waypoints ATTACKING, which is 
	 * when they attack the player, or are DEAD.
	 */
	public enum SLIME_STATE {
		PATROLLING,
		ATTACKING,
		DEAD
	};

	public SLIME_STATE state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		buddy = GameObject.FindGameObjectWithTag("buddy");
		state = SLIME_STATE.PATROLLING;
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		targetWaypoint = waypoints[wayPointCounter];
		playerCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");
	}


	// Update is called once per frame
	void Update () {
		/*
		 * Here I constantly update the distance between the enemy and the player + his buddy. I also keep reference
		 * to the enemys health incase I need to change his state based on that
		 */ 
		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		playerCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;

		/*If the health is zero then I change state to dead, which plays a DEAD animation, and then after a few seconds
		 * deletes the body
		 */ 
		if(playerCurrentHealth <= 0)
		{
			state = SLIME_STATE.DEAD;
		}

		/*
		 * If the player or his buddy are in sight of the enemy, and the enemy isnt dead, then he will change his state
		 * to ATTACKING.
		 */ 
		if(distanceToBuddy <= 6 || distanceToPlayer <= 6)
		{
			if(state != SLIME_STATE.DEAD)
			{
				state = SLIME_STATE.ATTACKING;
			}

		}

		switch(state) {
			/*
			 * In the PATROLLONG state, the enemy will move between waypoints. As the enemy reaches his target waypoinT
			 * I increase a counter, and he then moves towards the next waypoint in the waypoint array.
			 */ 
		case SLIME_STATE.PATROLLING:

			if(Vector3.Distance(transform.position, targetWaypoint.position) < 3)
			{
				wayPointCounter += 1;
			}

			if(wayPointCounter >=4)
			{
				wayPointCounter = 0;
			}
			targetWaypoint = waypoints[wayPointCounter];
			agent.SetDestination(targetWaypoint.position);

			break;

			/*
			 * In the ATTACKING state the enemy compares the distance to both the player and the buddy. It will then
			 * attack whichever one is closer as this is the one that poses most threat.
			 */ 
		case SLIME_STATE.ATTACKING:
			/* This Mathf.Min function allows me to compare two float values and find the smaller of the two. I use it
			 * to find the shortest distance.
			 */ 
			float distanceToClosestFoe = Mathf.Min(distanceToBuddy, distanceToPlayer);

			if(distanceToClosestFoe == distanceToBuddy)
			{
				agent.SetDestination(buddy.transform.position);
			}
			else{
				agent.SetDestination(player.transform.position);
			}


			if(distanceToClosestFoe <= 4.0f)
			{
				anim.SetTrigger("AttackFoe");
				if(distanceToClosestFoe == distanceToBuddy)
				{
					healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
				else{
					healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}

			}

			if(distanceToClosestFoe >= 15)
			{
				state = SLIME_STATE.PATROLLING;
			}



			break;

			/*
			 * If the state is DEAD, then I play the DEAD animation, and call the Die() function. The reason I call
			 * it using a StartCoroutine() is becuase I declared the Die() method as IEnumerator. This allows me to
			 * specify an amount of time to wait before I call an action. In this case, I wait 3 seconds before deleting
			 * the gameobject
			 */ 
		case SLIME_STATE.DEAD:
			collider.enabled = false;
			anim.SetBool("Dead", true);
			StartCoroutine("Die");
			break;
		}
	
	}

	/*
	 * Deletes the game object a few seconds after the enemy is killed
	 */ 
	IEnumerator Die()
	{
		Debug.Log("getting ready to die");
		yield return new WaitForSeconds(3.0f);
		Destroy(gameObject);
	}
}
