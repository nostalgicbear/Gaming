using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is placed on the First Person Controller. This script relates to the controls for the game.
 */ 

public class PlayerControlScript : MonoBehaviour {
	public bool holdingHealth = false; //a bool that determines whether the player is currently holding a healthpack
	private float distanceToBuddy;
	private GameObject buddy;
	private GameObject healthScript;
	private float healthIncrease = 50.0f; //the amount to increase health by when a healthpack is used
	private GameObject buddyView;
	private List<GameObject> weapons = new List<GameObject>(); //A list of all weapons in the level
	public bool holdingWeapon; //a bool that determines whether the player is holding a weapon
	public GameObject weaponPrefab; //The weapon prefab
	private bool paused = false; //A bool that determines whether or not the game is paused
	private GameObject pasuedGUI; //A guitext object that is displayed when the game is paused

	// Use this for initialization
	void Start () {
		buddy = GameObject.FindGameObjectWithTag("buddy");
		healthScript = GameObject.FindGameObjectWithTag("GUI_Controller");
		buddyView = GameObject.FindGameObjectWithTag("buddyView");
		pasuedGUI = GameObject.FindGameObjectWithTag("paused");

		foreach(GameObject weapon in GameObject.FindGameObjectsWithTag("weapon"))
		{
			weapons.Add(weapon);
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * If the game is pasued, I set the timescale to 0. This essentially freezes everything in the game until the 
		 * pause button is pressed again, unpausing the game
		 */ 
		if(paused)
		{
			Time.timeScale = 0.0f;
			pasuedGUI.guiText.enabled = true;
		}
		else{
			Time.timeScale = 1.0f;
			pasuedGUI.guiText.enabled = false;
		}

		if(Input.GetKeyUp(KeyCode.P))
		{
			paused = !paused;
		}

		/*
		 * This enables and disables the buddyview window. This is the window that lets you see the game world from
		 * your AI sidekicks point of view
		 */ 
		if(Input.GetKeyDown(KeyCode.H) || Input.GetButtonUp("xboxY"))
		{
			buddyView.guiTexture.enabled = !buddyView.guiTexture.enabled;
		}

		distanceToBuddy = Vector3.Distance(transform.position, buddy.transform.position);

		/*
		 * This bool determines whether or not you are currently holding a medpack. If true, I enable a mesh renderer
		 * on a gameobject that is positioned in the players hands. This way, when the player picks up a healthpack, it
		 * actually appears in their hands
		 */ 
		if(holdingHealth)
		{
			GameObject.FindGameObjectWithTag("HealthPack").GetComponent<MeshRenderer>().enabled = true;
		}

		/*
		 * When the player isnt currently holding a healthpack, the renderer is turned off
		 */ 
		else{
			GameObject.FindGameObjectWithTag("HealthPack").GetComponent<MeshRenderer>().enabled = false;
		}

		/*
		 * Uses the healthpack on the player. The players health increases, and I then set "holdingHealth" to false,
		 * making the heathpack disapear from the players hands after it has been used
		 */ 
		if(Input.GetKeyUp(KeyCode.N) || Input.GetButtonUp("xboxX"))
		{
			healthScript.SendMessage("IncreasePlayerHealth", healthIncrease, SendMessageOptions.DontRequireReceiver);
			holdingHealth = false;
		}

		/*
		 * When in range of their AI buddy, the player can choose to give the healthpack to the buddy instead. This is
		 * an intrical part of my game. The AI buddy helps the player by fighting the enemies, and so the player can 
		 * heal the buddy by picking up healthpacks and healing the buddy
		 */ 
		if(distanceToBuddy <= 4)
		{
			if(holdingHealth)
			{
				if(Input.GetKeyUp(KeyCode.B) || Input.GetButtonUp("xboxB"))
				{
					healthScript.SendMessage("IncreaseBuddyHealth", healthIncrease, SendMessageOptions.DontRequireReceiver);
					holdingHealth = false;
				}
			}
		}

		/*
		 * The player can pick up weapons in the game. When they do, they can through the weapon to attack enemies.
		 * When the player throws a spear, I instantiate a spear at the exact location of the one that appears in the
		 * players hand. I then add a force to it and throw it the direction the player is facing
		 */ 
		if(holdingWeapon)
		{
			if(Input.GetKeyUp(KeyCode.M) || Input.GetButtonUp("xboxRB"))
			{
				GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>().holdingWeapon = false;
				GameObject.FindGameObjectWithTag("weaponPosition").GetComponent<MeshRenderer>().enabled = false;
				Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.forward;
				direction.y += 5;

				GameObject weaponInstance = Instantiate(weaponPrefab, GameObject.FindGameObjectWithTag("weaponPosition").GetComponent<Transform>().position, GameObject.FindGameObjectWithTag("weaponPosition").GetComponent<Transform>().rotation) as GameObject;
				weaponInstance.rigidbody.AddForce(transform.forward * 800.0f);
			}
		}
	}
}
