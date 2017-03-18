using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Pixel Perfect Dynamic Objects:

IMPORTANT NOTE: DO NOT USE THIS SCRIPT FOR OBJECTS THAT WILL ROTATE FREQUENTLY (GUNS AND SUCH). ONLY
OBJECTS THAT MOVE.

Because Unity uses floats for coordinates, we need to manually round out the coordinates, otherwise
we end up with artifacts where a pixel is in two screen points at once. Attach this code to any object
that will be moving during the game.

*/
[ExecuteInEditMode]
public class PixelPerfectDynamicObject : MonoBehaviour {


	public float pixelsPerUnit = 100.0f;
	private float unitsPerPixels;

	void Start() {
		unitsPerPixels = 1.0f / pixelsPerUnit;
	}

	/*
	Run in LateUpdate as to not interfere with physics code.

	*/

	void LateUpdate() {
		RoundCoordinates ();
	}

	/*

	Round Coordinates:
	Rounds out the transform position coordinates. I used Mathf.Floor instead of Mathf.Round because
	the latter can cause jumpy results while the former is more consistent.

	*/
	void RoundCoordinates() {
		transform.position = unitsPerPixels * new Vector2(Mathf.Floor (transform.position.x * pixelsPerUnit), Mathf.Floor (transform.position.y * pixelsPerUnit));
	}

}
