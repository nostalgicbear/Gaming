using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is attached to the GUI_Controller gameobject in every level. It controls the egg textures on screen. These
 * textures indicate to the player how many eggs are left to be found.
 */ 

public class EggTextureScript : MonoBehaviour {
	private Color newEggColour; //The color to display the egg once its been found.
	public int eggsRemaining; 
	private float eggTextureFound = 1.0f; //This refers to the alpha value of the texture. If set to a low value, the texture will be transparent
	private GameObject youWinGUI; //This gui is displayed when the player wins the level. (I.e when they collect all eggs)
	private List<GameObject> eggTextures = new List<GameObject>(); //List of all the egg textures

	/*
	 * This enum allows me to control the appearence of the egg textures based on how many eggs have been found.
	 */ 
	private enum Eggs_Remaining {
		FIVE_REMAINING,
		FOUR_REMAINING,
		THREE_REMAINING,
		TWO_REMAINING,
		ONE_REMAINING,
		NONE_REMAINING
	};

	private Eggs_Remaining state;

	// Use this for initialization
	void Start () {
		eggsRemaining = 5;
		state = Eggs_Remaining.FIVE_REMAINING;

		foreach(GameObject eggTexture in GameObject.FindGameObjectsWithTag("eggTexture"))
		{
			eggTextures.Add(eggTexture);
		}

		youWinGUI = GameObject.FindGameObjectWithTag("YouWinGUI");
	
	}
	
	// Update is called once per frame
	void Update () {

		/*
		 * Depending on the amount of eggs found, I alter the appearence of the GUI textures on screen. When an egg is not
		 * found, its texture is transparent so its clear that it hasnt been found. When it is found, I set it to be 
		 * NOT transparent so its clear it has been found. 
		 */ 
		switch(state) {
		case Eggs_Remaining.FIVE_REMAINING:
			foreach(GameObject eggTexture in eggTextures)
			{
				newEggColour = eggTexture.guiTexture.color;
				newEggColour.a = 0.1f;
				eggTexture.guiTexture.color = newEggColour;

				if(eggsRemaining == 4)
				{
					state = Eggs_Remaining.FOUR_REMAINING;
				}
			}
			break;
			
		case Eggs_Remaining.FOUR_REMAINING:

			newEggColour = eggTextures[0].guiTexture.color;
			newEggColour.a = 1.0f;
			eggTextures[0].guiTexture.color = newEggColour;

			if(eggsRemaining == 3)
			{
				state = Eggs_Remaining.THREE_REMAINING;
			}

			break;
			
		case Eggs_Remaining.THREE_REMAINING:
			newEggColour = eggTextures[0].guiTexture.color;
			newEggColour.a = 1.0f;

			eggTextures[0].guiTexture.color = newEggColour;
			eggTextures[1].guiTexture.color = newEggColour;

			if(eggsRemaining == 2)
			{
				state = Eggs_Remaining.TWO_REMAINING;
			}
		
			break;
			
		case Eggs_Remaining.TWO_REMAINING:
			newEggColour = eggTextures[0].guiTexture.color;
			newEggColour.a = 1.0f;
			
			eggTextures[0].guiTexture.color = newEggColour;
			eggTextures[1].guiTexture.color = newEggColour;
			eggTextures[2].guiTexture.color = newEggColour;
			
			if(eggsRemaining == 1)
			{
				state = Eggs_Remaining.ONE_REMAINING;
			}

			break;
			
		case Eggs_Remaining.ONE_REMAINING:
			newEggColour = eggTextures[0].guiTexture.color;
			newEggColour.a = 1.0f;
			
			eggTextures[0].guiTexture.color = newEggColour;
			eggTextures[1].guiTexture.color = newEggColour;
			eggTextures[2].guiTexture.color = newEggColour;
			eggTextures[3].guiTexture.color = newEggColour;
			
			if(eggsRemaining == 0)
			{
				state = Eggs_Remaining.NONE_REMAINING;
			}

			break;
			
		case Eggs_Remaining.NONE_REMAINING:
			foreach(GameObject eggTexture in eggTextures)
			{
				newEggColour = eggTexture.guiTexture.color;
				newEggColour.a = 1.0f;
				eggTexture.guiTexture.color = newEggColour;

			}

			StartCoroutine("WonGame");
			break;
		}
	}

	/*
	 * This is only called if all eggs have been found. I enable the GUI to tell the player that they have won, and
	 * I then load the next level in the build list.
	 */ 
	IEnumerator WonGame()
	{
		youWinGUI.guiText.enabled = true;
		yield return new WaitForSeconds(3.0f);
		int i = Application.loadedLevel; //stores the index of the current level. This is displaued under build options
		Application.LoadLevel(i + 1);
	}
}
