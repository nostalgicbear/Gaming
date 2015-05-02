using UnityEngine;
using System.Collections;

public class WeaponPickUp : MonoBehaviour {
	private GameObject weaponInHands;
	private GameObject playersHands;

	// Use this for initialization
	void Start () {
		playersHands = GameObject.FindGameObjectWithTag("HealthPack");
		weaponInHands = GameObject.FindGameObjectWithTag("weaponPosition");
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{

				Destroy(gameObject);
				weaponInHands.renderer.enabled = true;
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControlScript>().holdingWeapon = true;

		}
	}
}
