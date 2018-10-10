using System.Collections;
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
	public ParticleSystem ps;
	public float maxPlayerX = 8.8f;
	public float maxPlayerY = 5f;
	public int lastPrint;
	public List<SpriteRenderer> coloredDetails;

	void RandomColor()
	{
		float r = Random.Range( 0f, 1f );
		float g = Random.Range( 0f, 1f );
		float b = Random.Range( 0f, 1f );
		ColorDetails(new Color(r, g, b));
	}

	void Warp()
	{
		this.transform.position = new Vector3( -this.transform.position.x, -this.transform.position.y, this.transform.position.z );
	}

	void ColorDetails( Color c )
	{
		foreach ( SpriteRenderer s in coloredDetails )
		{
			s.color = c;
		}
	}

	void Update () {

		//print time
		if ( Time.time > lastPrint + 1f )
		{
			lastPrint = Mathf.RoundToInt(Time.time);
			Debug.Log("Time: " + lastPrint);
		}

		nowPaused = World.current.isPaused;
		// Static stuff

		if ( Input.GetKeyDown( KeyCode.P ) )
		{
			if ( nowPaused ) // if user wants to unpause
			{
				Debug.Log( "Unpaused" );
				World.current.Unpause( 3 ); //unpause with code 3
			}
			else // if user wants to pause
			{
				Debug.Log( "Paused" );
				World.current.Pause( 3 ); //pause with code 3
			}
		}

		if ( Input.GetKeyDown( KeyCode.Space ) )
		{
			RandomColor();
		}

		// Movement:
		if ( nowPaused )
			return;
			

		transform.Translate( 0f, shipSpeed * Time.deltaTime, 0f, Space.Self ); //move forward

		if ( Input.GetKey( KeyCode.S ) ) // if the user wants to move slower
		{
			transform.Translate( 0f, -shipSpeed * 0.5f * Time.deltaTime, 0f, Space.Self ); // move halfway back ()
		}

		// blinkey stuff:

		Color frontColor = new Color(0f, .5f + (BlinkCharge==1f?.3f:0f), BlinkCharge);
		FrontSprite.color = frontColor;
		if ( Input.GetKeyDown( KeyCode.W ) ) // if the user wants to blink
		{
			Blink();
		}

		//rotaty stuff:
		if ( Input.GetKeyUp( KeyCode.A ) || Input.GetKeyUp( KeyCode.D ) )
			ColorDetails( new Color( 0.98f, 0.77f, 0.33f ) );
		if ( Input.GetKey( KeyCode.A ) ) // if the user wants to turn left
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * 80 * Time.deltaTime ); // rotate left
			ColorDetails(Color.green);
		}
		if ( Input.GetKey( KeyCode.D ) ) // if the user wants to turn right
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * -120 * Time.deltaTime ); // rotate right
			ColorDetails( Color.blue );
		}

		if ( this.transform.position.x > maxPlayerX )
		{
			this.transform.position += new Vector3(-0.1f, 0, 0);
			Warp();
		}

		if ( this.transform.position.x < -maxPlayerX )
		{
			this.transform.position += new Vector3( 0.1f, 0, 0 );
			Warp();
		}

		if ( this.transform.position.y > maxPlayerY )
		{
			this.transform.position += new Vector3( 0, -0.1f, 0 );
			Warp();
		}

		if ( this.transform.position.y < -maxPlayerY )
		{
			this.transform.position += new Vector3( 0, 0.1f, 0 );
			Warp();
		}
	}

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
			ps.Pause(); //pause the particle system
		}
		else // if we just unpaused
		{
			lastBlink = Time.time - blinkTime; // calculate the proper values to emulate there not being a paus
			ps.Play(); //play the particle system
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

	public void Respawn()
	{
		
	}
}
