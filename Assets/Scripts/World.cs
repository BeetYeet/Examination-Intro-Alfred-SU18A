using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World: MonoBehaviour
{

	public static World current //the one and only instance of world
	{
		get;
		private set;
	}
	public bool isPaused
	{
		get
		{
			return pauseCodes.Count == 0;
		}
	}
	public event System.Action OnPauseChange; // all things that should be pausable must subscribe to this
	public event System.Action OnGameStart;

	/*
	 * Special pause codes:
	 * 0 - requires force to be unpaused
	 * 1 - startup
	 */
	private List<uint> pauseCodes; // if a pausecode appears here, dont unpause untill its gone

	void Awake()
	{ // make it awake so that it is called way before all other scripts so that they can cashe it in Start()
		if ( World.current != null ) // íf there already is an instance of World
		{
			Debug.LogError( "Multiple instances of World exist, please keep it to one", this ); // throw an error
			return; // and return
		}
		World.current = this; // else just set it to this instance
		pauseCodes = new List<uint>();
		Pause( 1 ); // pause the game and wait for input so that we know that the user is ready to play
	}


	/// <summary>
	/// to pause the code
	/// </summary>
	/// <param name="pauseCode">a unique code for the specific purpouse of the pause so that you can pause from multiple sources (for example opening the pause menu during an event)</param>
	/// <returns>returns true if it wasn't paused before</returns>
	public bool Pause( uint pauseCode )
	{
		if ( pauseCode == 0 )
		{
			Debug.LogError( "Disallowed pauscode: 0", this );
			return false;
		}
		bool returns = true; // a variable to store the result
		if ( isPaused ) // if its already paused
		{
			returns = false; //mark it in the variable
		}
		pauseCodes.Add( pauseCode ); //adds the pausecode to the List

		return returns;
	}


	/// <summary>
	/// Forcefull pause, won't unpause without the Unpause() (forcefull unpause)
	/// </summary>
	/// <returns>returns true if it wasn't paused before</returns>
	public bool Pause() // to pause the code, makes it not unpause unless its forced, returns true if it wasn't paused before
	{
		bool returns = true; // a variable to store the result
		if ( isPaused ) // if its already paused
		{
			returns = false; //mark it in the variable
		}
		if ( pauseCodes.Contains( 0 ) ) // if it is already forcefully paused
		{
			Debug.LogWarning( "Attempted to force a pause when it is already forcefully paused" ); // warn for it
			//
		}
		else // we need to add it
			pauseCodes.Add( 0 ); //adds the pausecode for forcefull to the List

		return returns;
	}


	/// <summary>
	/// Unpauses the game if pauseCode it the only pause code, otherwise unpauses only from pauseCode, raises an error if the code isn't in the list
	/// </summary>
	/// <param name="pauseCode">a unique code for the specific purpouse of the pause so that you can pause from multiple sources (for example opening the pause menu during an event)</param>
	/// <returns>true if it successfully unpaused it, false if theres another pausecode keeping it from unpausing</returns>
	public bool Unpause( uint pauseCode )
	{
		return Unpause( pauseCode, false );
	}

	/// <summary>
	/// Unpauses the game if pauseCode it the only pause code, otherwise unpauses only from pauseCode
	/// </summary>
	/// <param name="pauseCode">a unique code for the specific purpouse of the pause so that you can pause from multiple sources (for example opening the pause menu during an event)</param>
	/// <param name="allowNotThere">if true wont raise an error if the code isn't in the list</param>
	/// <returns>true if it successfully unpaused it, false if theres another pausecode keeping it from unpausing</returns>
	public bool Unpause( uint pauseCode, bool allowNotThere )
	{
		bool wasPaused = isPaused;
		bool returns = true; // a variable to store the result

		if ( pauseCodes.Contains( pauseCode ) )
		{
			if ( allowNotThere )
			{
				return false;
			}
			Debug.LogError( "Errorcode does not exist, maby use allowNotThere parameter?" );
			return false;
		}



		if ( isPaused ) // if its already paused
		{
			returns = false; //mark it in the variable
		}
		if ( wasPaused != isPaused )
		{
			OnPauseChange();
		}
		return returns;
	}

	void Update()
	{

	}


}

/// <summary>
/// A class to derive from if you want an autoupdated isPaused variable
/// </summary>
public class Pausable: MonoBehaviour
{
	public bool nowPaused
	{
		get;
		private set;
	}
	internal virtual void SubscribeToPause()
	{
		World.current.OnPauseChange += OnPauseChange; // subsribe to the OnPauseChange event
	}

	internal virtual void OnPauseChange()
	{
		nowPaused = World.current.isPaused;
	}
}
