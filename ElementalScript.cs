using UnityEngine;
using System.Collections;

/*
 * This script is on the "Elemental" enemy. This is a large rock enemy. It is one of the strongest enemies in the game, and
 * it does a flurry of attacks when the player is near. This enemy also guards the eggs. It will approach and attack the 
 * player if the player gets near, and if the player then goes beyond a certain distance from the elemental, it will return
 * to the egg and start guarding it once again. 
 */ 

public class ElementalScript : MonoBehaviour {

	private GameObject player;
	private GameObject buddy;
	private Animator anim;
	private NavMeshAgent agent;
	private float distanceToBuddy;
	private float distanceToPlayer;
	private float distanceToObjectToGuard; //You can specify an object for the enemy to guard. This is the distance to that object
	private RaycastHit hit;
	public GameObject objectToGuard; //What object you want the enemy to walk to and guard
	private float elementalCurrentHealth;
	private GameObject healthManager;
	public float damage = 0.8f; //damage to do to the player or the players buddy
	
	/*
	 * These enemies can be either GUARDING, ATTACKING or DEAD
	 */ 
	public enum ELEMENTAL_STATE {
		GUARDING,
		ATTACKING,
		DEAD
	};
	
	public ELEMENTAL_STATE state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		buddy = GameObject.FindGameObjectWithTag("buddy");
		state = ELEMENTAL_STATE.GUARDING;
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		elementalCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");
	}
	
	// Update is called once per frame
	void Update () {

		/*
		 * Here I calculate the distance to the player, to the players buddy, and to the object that the enemy has been
		 * assigned to guard.
		 */ 
		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		distanceToObjectToGuard = Vector3.Distance(transform.position, objectToGuard.transform.position);
		elementalCurrentHealth = GetComponent<EnemyHealth>().enemyHealth; //reference to the enemys health

		if(elementalCurrentHealth <=0)
		{
			state = ELEMENTAL_STATE.DEAD;
		}

		/*
		 * Here I cast a raycast to both the player and the players buddy.
		 */ 
		Vector3 castToPlayer = player.transform.position - transform.position;
		Vector3 castToBuddy = buddy.transform.position - transform.position;
		Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
		if(Physics.Raycast(raycastStartPosition, castToPlayer, out hit))
		{
			if(hit.collider.gameObject.tag == "Player")
			{
				/*
				 * If the raycast hits the player, that means this enemy can see the player, and so it changes its state
				 * to ATTACK
				 */ 
				if(distanceToPlayer <=30)
				{
					if(state != ELEMENTAL_STATE.DEAD)
					{
						state = ELEMENTAL_STATE.ATTACKING;
					}
				}

			}
		}

		/*
		 * If the enemys raycast hits the players buddy, then it can see the buddy and changes its state to ATTACK
		 */ 
		if(Physics.Raycast(raycastStartPosition, castToBuddy, out hit))
		{
			if(hit.collider.gameObject.tag == "buddy")
			{
				if(distanceToBuddy <=30)
				{
					if(state != ELEMENTAL_STATE.DEAD)
					{
						state = ELEMENTAL_STATE.ATTACKING;
					}
				}
			}
		}

		/*
		 * I draw the raycasts for debugging purposes in the Editor
		 */ 
		Debug.DrawRay(raycastStartPosition, castToPlayer, Color.blue);
		Debug.DrawRay(raycastStartPosition, castToBuddy, Color.blue);

		/*
		 * When GUARDING, the enemy checks its distance to the object it is meant to guard. If it is too far away, it 
		 * will move back to the object. If it is near the object, it will continue to guard the object while keeping
		 * an eye out for the player and its buddy
		 */ 
		switch(state) {
		case ELEMENTAL_STATE.GUARDING:
			if(distanceToObjectToGuard >= 15)
			{
				agent.SetDestination(objectToGuard.transform.position);
				anim.SetBool("Moving", true);
			}

			if(distanceToObjectToGuard <= 15)
			{
				anim.SetBool("Moving", false);
			}
			break;

			/*
			 * When ATTACKING, it is possible that the enemy can see both the player and the players buddy at the same
			 * time. So I calculate which enemy is closer using the Mathf.Min function. I then set the enmy to attack 
			 * whichever is closer, the player or the buddy as that is the one that is the biggest threat.
			 */ 
		case ELEMENTAL_STATE.ATTACKING:
			anim.SetBool("Moving", true);
			float distanceToClosestFoe = Mathf.Min(distanceToBuddy, distanceToPlayer);

			if(distanceToClosestFoe == distanceToBuddy)
			{
				agent.SetDestination(buddy.transform.position);
			}
			if(distanceToClosestFoe == distanceToPlayer){
				agent.SetDestination(player.transform.position);
			}

			if(distanceToClosestFoe >= 20)
			{
				state = ELEMENTAL_STATE.GUARDING;

			}

			/*
			 * Goes berserk if the player or his buddy is near. The code below makes the enemy choose any attack 
			 * animation from a number of possible ones.
			 */ 
			if(distanceToClosestFoe <=8)
			{
				string attackTriggers = string.Format("Attack {0}", Random.Range((int)1, (int)4));
				anim.SetTrigger(string.Format("Attack {0}", Random.Range((int)1, (int)4)));
			}

			/*
			 * Here the enemy damages the player and the players buddy if they are in range of its attack
			 */ 
			if(distanceToBuddy <=5)
			{
				healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}

			if(distanceToPlayer <=5)
			{
				healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
			}
			//Debug.Log("Distance to closest foe is " + distanceToClosestFoe);
			break;

			/*
			 * Enemy is dead if it enters this state
			 */ 
		case ELEMENTAL_STATE.DEAD:
			collider.enabled = false;
			anim.SetTrigger("Die");
			StartCoroutine("Die");
			break;
		}
	}

	/*
	 * Deletes the game object a few seconds after the enemy is killed
	 */ 
	IEnumerator Die()
	{
		yield return new WaitForSeconds(2.0f);
		Destroy(gameObject);
	}
}
