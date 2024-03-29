﻿using UnityEngine;
using System.Collections;

public class SwarmUnitBehaviour : MonoBehaviour {
	private GameObject Controller;
	private bool inited = false;
	private float minVelocity;
	private float maxVelocity;
	private float randomness;
	private GameObject chasee;
	private Rigidbody rigidbody;

	void Start ()
	{
		rigidbody = GetComponent<Rigidbody> ();
		StartCoroutine ("BoidSteering");
	}

	IEnumerator BoidSteering ()
	{
		while (true)
		{
			if (inited)
			{
				rigidbody.velocity = rigidbody.velocity + Calc () * Time.deltaTime;

				// enforce minimum and maximum speeds for the boids
				float speed = rigidbody.velocity.magnitude;
				if (speed > maxVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * maxVelocity;
				}
				else if (speed < minVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * minVelocity;
				}
			}

			float waitTime = Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds (waitTime);
		}
	}

	private Vector3 Calc ()
	{
		Vector3 randomize = new Vector3 ((Random.value *2) -1, (Random.value * 2) -1, (Random.value * 2) -1);

		randomize.Normalize();
		SwarmController swarmController = Controller.GetComponent<SwarmController>();
		Vector3 flockCenter = swarmController.flockCenter;
		Vector3 flockVelocity = swarmController.flockVelocity;
		Vector3 follow = chasee.transform.localPosition;

		flockCenter = flockCenter - transform.localPosition;
		flockVelocity = flockVelocity - rigidbody.velocity;
		follow = follow - transform.localPosition;

		return (flockCenter + flockVelocity + follow * 2 + randomize * randomness);
	}

	public void SetController (GameObject theController)
	{
		Controller = theController;
		SwarmController swarmController = Controller.GetComponent<SwarmController>();
		minVelocity = swarmController.minVelocity;
		maxVelocity = swarmController.maxVelocity;
		randomness = swarmController.randomness;
		chasee = swarmController.chasee;
		inited = true;
	}
}