using UnityEngine;
using System.Collections;

/*
 * This script is on the GUI_Controller gameobject in all levels. This script controls the health bars for both the player
 * and the players buddy. These health bars are located in the bottom left corner of the screen.
 */ 

public class HealthManager : MonoBehaviour {
	private float maxHealth = 100.0f;
	private float playerCurrentHealth; 
	private float buddyCurrentHealth;
	public GUITexture playerHealth; //The red health bar texture representing the players health
	public GUITexture buddyHealth; //The red health bar texture representing the buddys health


	// Use this for initialization
	void Start () {
		/*
		 * Here I set the players health to be equal to the width of the guitextures that represent their health. So 
		 * the bars start at full width, and the player and buddy start at full heatlh. 
		 */ 
		playerCurrentHealth = playerHealth.guiTexture.pixelInset.width; 
		buddyCurrentHealth = buddyHealth.guiTexture.pixelInset.width;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * If the player or buddy receive a heatlh pack, there is a chance that their health will go over the max health
		 * so I just make sure that doesnt happen in the lines below
		 */ 
		if(playerCurrentHealth > maxHealth)
		{
			playerCurrentHealth = maxHealth;
		}

		if(buddyCurrentHealth > maxHealth)
		{
			buddyCurrentHealth = maxHealth;
		}

		/*
		 * Health is based off a gui texture width, and so when the guiTexture reaches -133.0, that is the equivalent
		 * of the players health bar being 0. 
		 */ 
		if(buddyCurrentHealth <= -133.0f)
		{
			GameObject.FindGameObjectWithTag("buddy").GetComponent<BuddyScript>().state = BuddyScript.BuddyState.DEAD;
			buddyCurrentHealth = -133.0f;
		}

		if(playerCurrentHealth <= -133.0f)
		{
			GameObject.FindGameObjectWithTag("YouLoseGUI").GetComponent<GUIText>().enabled = true;
			StartCoroutine("EndGame");
			playerCurrentHealth = -133.0f;
		}

		/*
		 * Using C# I need to update the player and buddys health by storing their current health in a temporary variable
		 * before modifying it. Thats what Im doing below.
		 */
		Rect playersHealthTemp = playerHealth.guiTexture.pixelInset;
		playersHealthTemp.width = playerCurrentHealth;
		playerHealth.pixelInset = playersHealthTemp;

		Rect buddysHealthTemp = buddyHealth.guiTexture.pixelInset;
		buddysHealthTemp.width = buddyCurrentHealth;
		buddyHealth.pixelInset = buddysHealthTemp;
	}

	/*
	 * This is called if the game ends. It just loads the main menu after 3 seconds
	 */ 
	IEnumerator EndGame()
	{
		yield return new WaitForSeconds(3.0f);
		Application.LoadLevel("MainMenu");
	}
	
	/*
	 * This is called when the player collects a health pack. It increases their health
	 */ 
	void IncreasePlayerHealth(float amount)
	{
		playerCurrentHealth += amount;
	}
	/*
	 * This is called when the player gives a health pack to their dinosaur buddy. It increases its health
	 */ 
	void IncreaseBuddyHealth(float amount)
	{
		buddyCurrentHealth += amount;
	}

	/*
	 * This is called when an enemy is attacking the player. It reduces the players health
	 */ 
	void PlayerTakeDamage(float damage)
	{
		playerCurrentHealth -= damage;
		
	}

	/*
	 * This is called when the players buddy takes damage. It reduces his health
	 */ 
	void BuddyTakeDamage(float damage)
	{
		buddyCurrentHealth -= damage;
	}
}
