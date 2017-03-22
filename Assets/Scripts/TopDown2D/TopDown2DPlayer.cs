using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDown2DPlayer : MonoBehaviour {

	/*
	====================================================================================================
	VARIABLES
	====================================================================================================
	*/

	/*
	<<<COMPONENTS>>>
	*/

	SpriteRenderer spriteRenderer;
	Animator animator;

	/*
	<<<MOVEMENT>>>
	*/

	/*
	Polar Directions:
	Polar angles laid out like E=0 - 45 - N=90 - 135 - W=180 - 225 - S=270
	*/
	public enum Direction {
		EAST,
		NORTHEAST,
		NORTH,
		NORTHWEST,
		WEST,
		SOUTHWEST,
		SOUTH,
		SOUTHEAST
	}

	[System.Serializable]
	public class Movement {
	
		public Direction direction = Direction.EAST;
		public float speed;

	}
	
	public Movement movement;
	
	/*
	<<<INPUT>>>
	*/

	/*
	Mouse for direction. Keys for movement. Potential joystick support is easily implemented.
	*/
	
	[System.Serializable]
	public class KeyboardInput {
		public Vector2 mousePos;
		public Vector2 moveAxes;
	}

	public KeyboardInput input;
	Camera cam;

	/*
	====================================================================================================
	METHODS
	====================================================================================================
	*/

	void Start() {
		cam = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
	}

	void Update() {
		UpdateInput ();
		Move ();
		Rotate ();
		spriteRenderer.color = ColorFromDirection (movement.direction);

	}

	/*
	UPDATE INPUT
	Update the mouse and movement inputs
	*/
	void UpdateInput() {
		/*I’m using 4-Quadrant vector here rather than Quadrant 1 because it helps with the trigonometry*/
		input.mousePos = cam.ScreenToWorldPoint(new Vector2(
			Input.mousePosition.x,
			Input.mousePosition.y
		));

		input.moveAxes = new Vector2 (
			Input.GetAxisRaw ("Horizontal"),
			Input.GetAxisRaw ("Vertical")
		);
	}
		
	/*
	MOVE
	Move the player based on axis input
	*/
	void Move() {
		transform.position += new Vector3(input.moveAxes.x, input.moveAxes.y, 0.0f) * movement.speed * Time.deltaTime;
	}

	/*
	ROTATE
	Rotate the player based on mouse position
	*/
	void Rotate() {

		Vector2 relativePoint = new Vector2 (transform.position.x - input.mousePos.x, transform.position.y - input.mousePos.y);

		float angle = (180.0f + Mathf.Atan2(relativePoint.y, relativePoint.x) * Mathf.Rad2Deg);

		float cardinal = (int) Mathf.Round(angle / 45.0f);

		cardinal = cardinal % 8;

		movement.direction = Cardinal2Direction((int) cardinal);


		Animate ((int) cardinal);

	}

	/*
	ANIMATE
	Set animator values
	*/
	void Animate(float cardinal) {
		animator.SetFloat ("cardinal", cardinal);
	}

	Direction Cardinal2Direction(int card) {
		switch (card) {
		default:
		case 0:
			return Direction.EAST;
		case 1:
			return Direction.NORTHEAST;
		case 2:
			return Direction.NORTH;
		case 3:
			return Direction.NORTHWEST;
		case 4:
			return Direction.WEST;
		case 5:
			return Direction.SOUTHWEST;
		case 6:
			return Direction.SOUTH;
		case 7:
			return Direction.SOUTHEAST;
		}
	}

	/*
	COLOR FROM DIRECTION
	Quick function to test the directions
	*/
	Color ColorFromDirection(Direction direction) {

		switch (direction) {

		default:
		case Direction.EAST:
			return Color.red;
		case Direction.NORTHEAST:
			return Color.yellow;
		case Direction.NORTH:
			return Color.green;
		case Direction.NORTHWEST:
			return Color.blue;
		case Direction.WEST:
			return Color.magenta;
		case Direction.SOUTHWEST:
			return Color.cyan;
		case Direction.SOUTH:
			return Color.gray;
		case Direction.SOUTHEAST:
			return Color.white;

		}

	}

}
