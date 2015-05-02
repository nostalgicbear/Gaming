using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is placed on the Plant enemies in the game. THe plant enemies are the only enemy in the game that cannot
 * move from their location. Instead, when they see the player or the players buddy, they call out to all wasps in the level.
 * They start shouting for the wasps, and if the wasps hear the plants cry for help, the wasps will fly to the plants 
 * location and help the plant. If the player goes close to the plant, it will start snapping at the player or buddy.
 */ 

public class PlantScript : MonoBehaviour {
	private GameObject player;
	private GameObject buddy;
	private float distanceToPlayer;
	private float distanceToBuddy;
	private Animator anim;
	private GameObject healthManager;
	private float plantCurrentHealth;
	private RaycastHit hit;
	public float damage = 0.2f;
	private List<GameObject> wasps = new List<GameObject>();

	/*
	 * IDLE - When neither the player or Buddy are in range 
	 * ATTACKING - When either player or buddy are within biting range of the plant
	 * CALLING_WASPS - The plants call for help from the wasps when the player or buddy are within their field of view
	 * DEAD - Plant has been killed
	 */ 
	public enum PLANT_STATE {
		IDLE,
		ATTACKING,
		CALLING_WASPS,
		DEAD
	};

	public PLANT_STATE state;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		buddy = GameObject.FindGameObjectWithTag("buddy");
		anim = GetComponent<Animator>();
		plantCurrentHealth = GetComponent<EnemyHealth>().enemyHealth; //gets the enemies health from the ENEMYHEALTH script
		state = PLANT_STATE.IDLE;
		healthManager = GameObject.FindGameObjectWithTag("GUI_Controller");

		/*
		 * Here I add all the wasps in the level to a list. This is used when the plant crys out for help
		 */ 
		foreach(GameObject enemyWasp in GameObject.FindGameObjectsWithTag("wasp"))
		{
			GameObject wasp = enemyWasp.transform.parent.gameObject;
			wasps.Add(wasp);
		}
	}
	
	// Update is called once per frame
	void Update () {

		/*
		 * Here the plant shoots a raycast out forward. This acts as its line of sight and determines whether or not it
		 * has seen the player or the players buddy
		 */ 
		Vector3 fwd = transform.forward;
		Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);
		float distanceToClosestFoe = Mathf.Min(distanceToBuddy, distanceToPlayer);
		plantCurrentHealth = GetComponent<EnemyHealth>().enemyHealth;

		if(plantCurrentHealth <=0)
		{
			state = PLANT_STATE.DEAD;
		}

		switch(state) {
		case PLANT_STATE.IDLE:
			anim.SetBool("Idle", true);

			/*
			 * While the player is idle, if its raycast hits either the player or buddy, it changes its state.
			 * Depending on the distance to the player or buddy, the plant will either call for help from the wasps
			 * or attack itself
			 */ 
			if(Physics.Raycast(raycastStartPosition, fwd, out hit))
			{
				if(hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.tag == "buddy")
				{
					if(Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) <=25.0f)
					{
						state = PLANT_STATE.CALLING_WASPS;
					}
					if(Vector3.Distance(transform.position, hit.collider.gameObject.transform.position) <=5.0f)
					{
						state = PLANT_STATE.ATTACKING;
					}
				}
			}
			
			Debug.DrawRay(raycastStartPosition, fwd, Color.green);

			break;

			/*
			 * When the plant sees the player or buddy, and they are a certain distance away, the plant calls out to the
			 * wasps. If the wasps are within a certain range, they can hear the plant, and so they will start flying in
			 * the direction of the plant.
			 */ 
		case PLANT_STATE.CALLING_WASPS:
			anim.SetBool("Idle", false);

			if(distanceToClosestFoe <= 25)
			{
				anim.SetBool("CallingWasps", true);

				foreach(GameObject wasp in wasps)
				{
					if(wasp.gameObject != null)
					{
						float distanceToWasp = Vector3.Distance(transform.position, wasp.transform.position); 
						if(distanceToWasp <= 100.0f)
						{
							/*
							 * Here if the wasp has heard the plant, it changes the wasps state to BEING_SUMMONED, which
							 * makes it go dynamically calculate a path to the plant
							 */ 
							wasp.gameObject.GetComponent<NavMeshAgent>().SetDestination(transform.position);
							wasp.gameObject.GetComponent<WaspScript>().state = WaspScript.WASP_STATE.BEING_SUMMONED;
						}
					}
				}
			}
			else{
				anim.SetBool("CallingWasps", false);
				state = PLANT_STATE.IDLE;
			}

			if(distanceToClosestFoe <=5)
			{
				state = PLANT_STATE.ATTACKING;
			}

			break;

			/*
			 * If the plant is attacking, and the player or buddy are in range of its attack, then the plant damages them
			 * with its attack
			 */ 
		case PLANT_STATE.ATTACKING:

			if(distanceToClosestFoe <= 5.0f)
			{
				anim.SetTrigger("AttackFoe");
				anim.SetBool("CallingWasps", false);
				anim.SetBool("Idle", false);

				if(distanceToClosestFoe == distanceToBuddy)
				{
					transform.LookAt(buddy.transform.position);
					healthManager.SendMessage("BuddyTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
				else{
					transform.LookAt(player.transform.position);
					healthManager.SendMessage("PlayerTakeDamage", damage, SendMessageOptions.DontRequireReceiver);
				}
				
			}

			if(distanceToClosestFoe > 5.0f)
			{
				state = PLANT_STATE.IDLE;
			}

			break;

			/*
			 * If this state has been triggered, the plant has been killed.
			 */ 
		case PLANT_STATE.DEAD:
			collider.enabled = false;
			anim.SetBool("IDLE", false);
			anim.SetBool("AttackFoe", false);
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
		yield return new WaitForSeconds(2.0f);
		Destroy(gameObject);
	}
}
