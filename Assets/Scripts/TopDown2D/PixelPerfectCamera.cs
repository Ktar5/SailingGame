using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

All this does is make the camera's orthographic size update based on ppu.

*/
[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour {

	public float pixelsPerUnit = 100.0f;
	private float unitsPerPixel;

	void Start() {

		unitsPerPixel = 1.0f / pixelsPerUnit;

		Camera cam = GetComponent<Camera> ();

		cam.orthographicSize = (Screen.height / 2.0f) * unitsPerPixel;

	}

}
