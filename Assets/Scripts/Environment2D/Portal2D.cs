using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal2D : MonoBehaviour {

	public Portal2D otherPortal;
	public TopDown2DPlayer.Direction facingDirection;
	public bool drawConnection = false;

	[Range(0.0f, 100.0f)]
	private float letterboxFillPercent = 0.0f;
	private Texture2D letterbox;
	private Rect letterboxRect;

	private bool playerInPortal = false;
	private bool fading = false;
	private bool transporting = false;
	private TopDown2DPlayer player;

	void Start() {
		InitGUILetterbox ();
		InitPlayer ();
	}

	void Update() {
		CheckInPortal ();
		Fade ();
		StartCoroutine (Transport());

	}
	void OnDrawGizmos() {

		if (drawConnection) {
			otherPortal.drawConnection = false;
			Gizmos.color = Color.green;
			Gizmos.DrawLine (transform.position, otherPortal.transform.position);

		}

	}

	void InitPlayer() {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<TopDown2DPlayer> ();
	}


	void CheckInPortal() {
		if (playerInPortal && !fading && !transporting) {

			if (Input.GetKeyDown (KeyCode.E)) {

				fading = true;

			}

		}
	}

	void Fade() {


		letterboxFillPercent = Mathf.Lerp (letterboxFillPercent,(fading ? 100.0f : 0.0f), Time.deltaTime * 10.0f);

		if (fading && letterboxFillPercent > 99.9f) {

			transporting = true;

		}

	}

	IEnumerator Transport() {
		if (transporting && player != null) {
			player.transform.position = otherPortal.transform.position + (1.0f * PortalOffsetFromDirection (otherPortal.facingDirection));
			player.movement.direction = otherPortal.facingDirection;
			yield return new WaitForSeconds(1.0f);
			fading = false;
			transporting = false;
		}

	}

	void InitGUILetterbox() {
		letterboxRect = new Rect (0, 0, 0, Screen.height);
		letterbox = new Texture2D (1, 1);
		letterbox.SetPixel (0, 0, Color.black);
		letterbox.Apply ();

	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.name == "Player") {
			playerInPortal = true;
		}


	}

	void OnTriggerExit2D(Collider2D other) {
		
		if (other.name == "Player") {
			playerInPortal = false;
		}
	}

	Vector3 PortalOffsetFromDirection(TopDown2DPlayer.Direction dir) {

		switch (dir) {

		default:
		case TopDown2DPlayer.Direction.EAST:
			return new Vector3 (1, 0, 0);
		case TopDown2DPlayer.Direction.NORTHEAST:
			return new Vector3 (0.7f, 0.7f, 0);
		case TopDown2DPlayer.Direction.NORTH:
			return new Vector3 (0, 1, 0);
		case TopDown2DPlayer.Direction.NORTHWEST:
			return new Vector3 (-0.7f, 0.7f, 0);
		case TopDown2DPlayer.Direction.WEST:
			return new Vector3 (-1, 0, 0);
		case TopDown2DPlayer.Direction.SOUTHWEST:
			return new Vector3 (-0.7f, -0.7f, 0);
		case TopDown2DPlayer.Direction.SOUTH:
			return new Vector3 (0, -1, 0);
		case TopDown2DPlayer.Direction.SOUTHEAST:
			return new Vector3 (0.7f, -0.7f, 0);

		}

	}

	void OnGUI() {

		if (playerInPortal) {
			GUI.Label (new Rect (Screen.width/2, Screen.height - 30, 200, 50), "[E] Advance");
		}

		letterboxRect.width = (letterboxFillPercent * 0.01f * Screen.width);
		GUI.DrawTexture (letterboxRect, letterbox);

	}

}
