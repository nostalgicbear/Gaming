using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This is attached to the ClawBug enemy. These enemies are pretty cool. Most games have that annoying weak cowardly
 * little creature. This is the clawbug. They patrol random paths. They are 100% dynamic. So they choose a random waypoint
 * and they start moving towards it. If they reach that waypoint, they choose another random waypoint, and so it the path 
 * they take is 100% random. They shoot a raycast out the front, and if they see the player they will attack. However
 * when their health drops below 50%, they will run to a random enemy on the map as they are cowardly creatures and 
 * need help. When they get to that random enemy, they will then start patrolling randomly again. The player can kill
 * these creatures by jumping on their back or throwing the spear at them. The players buddy can kill them by attacking.
 */ 

public class ClawBugScript : MonoBehaviour {
	private GameObject player;
	private GameObject buddy; //the players dinosaur buddy
	private Animator anim;
	private NavMeshAgent agent;
	private float distanceToDestination; // distance to the waypoint the enemy is travelling to
	private GameObject destination; //The destination they are going to. I.E The waypoint they are traveling to.
	private RaycastHit hit;
	private float distanceToPlayer; //distance between the bug and the player
	private float distanceToBuddy; //distance between the bug and the players buddy
	private float bugCurrentHealth;
	private GameObject enemyToRunTo; //The random enemy choosen to run to if the bugs health goes below 50
	private List<GameObject> enemies = new List<GameObject>(); //List of all enemies in the level
	private List<GameObject> waypoints = new List<GameObject>(); //list of all waypoints in the level
	public float damage = 0.1f;
	private GameObject healthManager;

	/*
	 * This is the state machine for the bug. 
	 */ 
	public enum BUG_STATE {
		PATROLLING,
		ATTACKING,
		FLEEING, //When the enemies health falls below 50%, they will flee, and run to another enemy for help.
		DEAD
	};

	public BUG_STATE state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		buddy = GameObject.FindGameObjectWithTag("buddy");
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		state = BUG_STATE.PATROLLING;
		bugCurrentHealth = GetComponent<EnemyHealth>().enemyHealth; //gets the enemies health from the ENEMYHEALTH script
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");

		/*
		 * Here I cycle through all waypoints in the game and add them to a list
		 */ 
		foreach(GameObject waypoint in GameObject.FindGameObjectsWithTag("waypoint"))
		{
			waypoints.Add(waypoint);
		}

		/*
		 * Here I cycle through all enemies in the game and add them to a list
		 */ 
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
		{
			enemies.Add(enemy);
		}

