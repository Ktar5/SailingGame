using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

BOAT CONTROLLER

Attach this to boat object. 
Accelerate and decelerate with Input.Vertical axis (default W/S or Up Arrow/Down Arrow).
Turn with Input.Horizontal axis (default A/D or Left Arrow/Right Arrow).

Wind has an angle from -pi to pi and a force. Set force to 0 for no wind. Wind will continually push the boat in some direction.
If the boat is stationary, then wind will not effect it. Wind only effects boat when boat is moving. If the boat moves with the
wind, it will speed up, or if the boat moves against the wind, it will slow down.

If you check 'Visualize Forces' under Wind category, then you'll notice a red line and a black line. The black line is the
wind force and the red line is the velocity of the ship. When going against the wind, the red line is shorter than when going with
the wind (the magnitude of this phenomenon is relative to the wind force).

*/
public class BoatController : MonoBehaviour {
	/*
	MOVEMENT
	Variable organizer for movement values.
	*/
	[System.Serializable]
	public class Movement {

		public float maxSpeed = 500f;
		public float turnSpeed = 0.6f;
		public float acceleration = 5.5f;
		public float waterFriction = 5f;
	    public float sailSpeed = 1f;

	}

    [System.Serializable]
    public class Sail {
        [Range(0, 3.14f)]
        public float angle;
        [Range(0,100)]
        public float strength = 0.0f;
    }

	/*
	WIND
	Variable organizer for wind values.
	*/
	[System.Serializable]
	public class Wind {

		[Range(-3.14f,3.14f)]
		public float angle = 0.0f;
		public float strength = 0.0f;
		public bool visualizeForces = true;

	}


	public Movement movement;
    public Sail sail;
	public Wind wind;

	// Private vars for movement.
	private float speed;
	private float turnInput;

	//COMPONENTS
	Rigidbody2D rigidbody;

	//INPUT
	Vector2 input;

	void Start() {
		//Init COMPONENTS
		rigidbody = GetComponent<Rigidbody2D> ();
	}


	void Update() {

		GetInput ();
		MovementPhysics ();
		TurnPhysics ();
		CalcVelocity ();


	}

	/*
	TURN PHYSICS
	Turn the ship based on turn speed and input.
	*/
	void TurnPhysics() {
		//Get a LERP'ed input
		turnInput = Mathf.Lerp (turnInput, input.x, 1.0f * Time.deltaTime);
		//Calculate an appropriate amount to turn
		float turnAmount = turnInput * movement.turnSpeed * Time.deltaTime * rigidbody.velocity.sqrMagnitude;
		//Turn the ship! Using transform.euler angles rather than Rigidbody in order to preserve velocity. Not entirely realistic,
		//but a fuckton easier to control.
        //We have to subtract because clockwise is -angle around z axis
		transform.Rotate(new Vector3(0, 0, transform.rotation.z - turnAmount));
    }

	/*
	MOVEMENT PHYSICS
	Modify the 'speed' float variable used in velocity calculations based on input, acceleration, and water friction.
	*/
	void MovementPhysics() {
		//Add acceleration to speed if we are moving forward.
		speed += input.y * movement.acceleration;
		//Subtract water friction from speed.
		speed -= movement.waterFriction;
		//Don't let speed go negative.
		speed = Mathf.Clamp (speed, 0.0f, movement.maxSpeed);
	}


	/*
	CALC VELOCITY
	Based on factors like speed and wind force, calculate the resultant velocity of the ship.
	*/
	void CalcVelocity() {

		//Init velocity and windforce vars
		Vector3 velocity = transform.up * speed * Time.deltaTime;
		Vector3 windForce = new Vector3 (
			                    Mathf.Sin (wind.angle) * wind.strength * Mathf.Clamp01 (rigidbody.velocity.sqrMagnitude), 0.0f, 
			                    Mathf.Cos (wind.angle) * wind.strength * Mathf.Clamp01 (rigidbody.velocity.sqrMagnitude)
		                    );

		//Add windForce to velocity
		velocity += windForce;

		//Draw the two vectors IF visualizeForces is checked
		if (wind.visualizeForces) {
			Debug.DrawLine (transform.position, transform.position + windForce, Color.blue);
			Debug.DrawLine (transform.position, transform.position + velocity, Color.red);
		}

		//Finally, apply velocity
		rigidbody.velocity = velocity;
	}

	/*
	GET INPUT
	Gather input data from axes/buttons.
	*/
	void GetInput() {
		input.x = Input.GetAxisRaw ("Horizontal");
		input.y = Input.GetAxisRaw ("Vertical");
        print(input.x);
	}

}
