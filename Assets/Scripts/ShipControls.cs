using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControls : Pausable
{

	public float shipSpeed = 3; //how fast should the ship move? (holding S will halve this)
	public float rotateSpeed = 1f; //how fast should the ship rotate?
	public float blinkDistance = 2f; //how far is a blink?
	public float blinkRecharge = 2f; //how much time is there between allowed blinks?
	private float lastBlink; //when did the user last blink?
	public float BlinkCharge //returns a value between 1 and 0 showing how recharged blink is
	{                        //(1 being fully charged and 0 being completely empty)
		get
		{
			if ( nowPaused )
			{
				return blinkTime / blinkRecharge; // to emulate pausing Time.time
			} else
				return Mathf.Clamp01((Time.time - lastBlink)/blinkRecharge); //math to calculate the charge based on what time the last blink was done at, the current time and the time of recharge between blinks
		}
	}
	public SpriteRenderer FrontSprite;
	public float blinkTime {get; private set;} //for when the game is paused
	public ParticleSystem ps; // för raketeffekt
	public float maxPlayerX = 8.8f; // max X i positivt X led 
	public float maxPlayerY = 5f; // max Y i positivt Y led
	private int lastPrint; // senaste gången tiden printades
	public List<SpriteRenderer> coloredDetails; // alla detaljer som ska färjas med ColorDetails()

	void RandomColor() // randomiserar skeppets detaljer
	{
		//tre värden mellan 0 och 1 av typ float motsvarande röd, grön och blå
		float r = Random.Range( 0f, 1f );
		float g = Random.Range( 0f, 1f );
		float b = Random.Range( 0f, 1f );
		ColorDetails(new Color(r, g, b)); // färja detaljerna
	}

	void ColorDetails( Color c ) //c är färjen som detaljerna ska få
	{
		foreach ( SpriteRenderer s in coloredDetails ) // gå igenom alla spriterenderers
		{
			s.color = c; // sätt de till färjen
		}
	}

	void Update () {

		//print time
		if ( Time.time > lastPrint + 1f ) // har det gått en sekund?
		{
			lastPrint = Mathf.RoundToInt(Time.time); // ställ om variabeln
			Debug.Log("Time: " + lastPrint); // printa den
		}
		
		// Static stuff

		if ( Input.GetKeyDown( KeyCode.P ) ) // om spelaren vill pausa
		{
			if ( nowPaused ) // if user wants to unpause
			{
				World.current.Unpause( 3 ); //unpause with code 3
			}
			else // if user wants to pause
			{
				World.current.Pause( 3 ); //pause with code 3
			}
		}

		if ( Input.GetKeyDown( KeyCode.Space ) ) // om spelaren vill byta färg
		{
			RandomColor(); // byt färg
		}

		// Movement:
		if ( nowPaused ) // om det är pausat
			return; // hoppa över att göra saker som rör på skeppet
			

		transform.Translate( 0f, shipSpeed * Time.deltaTime, 0f, Space.Self ); //move forward

		if ( Input.GetKey( KeyCode.S ) ) // if the user wants to move slower
		{
			transform.Translate( 0f, -shipSpeed * 0.5f * Time.deltaTime, 0f, Space.Self ); // move halfway back ()
		}

		// blinkey stuff:

		FrontSprite.color = new Color(0f, .5f + (BlinkCharge==1f?.3f:0f), BlinkCharge); // matte för färjen på framsidan
		if ( Input.GetKeyDown( KeyCode.W ) ) // if the user wants to blink
		{
			Blink(); // självförklarande
		}

		//rotaty stuff:
		if ( Input.GetKeyUp( KeyCode.A ) || Input.GetKeyUp( KeyCode.D ) )
			ColorDetails( new Color( 0.98f, 0.77f, 0.33f ) ); // om spelaren släpper vänster eller höger knapparna så byt tillbaka färjen (för att den randomiserade färjen inte ska försvinna)
		if ( Input.GetKey( KeyCode.A ) ) // if the user wants to turn left
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * 80 * Time.deltaTime ); // rotate left
			ColorDetails(Color.green);// självförklarande
		}
		if ( Input.GetKey( KeyCode.D ) ) // if the user wants to turn right
		{
			transform.Rotate( new Vector3( 0f, 0f, 1f ), rotateSpeed * -120 * Time.deltaTime ); // rotate right
			ColorDetails( Color.blue );// självförklarande
		}

		// stay on screen stuff

		if ( this.transform.position.x > maxPlayerX || this.transform.position.x < -maxPlayerX ) // if not in screen
		{
			this.transform.position = new Vector3( -Mathf.Clamp(this.transform.position.x, -maxPlayerX, maxPlayerX ), this.transform.position.y, this.transform.position.z ); //put on screen
		}

		if ( this.transform.position.y > maxPlayerY || this.transform.position.y < -maxPlayerY ) // if not in screen
		{
			this.transform.position = new Vector3( this.transform.position.x, -Mathf.Clamp( this.transform.position.y, -maxPlayerY, maxPlayerY ), this.transform.position.z ); //put on screen
		}
	}

	void Start()
	{
		SubscribeToPause(); // startfunktion till Pausable classen
		shipSpeed += Random.Range( -1f, 1f ); // randomisering
		this.transform.position = new Vector3( Random.Range( -maxPlayerX, maxPlayerX ), Random.Range( -maxPlayerY, maxPlayerY ) ); // randomisering
	}

	internal override void OnPauseChange() // om spelet blivit pausat eller avpausat
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

	void Blink() // blinka framåt
	{
		if ( BlinkCharge == 1f ) // om du har tillräckligt med charge
		{
			transform.Translate( 0f, blinkDistance, 0f, Space.Self ); //move blinkDistance far forward
			lastBlink = Time.time; // håll koll på när vi blinkade
		}
	}
}
