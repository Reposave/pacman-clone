using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public bool isPortal;//This will inform PacMan if the current Node it is on has a Tile Component and is a portal.

	public bool isPellet;
	public bool isSuperPellet;

	public bool didConsumePlayerOne;
	public bool didConsumePlayerTwo;

	public bool IsGhostHouseEntrance;
	public bool IsGhostHouse;

	public bool IsBonusItem;
	public int pointValue; //The points that each bonus item will give you when obtained.
	 
	public GameObject portalReceiver; //This stores the exit portal.
}
