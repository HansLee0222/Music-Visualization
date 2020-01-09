using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{

	public GameObject player;
	public Vector3 playerOffset;
	public float sensitivity;


    // Update is called once per frame
    void Update(){
    	transform.position = player.transform.position + playerOffset;

    	float rotateHorizontal = Input.GetAxis ("Mouse X");
        float rotateVertical = Input.GetAxis ("Mouse Y");
        transform.RotateAround (transform.position, -Vector3.up, -rotateHorizontal * sensitivity); //use transform.Rotate(-transform.up * rotateHorizontal * sensitivity) instead if you dont want the camera to rotate around the player
        transform.RotateAround (transform.position, transform.right, -rotateVertical * sensitivity); // again, use transform.Rotate(transform.right * rotateVertical * sensitivity) if you don't want the camera to rotate around the player
    }

}