		ChooseWaypoint(); //Choose a random waypoint at the start so the enemy starts patrolling straight away

	}
	
	// Update is called once per frame
	void Update () {

		distanceToDestination = Vector3.Distance(transform.position, destination.transform.position);
		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		bugCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;

		/*
		 * If the bug reaches its destination, then I make it choose another. That way the enemy keeps patrolling
		 * between random waypoints until it encounters the player or the players buddy
		 */ 
		if(distanceToDestination <= 3)
		{
			ChooseWaypoint();
		}

		/*
		 * If at any time the bugs health is 0, then it is dead, and I change its state to DEAD.
		 */ 
		if(bugCurrentHealth <=0)
		{
			state = BUG_STATE.DEAD;
		}

		/*
		 * A lot of my enemies detect the player based on distance, however the bugs detect enemies based on sight. So
		 * it casts a raycast out in front of it. If the raycast hits either the player or the players buddy, it 
		 * changes its state to ATTACKING. 
		 */ 
		Vector3 fwd = transform.forward;
		Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

		if(Physics.Raycast(raycastStartPosition, fwd, out hit))
		{
			if(hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "buddy")
			{
				state = BUG_STATE.ATTACKING;
			}
		}

		switch(state) {
			/*
			 * When the bugs are PATROLLING they travel to a random waypoint. When they get there, they choose another
			 * random waypoint. This way it is entirely dynamic. I dont have to manually set the route.
	 		* They choose a random waypoint and go there. It is also much more interesting for the player this way as they wont
	 		* know where enemies will be.
	 		*/
		case BUG_STATE.PATROLLING:
			agent.SetDestination(destination.transform.position);

			/*
			 * This is just here in case for some reason, the destination is null. I make
			 * them choose again here.
			 */ 
			if(destination == null)
			{
				ChooseWaypoint();
			}

			break;

			/*
			 * When the bug is ATTACKING that means it has seen either the player or the buddy. So it then checks to
			 * see which is closer, the player or the buddy. I do this because I think its stupid to just make the bug 
			 * attack the one it sees first. For example, the bug could see the player, but if the buddy then moves
			 * closer, I dont think it should run past the buddy. It should attack the most immediate threat. So thats
			 * what I make it do. If its health drops below 50% it then flees.
			 */ 
		case BUG_STATE.ATTACKING:
			float distanceToClosestFoe = Mathf.Min(distanceToBuddy, distanceToPlayer); //compares distance to player and distance to buddy
			
			if(distanceToClosestFoe == distanceToBuddy)
			{
				agent.SetDestination(buddy.transform.position); //if the buddy is closer, move towards hi
			}
			if(distanceToClosestFoe == distanceToPlayer){
				agent.SetDestination(player.transform.position); //if the player is closer, move towards him
			}

			if(distanceToClosestFoe <=3.5f)
			{
				anim.SetTrigger("Attack"); //when in range of whoever is getting attacked, attack.
				if(distanceToClosestFoe == distanceToBuddy)
				{
					healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
				else{
					healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
			}

			/*
			 * If the health is below 50%, the enemy calls the Flee method. This picks a random enemy from the map.
			 * The enemy then flees to the position of that enemy as they know they are about to die.
			 */ 
			if(bugCurrentHealth <= 50)
			{
				if(distanceToBuddy <=4 || distanceToPlayer <= 4)
				{
					Flee ();
					state = BUG_STATE.FLEEING;
				}
			}

			break;

			/*
			 * If the enemy is in this state, then they are FLEEING from battle, as their health dropped below 50%.
			 * I increase their speed, as they are making a quick getaway. If they reach the enemy they are fleeing
			 * to, I then change their state back to patrolling, as they have made a safe getaway.
			 */ 
		case BUG_STATE.FLEEING:

			agent.speed = 5.0f;

			if(enemyToRunTo != null)
			{
				if(Vector3.Distance(transform.position, enemyToRunTo.transform.position) <=4.0)
				{
					state = BUG_STATE.PATROLLING;
				}
			}

			break;

			/*
			 * In this state, the enemy has died, and I play the death animation, and also call the Die() function.
			 */ 
		case BUG_STATE.DEAD:
			collider.enabled = false;
			agent.SetDestination(transform.position);
			anim.SetTrigger("Die");
			StartCoroutine("Die");
			break;
		}
	}

	/*
	 * This waits for 3 seconds and then destroys the enemy.
	 */ 
	IEnumerator Die()
	{
		yield return new WaitForSeconds(1.0f);
		Destroy(gameObject);
	}

	/*
	 * This method is called when the bugs health falls below 50% and it is fleeing the battle. This method goes
	 * through the enemies list, and chooses a random enemy. The bug then sets the position of that enemy as its
	 * destination.
	 */ 
	void Flee()
	{
		if(enemies.Count != 0)
		{
			enemyToRunTo = enemies[Random.Range(0, enemies.Count - 1)]; //choose a random enemy from the enemies list
			if(enemyToRunTo == null)
			{
				state = BUG_STATE.ATTACKING; //if no enemy is found, then just attack.
			}
			else{
				Debug.Log("Going to enemy " + enemyToRunTo.name);
				agent.SetDestination(enemyToRunTo.transform.position); //set your destination to the enemy position
			}
		}

		if(enemyToRunTo == null)
		{
			Flee();
		}
	}

	/*
	 * This function picks a random waypoint out of the list of waypoints and sets it as the bugs destination.
	 */ 
	void ChooseWaypoint()
	{
		destination = waypoints[Random.Range(0, waypoints.Count - 1)];
	}
	
}
