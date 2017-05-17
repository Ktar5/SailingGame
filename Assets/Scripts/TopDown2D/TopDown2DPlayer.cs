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
	Rigidbody2D rb2D;
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
		public float weaponWeight = 300.0f;
		public float speed = 2.0f;

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
		public bool shieldUp;
		public bool aiming;
	}

	public KeyboardInput input;

	[System.Serializable]
	public class GUIControls
	{
		public bool showUI = true;
		public Texture2D letterbox;
		[Range(3.0f,8.0f)]
		public float range = 5.0f;
		[Range(0.0f, 1.0f)]
		public float cutoff = 1.0f;
	}

	public GUIControls guiControls;

	Camera cam;

	/*
	<<<COMBAT>>>
	*/



	/*
	====================================================================================================
	METHODS
	====================================================================================================
	*/

	void Start() {
		cam = Camera.main;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		rb2D = GetComponent<Rigidbody2D> ();
		InitGUILetterbox ();
		InitPhysics ();

	}

	void InitPhysics() {

		//rigidbody.drag = 0.03f * movement.weaponWeight;

	}

	void InitGUILetterbox() {
		guiControls.letterbox = new Texture2D (1, 1);
		guiControls.letterbox.SetPixel (0, 0, new Color(0.0f,0.0f,0.0f,0.5f));
		guiControls.letterbox.Apply ();

	}

	void Update() {
		UpdateInput ();
		Rotate ();
		Attack ();
		Animate ();
	}

	void FixedUpdate() {
		Move ();

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

		input.aiming = Input.GetButton ("Fire2");
		input.shieldUp = Input.GetButton ("Action1");
	}

	void Attack() {
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 jerk = (new Vector3(input.mousePos.x, input.mousePos.y, 0.0f) - (transform.position)).normalized * movement.weaponWeight;
			rb2D.AddForce (jerk);
		}
	}
		
	/*
	MOVE
	Move the player based on axis input
	*/
	void Move() {
		transform.position += new Vector3(input.moveAxes.x, input.moveAxes.y, 0.0f) * movement.speed * 0.02f;
	}

	/*
	ROTATE
	Rotate the player based on mouse position
	*/
	void Rotate() {

		if (input.aiming || Input.GetButtonDown("Fire1")) {
			Vector2 relativePoint = new Vector2 (transform.position.x - input.mousePos.x, transform.position.y - input.mousePos.y);
			float angle = (180.0f + Mathf.Atan2 (relativePoint.y, relativePoint.x) * Mathf.Rad2Deg);
			float cardinal = (int)Mathf.Round (angle / 45.0f);
			cardinal = cardinal % 8;
			movement.direction = Cardinal2Direction ((int)cardinal);
		} else {
			movement.direction = DirectionFromAxis();
		}
	}

	/*
	ANIMATE
	Set animator values
	*/
	void Animate() {
		animator.SetFloat ("cardinal", Direction2Cardinal(movement.direction));
		spriteRenderer.color = (input.shieldUp ? Color.blue : Color.white);
	}


	/*
	CARDINAL TO DIRECTION
	Convert a cardinal integer from 0-8
	*/
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
	DIRECTION FROM AXIS
	Get a direction from a keypress axis
	*/

	Direction DirectionFromAxis() {
		if (input.moveAxes == Vector2.right)
			return Direction.EAST;
		else if (input.moveAxes == -1 * Vector2.right)
			return Direction.WEST;
		else if (input.moveAxes == Vector2.up)
			return Direction.NORTH;
		else if (input.moveAxes == -1 * Vector2.up)
			return Direction.SOUTH;
		else if (input.moveAxes == Vector2.up + Vector2.right)
			return Direction.NORTHEAST;
		else if (input.moveAxes == (-1 * Vector2.up) + Vector2.right)
			return Direction.SOUTHEAST;
		else if (input.moveAxes == Vector2.up + (-1 * Vector2.right))
			return Direction.NORTHWEST;
		else if (input.moveAxes == (-1 * Vector2.up) + (-1 * Vector2.right))
			return Direction.SOUTHWEST;
		else {
			return movement.direction;
		}
	}

	int Direction2Cardinal(Direction direction) {

		switch (direction) {

			default:
			case Direction.EAST:
			return 0;
			case Direction.NORTHEAST:
			return 1;
			case Direction.NORTH:
			return 2;
			case Direction.NORTHWEST:
			return 3;
			case Direction.WEST:
			return 4;
			case Direction.SOUTHWEST:
			return 5;
			case Direction.SOUTH:
			return 6;
			case Direction.SOUTHEAST:
			return 7;

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

	void OnGUI() {


		guiControls.cutoff = Mathf.Lerp(guiControls.cutoff, (input.aiming ? 1 : 0), 5.0f * Time.deltaTime);

		//SHIELD LETTERBOXING
		Rect rect = new Rect(0,0,Screen.width, (Screen.height/guiControls.range) * guiControls.cutoff);
		GUI.DrawTexture (rect, guiControls.letterbox);
		rect.Set(0,Screen.height - (Screen.height/guiControls.range) * guiControls.cutoff, Screen.width, (Screen.height/guiControls.range) * guiControls.cutoff);
		GUI.DrawTexture (rect, guiControls.letterbox);


		if (guiControls.showUI) {

			GUI.Label (new Rect (32, 32, 128, 32), input.shieldUp? "SHIELD UP" : "SHIELD DOWN");

		}

	}

}
