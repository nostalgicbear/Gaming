using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is placed on the Wasp enemies in the game. The wasp enemies move towards the player and the buddy when
 * it can see them. When it looses sight of them, it follows its breadcrumbs back to its initial position.
 */ 

public class WaspScript : MonoBehaviour {
	private GameObject player;
	private GameObject buddy;
	private float distanceToPlayer;
	private float distanceToBuddy;
	private List<GameObject> plants = new List<GameObject>();
	private Animator anim;
	private NavMeshAgent agent;
	public int rayLength = 30; //How far the wasp can see
	private Transform lastPosition;
	private Transform currentPosition;
	private bool canSeePlayer;
	public GameObject breadCrumb;
	private List<GameObject> breadCrumbs = new List<GameObject>();
	Vector3 inititalPosition;
	Quaternion initialRotation;
	private bool hasMoved = false;
	private int crumbCounter;
	private GameObject healthManager;
	private float waspCurrentHealth;
	private float damage = 0.4f;
	
	/*
	 * The wasps states
	 */ 
	public enum WASP_STATE {
		IDLE,
		BEING_SUMMONED,
		ATTACKING,
		DEAD
	};
	
	public WASP_STATE state;
	
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		buddy = GameObject.FindGameObjectWithTag("buddy");
		state = WASP_STATE.IDLE;
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		waspCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;
		
		crumbCounter = 0;
		canSeePlayer = false;
		Vector3 inititalPosition = transform.position; //I store the wasps starting position so it knows where to return to
		initialRotation = transform.rotation; // I also make it return to its original rotation so it doesnt end up facing a wall
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");

		/*
		 * As the wasps in the game can be summoned by the plants, I add all the plants to an array.
		 */ 
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("plant"))
		{
			GameObject plant = enemy.transform.parent.gameObject;
			plants.Add(plant);
		}
	}
	
	// Update is called once per frame
	void Update () {
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
		float distanceToClosestFoe = Mathf.Min(distanceToBuddy, distanceToPlayer); //compares distance to player and distance to buddy

		waspCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;

		/*
		 * If the wasps health goes below 0, it is dead and I change its state
		 */ 
		if(waspCurrentHealth <=0)
		{
			state = WASP_STATE.DEAD;
		}
		
		RaycastHit hit;
		Vector3 castToPlayer = player.transform.position - transform.position;
		Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
		Debug.DrawRay(raycastStartPosition, castToPlayer, Color.magenta);

		/*
		 * I decided to make the wasps more determined to kill the player than to kill the players buddy, and so when
		 * the wasps see the player, they immediately start moving towards it. When the wasp can see the player it 
		 * changes the canSee varible to true. When this is true, the waps starts dropping breadcrumbs as it moves.
		 */ 
		if(Physics.Raycast(raycastStartPosition, castToPlayer ,out hit, rayLength))
		{
			if(hit.collider.gameObject.tag == "Player")
			{
				canSeePlayer = true; //wasp starts dropping breadcrumbs when this is true
				agent.SetDestination(player.transform.position);
				transform.LookAt(player.transform.position);
			}
			else
			{
				canSeePlayer = false;
				returnToStartingPosition();
			}
		}

		/*
		 * If the player or buddy is in range of the wasp, it faces them and starts attacking with its stinger.
		 */
		if(distanceToClosestFoe <=3.5f)
		{
			anim.SetBool("Moving", false);
			anim.SetBool("Idle", false);
			anim.SetTrigger("Attack"); //when in range of whoever is getting attacked, attack.
			if(distanceToClosestFoe == distanceToBuddy)
			{
				transform.LookAt(buddy.transform.position);
				healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}
			else{
				healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}
		}

		/*
		 * When this is truem, the wasp can see the player and so it starts dropping breadcrumbs as it moves towards it
		 */ 
		if(canSeePlayer == true)
		{
			hasMoved = true;
			if(breadCrumbs.Count > 1)
			{
				/*
				 * Breadcrumbs are only dropped when the wasp has travelled a certain distance from the last crumb
				 */ 
				if(Vector3.Distance(transform.position, breadCrumbs[breadCrumbs.Count-1].transform.position) > 5f)
				{
					dropBreadCrumb();
				}
			}
			else 
			{
				dropBreadCrumb();
			}
		}
		/*
		 * If the wasp can no longer see the player, but has moved from its initial starting position, then the 
		 * returnToStartingPosition() function is called. This function makes the wasp track back through the breadcrumbs
		 * it has dropped to get back to its starting position.
		 */ 
		if(canSeePlayer == false && hasMoved)
		{
			returnToStartingPosition();
		}

		switch(state) {
		case WASP_STATE.IDLE:
			anim.SetBool("Moving", false);
			break;

			/*
			 * The code for this is actually called when the plant sees the player. When the plany sees the player it 
			 * calls out to the wasps. Any wasps in range of the plant set their destination to be the plants position
			 * so I ended up not having to do the code here.
			 */ 
		case WASP_STATE.BEING_SUMMONED:
			break;

			
		case WASP_STATE.ATTACKING:

			break;

			/*
			 * When the wasp is dead, I stop playing any animations that are already playing, and I play the death 
			 * animation. I then call the Die() function which removes the dead wasp gameobject.
			 */ 
		case WASP_STATE.DEAD:
			collider.enabled = false;
			anim.SetBool("IDLE", false);
			anim.SetBool("Attack", false);
			agent.SetDestination(transform.position);
			anim.SetTrigger("Die");
			StartCoroutine("Die");
			break;
		}
	}

	/*
	 * This waits for 2 seconds and then destroys the enemy.
	 */ 
	IEnumerator Die()
	{
		yield return new WaitForSeconds(2.0f);
		Destroy(gameObject);

		foreach(GameObject crumb in breadCrumbs)
		{
			Destroy(crumb);
		}
	}

	/*
	 * This function instantiates a breadcrumb and adds it to a list of breadcrumbs. When the wasp looses sight of 
	 * the player it works backwards through this list to reach its starting position
	 */ 
	void dropBreadCrumb()
	{
		GameObject crumb = Instantiate(breadCrumb, transform.position, transform.rotation) as GameObject;
		breadCrumbs.Add(crumb);
	}

	/*
	 * When the wasp looses sight of its target, this function is called, and the wasp works backwards through
	 * the breadcrumbs array to find its path back to its initial position. As it works its way back through the path
	 * it removes the last crumb.
	 */ 
	void returnToStartingPosition()
	{
		float step = 5 * Time.deltaTime;
		if(breadCrumbs.Count > 0)
		{
			GameObject lastCrumb = breadCrumbs[breadCrumbs.Count-1];
			agent.SetDestination(lastCrumb.transform.position);
			if(Vector3.Distance(transform.position, lastCrumb.transform.position) < 1.0f)
			{
				breadCrumbs.Remove(lastCrumb);
				Destroy(lastCrumb);
				returnToStartingPosition();
			}
		}
		else{
			Quaternion newRot = initialRotation;
			transform.rotation = newRot;
		}
	}
}
