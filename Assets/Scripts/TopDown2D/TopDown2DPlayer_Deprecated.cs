using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDown2DPlayer_Deprecated : MonoBehaviour {

	/*
	====================================================================================================
	MOVEMENT
	====================================================================================================
	*/

	/*
	Polar Directions:
	Polar angles laid out like E=0, N=90, W=180, S=270
	*/
	public enum Direction {EAST, NORTH, WEST, SOUTH};
	public Direction direction = Direction.EAST;

	/*
	Player States:
	Change the state from STANDARD to CUTSCENE in order to manually change the player direction
	*/

	public enum PlayerState {STANDARD, CUTSCENE};
	public PlayerState playerState = PlayerState.STANDARD;

	/*
	Input:
	Mouse for direction. Keys for movement. Potential joystick support is easily implemented.
	*/
	Vector2 mousePos;
	Vector2 moveAxes;

	/*
	Movement:
	Statistics for movement.
	*/

	public float speed;

	/*
	====================================================================================================
	ANIMATION
	====================================================================================================
	*/
	private Animator animator;

	/*
	Start:
	ran first frame
	*/

	void Start() {
		animator = GetComponent<Animator> ();
	}

	/*
	Update:
	Ran every frame
	*/
	void Update() {
		UpdateInput();
		Move();
		Rotate();
	}

	void UpdateAnimator() {

		animator.SetBool ("facing_RIGHT" , direction == Direction.WEST  );
		animator.SetBool ("facing_UP"    , direction == Direction.NORTH );
		animator.SetBool ("facing_LEFT"  , direction == Direction.EAST  );
		animator.SetBool ("facing_SOUTH" , direction == Direction.SOUTH );
		animator.SetBool ("moving", moveAxes.sqrMagnitude > 0.0f);
	}

	/*
	UpdateInput:
	Update the mouse and movement inputs
	*/
	void UpdateInput() {
		/*I’m using 4-Quadrant vector here rather than Quadrant 1 because it helps with the trigonometry*/
		mousePos = new Vector2(
			Input.mousePosition.x - Screen.width/2,
			Input.mousePosition.y - Screen.height/2
		);

		moveAxes = new Vector2 (
			Input.GetAxisRaw ("Horizontal"),
			Input.GetAxisRaw ("Vertical")
		);
	}

	/*
	Move:
	Move the player based on axis input
	*/
	void Move() {
		transform.position += new Vector3(moveAxes.x, moveAxes.y, 0.0f) * speed * Time.deltaTime;
	}

	/*
	Rotate:
	Rotate the player based on mouse position
	*/
	void Rotate() {
		float angle = (180.0f + Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg);
		direction = Angle2Direction(angle);

	}

	/*
	Angle2Direction:
	Return an enumerated direction based on a degree-angle.
	@param angle angle (in degrees) to input
	@return the enumerated direction
	*/
	Direction Angle2Direction(float angle) {

		int temp = (int) Mathf.Round(angle / 90.0f);
		temp = (temp >= 4? 0 : temp);
		temp = (temp < 0 ? 0 : temp);
		switch(temp) {
		default:
		case 0:
			return Direction.SOUTH;
		case 1:
			return Direction.EAST;
		case 2:
			return Direction.NORTH;
		case 3:
			return Direction.WEST;
		}

	}

}
