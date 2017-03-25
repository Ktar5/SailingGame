using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour {

	public float adaptionSpeed = 5.0f;
	public Transform targetTransform;

	void FixedUpdate() {
		Vector3 targetPosition = new Vector3(targetTransform.position.x, targetTransform.position.y, transform.position.z);

		transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * adaptionSpeed);
	
	}

}
