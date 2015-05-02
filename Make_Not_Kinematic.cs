using UnityEngine;
using System.Collections;

/*
 * This script is on the Cage_Make_Not_Kinematic gameobject that is a child object of all cages. Its main purpose is 
 * to change the value of isKinematic for any character trapped in the cage. When trapped in the cage, some enemies will
 * continue to walk, and if their rigidbodies are not changed from isKinematic= true to isKinematic = false when they
 * are in the cage, they would just be able to push the cage regardless of its mass or their mass. So to combat this, 
 * anytime an enemy or buddy is stuck in a cage, I set their isKinematic value to false so they dont push it and the cage
 * functions as you would expect. 
 */ 
public class Make_Not_Kinematic : MonoBehaviour {

	/*
	 * If this is triggered, that means that an enemy, or the dinosaur companion has gotten trapped in the cage.
	 * Some enemies may have a navmesh agent attached. If this is the case, they will be able to push the cage, even if it
	 * is a heavier mass than them. To combat this, when an enemy or the dionsair gets trapped in a cage, I set their 
	 * rigidbody to be NOT kinrmatic. This stops them being able to push the cage.
	 */ 
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "enemy" || other.gameObject.tag == "buddy")
		{
			if(other.gameObject.rigidbody != null)
			{
				other.gameObject.rigidbody.isKinematic = false;
			}
		}
	}

	/*
	 * If the enemy leaves the cage. I set their rigidbody isKinematic to TRUE as otherwise they would not be able to
	 * get up steps.
	 */ 
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.rigidbody != null)
		{
			other.gameObject.rigidbody.isKinematic = true;
		}
	}
}
