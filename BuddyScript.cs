using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuddyScript : MonoBehaviour {
	private GameObject player; //reference to the player
	private NavMeshAgent agent; //The NavMesh agent attached to the current gameobject
	private float distanceToPlayer;
	private float distanceToEnemy;
	private Animator anim; //the gameObjects animator component
	private RaycastHit hit;
	private float idleTimer = 0.0f; //This increases when the player stands idle
	private GameObject currentEnemy; //the current enemy targeted by the dinosaur
	public float damage = 1.0f;
	private bool attackingAnEnemy = false;

	/*
	 * THe dinosaur can be in 4 states. It can either be IDLE where it is standing still. This is the case when the player
	 * is not moving and there are no enemies in range. It can be FOLLOWING_PLAYER which is when the player is running and 
	 * it follows. It can be ATTACKING_ENEMY, which is when it can see an enemy and it attacks, or it can be dead.
	 */ 
	public enum BuddyState {
		IDLE,
		FOLLOWING_PLAYER,
		ATTACKING_ENEMY,
		DEAD
	};
	
	public BuddyState state; //reference to the enum above
	
	// Use this for initialization
	void Start () {
		player = GameObject.Find("First Person Controller");
		agent = GetComponent<NavMeshAgent>();
		state = BuddyState.FOLLOWING_PLAYER;
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		//At all times I keep a reference to the distance from the player.
		distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

		/*
		 * Here I search for all enemies in the level. I then draw a raycast to each enemy. This way, I now have a reference 
		 * to every enemies position in relation to the dinosaur. I can now always record the distance to each enemy, and
		 * whether or not the raycast is hitting them.
		 */ 
		foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
		{
			Vector3 lineOfSightToEnemy = enemy.transform.position - transform.position;
			Vector3 raycastStartPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
			
			if(Physics.Raycast(raycastStartPosition, lineOfSightToEnemy, out hit))
			{
				/*
				 * Here I check to see if the raycast has hit an enemy, and if it has, I then check the distance to
				 * that enemy. If the enemy is in range, then the dinosaur goes and attacks.
				 */ 
				if(hit.collider.gameObject.tag == "enemy")
				{
					if(distanceToEnemy < 10)
					{
						currentEnemy = enemy;
						state = BuddyState.ATTACKING_ENEMY;
					}
				}
				/*
				 * I draw this ray to each enemy for debugging purposes
				 */ 
				Debug.DrawRay(raycastStartPosition, lineOfSightToEnemy, Color.red);
			}
		}

		if(attackingAnEnemy)
		{
			anim.SetBool("AttackEnemy", true);
		}

		if(!attackingAnEnemy)
		{
			anim.SetBool("AttackEnemy", false);
		}
		
		/*
		 * This state machine controls the dinosaurs states.
		 */ 
		switch(state) {

			/*
			 * If the dinosaur is IDLE, I check the distance to the player. If the player is more than 4 metres away,
			 * the dino will change state and follow the player. If the dino is less than 4 metres from the player, then
			 * he will stand idle. To give the dinosaue some personality, I make it play an animation if the player
			 * stands still for too long
			 */ 
		case BuddyState.IDLE:
			anim.SetFloat("distanceToPlayer", distanceToPlayer); //Here I pass the distance to the player to the animator
			attackingAnEnemy = false;

			idleTimer += Time.deltaTime;
			
			if(idleTimer >=10.0f)
			{
				anim.SetTrigger("complain");
				idleTimer = 0.0f;
			}
			
			if(distanceToPlayer > 4.0f)
			{
				state = BuddyState.FOLLOWING_PLAYER;
			}
			
			if(distanceToPlayer <= 4.0f && state != BuddyState.ATTACKING_ENEMY)
			{
				state = BuddyState.IDLE;
			}
			
			break;

			/*
			 * When FOLLOWING_PLAYER, the dinosaur plays the run animation, and its target position is set to the players
			 * position. To stop the dinosaur actually standing directly on the player, I applied a dampner to the 
			 * NAVMESH component, so it actually stops slighty away from the player.
			 */ 
		case BuddyState.FOLLOWING_PLAYER:
			anim.SetFloat("distanceToPlayer", distanceToPlayer);
			attackingAnEnemy = false;

			if(distanceToPlayer <=4)
			{
				state = BuddyState.IDLE;
			}

			agent.SetDestination(player.transform.position);
			break;

			/*
			 * When ATTACKING_ENEMY, the dino sets its target position to be the position of the enemy. Once it is 
			 * in range of the dinosaur, this triggers the "attackingEnemy" animation, and I send damge to the enemy
			 * via the SendMessage feature. When the enemy is destroyed, it will return NULL, and I then set the state
			 * of the dinosaur to IDLE. Once it goes to IDLE, it will straight away check whether it is near the player or 
			 * not, in which case it will either run after the player, or stand beside the player if the player is in 
			 * range.
			 */ 
		case BuddyState.ATTACKING_ENEMY:

			if(currentEnemy != null)
			{
				agent.SetDestination(currentEnemy.transform.position);

				if(Vector3.Distance(transform.position, currentEnemy.transform.position) <= 4)
				{
					if(currentEnemy == null || currentEnemy.Equals(null))
					{
						//state = BuddyState.IDLE;
					//	Debug.Log("null enemy");
					}
					else{
						agent.SetDestination(currentEnemy.transform.position);
						transform.LookAt(currentEnemy.transform.position);
						currentEnemy.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
						//anim.SetTrigger("attackingEnemy");
						attackingAnEnemy = true;
					}
				}
				else {
					state = BuddyState.IDLE;
				}
			}
			else{
				state = BuddyState.IDLE;
			}

			break;

			/*
			 * If buddy gets killed, I stop him moving, and then play the death animation. When this state is triggered,
			 * it calls the DIE() function below
			 */ 
		case BuddyState.DEAD:
			agent.SetDestination(transform.position);
			collider.enabled = false;
			anim.SetTrigger("Die");
			StartCoroutine("Die");
			break;
		}
	}

	/*
	 * Here I wait 3 seconds (which allows the death animation to complete) Then I call display the YOU LOSE gui text
	 * and load the main menu screen as the game is over and the player has lost.
	 */ 
	IEnumerator Die()
	{
		yield return new WaitForSeconds(3.0f);
		/*
		 * Note: On level 3, I do not end the game when buddy dies as the player has multiple dinosaurs helping them
		 * on that particular level
		 */ 
		if(Application.loadedLevelName == "Level3")
		{
			Destroy(gameObject);
		}
		else{
			GameObject.FindGameObjectWithTag("YouLoseGUI").GetComponent<GUIText>().enabled = true;
			collider.enabled = false;
			Application.LoadLevel("MainMenu");
		}
	}
}
