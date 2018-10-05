using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : MonoBehaviour {

	public float shipSpeed = 3; //how fast should the ship move? (holding S will halve this)
	public float rotateSpeed = 1f; //how fast should the ship rotate?
	public float blinkDistance = 2f; //how far is a blink?
	public float lastBlink; //when did the user last blink?

	void Start () {
		
	}
	
	void Update () {

		
		transform.Translate( 0f, shipSpeed * Time.deltaTime, 0f, Space.Self ); //move forward

		if ( Input.GetKey( KeyCode.DownArrow ) ) // if the user wants to move slower
		{
			transform.Translate( 0f, -shipSpeed * 0.5f * Time.deltaTime, 0f, Space.Self ); // move halfway back ()
		}

		// blinkey stuff:

		if ( Input.GetKeyDown( KeyCode.UpArrow ) ) // if the user wants to blink
		{
			transform.Translate( 0f, blinkDistance, 0f, Space.Self ); //move blinkDistance far forward
		}
		//rotaty stuff:

		if ( Input.GetKey( KeyCode.LeftArrow ) ) // if the user wants to turn left
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * 90 * Time.deltaTime ); // rotate left
		}
		if ( Input.GetKey( KeyCode.RightArrow ) ) // if the user wants to turn right
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * -90 * Time.deltaTime ); // rotate right
		}
	}
}
