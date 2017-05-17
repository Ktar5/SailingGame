using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

TOP DOWN 2D MOVEMENT

This script enables a 2D object to move according to input with
full functionality including smoothing. Follow steps below.

steps:
1 Create a 2D object with some 2D collider (CircleCollider2D, BoxCollider2D, etc.). This is your player.
2 Attach this script to the player object as a component. A Rigidbody2D with correct values will automatically be produced.
3 Create some test 2D obstacle objects in the scene that have box colliders.
4 Move with WASD or left stick on gamepad.
5 Adjust speed/smoothing under movement in the component options.

*/
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class TopDown2DMovement : MonoBehaviour {

	/*
	MOVEMENT
	All variables associated with movement and physics.
	*/
	[System.Serializable]
	public class Movement {
		
		public float speed = 7f; /*speed of the player*/
		public float smoothSpeed = 12f; /*smoothing velocity of the player. Higher values mean less smoothing.*/
		[HideInInspector]
		public Vector2 direction; /*direction of the player. Normalized to input.*/
		[HideInInspector]
		public Vector3 displacement; /*final displacement vector of player. Used in transform calculation.*/
		[HideInInspector]
		public Rigidbody2D body; /*attached rigidbody component*/
	}

	public Movement movement; /*Movement object.*/

	/*
	START
	Run first frame.
	*/
	void Start() {
		InitRigidBody ();
	}

	/*
	INIT RIGIDBODY
	Initialize some values for the Rigidbody object.
	*/
	void InitRigidBody() {
		movement.body = GetComponent<Rigidbody2D> ();
		movement.body.isKinematic = false;
		movement.body.gravityScale = 0.0f;
		movement.body.freezeRotation = true;
	}




	/*
	MOVE
	Main movement function. Moves the player according to input.
	*/
	public void Move() {
		movement.direction = new Vector2 (
			Input.GetAxisRaw ("Horizontal"),
			Input.GetAxisRaw ("Vertical")).normalized; /*Set direction to the normalized input vector.*/

		movement.displacement = 
			Vector3.Lerp(
				movement.displacement, 
				movement.direction * movement.speed * Time.fixedDeltaTime, 
				Time.fixedDeltaTime * movement.smoothSpeed); /*Set displacement accordingly. LERP for smoothing.*/

		movement.body.MovePosition(transform.position + movement.displacement); /*Apply the displacement to the rigidbody.*/
	}

	/*
	FIXED UPDATE
	Normalized physics update function.
	*/
	public void FixedUpdate() {
		Move();
	}

}
