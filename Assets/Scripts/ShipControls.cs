﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : Pausable
{

	public float shipSpeed = 3; //how fast should the ship move? (holding S will halve this)
	public float rotateSpeed = 1f; //how fast should the ship rotate?
	public float blinkDistance = 2f; //how far is a blink?
	public float blinkRecharge = 2f; //how much time is there between allowed blinks?
	public float lastBlink; //when did the user last blink?
	public float BlinkCharge //returns a value between 1 and 0 showing how recharged blink is
	{                        //(1 being fully charged and 0 being completely empty)
		get
		{
			if ( nowPaused )
			{
				return blinkTime / blinkRecharge;
			} else
				return Mathf.Clamp01((Time.time - lastBlink)/blinkRecharge); //math to calculate the charge based on what time the last blink was done at, the current time and the time of recharge between blinks
		}
	}
	public SpriteRenderer FrontSprite;
	public float blinkTime {get; private set;} //for when the game is paused

	

	void Start()
	{
		SubscribeToPause();
	}

	internal override void OnPauseChange()
	{
		base.OnPauseChange();
		//we need to make sure the charge of the blink doesn't drift while we're paused
		if ( nowPaused )
		{ // if we just paused
			blinkTime = Time.time - lastBlink; // store the blink state
		}
		else // if we just unpaused
		{
			lastBlink = Time.time - blinkTime;
		}
	}

	void Blink()
	{
		if ( BlinkCharge == 1f )
		{
			transform.Translate( 0f, blinkDistance, 0f, Space.Self ); //move blinkDistance far forward
			lastBlink = Time.time;
		}
	}
	
	void Update () {

		
		transform.Translate( 0f, shipSpeed * Time.deltaTime, 0f, Space.Self ); //move forward

		if ( Input.GetKey( KeyCode.DownArrow ) ) // if the user wants to move slower
		{
			transform.Translate( 0f, -shipSpeed * 0.5f * Time.deltaTime, 0f, Space.Self ); // move halfway back ()
		}

		// blinkey stuff:

		Color frontColor = new Color(0f, .5f + (BlinkCharge==1f?.3f:0f), BlinkCharge);
		FrontSprite.color = frontColor;
		if ( Input.GetKeyDown( KeyCode.UpArrow ) ) // if the user wants to blink
		{
			Blink();
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
