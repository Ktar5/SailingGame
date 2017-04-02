using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

	public Portal otherPortal;
	public TopDown2DPlayer.Direction facingDirection;


	void OnTriggerEnter2D(Collider2D other) {

		TopDown2DPlayer player = other.GetComponent<TopDown2DPlayer> ();

		if (player != null) {
			player.transform.position = otherPortal.transform.position + (2.0f * PortalOffsetFromDirection(otherPortal.facingDirection));
			player.movement.direction = otherPortal.facingDirection;

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

}
