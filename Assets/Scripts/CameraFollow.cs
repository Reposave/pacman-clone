using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform target;//our player
	public float smoothSpeed = 10f; //time taken for camera to catch up.


	private Vector3 velocity = Vector3.zero;

	public Vector3 offset;//positioning of the camera away from the target.

	public GameObject PacMan;

	void Start(){
		PacMan = GameObject.Find("PacMan");
	}

	void FixedUpdate(){//Executed after every update.

		float sign = Mathf.Sign((PacMan.transform.localPosition.x -13));

		offset.y = PacMan.transform.localPosition.y-10;

		offset.x = ((PacMan.transform.localPosition.x -13) + (-sign*19)); 

		Vector3 desiredPosition = target.position + offset;
		Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition,ref velocity,smoothSpeed * Time.deltaTime);
		//will smoothly transition between two points.

		transform.position = smoothedPosition;

		transform.LookAt(target); //unity will do the calculations required to make sure rotation follows player
	}

}
