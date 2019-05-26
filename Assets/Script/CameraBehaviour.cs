using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform Board;

	int rotateUpSpeed = 0;

	float mouseX = 0.0f;
	float mouseY = 0.0f;

	float speed = 2.0f;

	// Use this for initialization
	void Start () {
		//transform.LookAt(Board);
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			rotateUpSpeed = 20;
		}

		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			rotateUpSpeed = -20;
		}

		if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
		{
			rotateUpSpeed = 0;
		}

		mouseX = Input.GetAxis("Mouse X");
		mouseY = Input.GetAxis("Mouse Y");
		//Debug.Log(mouseX + " " + mouseY);
		
		//transform.LookAt(Board);
		transform.RotateAround(Board.position,Vector3.up,rotateUpSpeed*Time.deltaTime);

	}
}
