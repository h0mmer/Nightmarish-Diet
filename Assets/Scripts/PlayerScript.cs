﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//PlayerScript inherits from MovingObject, our base class for objects that can move.
public class PlayerScript : MovingObject
{
	public float restartLevelDelay = 1f;        
	public int pointsPerFood = 5; 
	public List<Sprite> spriteList;
	public List<int> playerPhases;

	private SpriteRenderer spriteRdr;           
	private int food;                           


	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		spriteRdr = GetComponent<SpriteRenderer> ();
		food = GameManager.instance.playerFoodPoints;
		base.Start ();
	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
		GameManager.instance.playerFoodPoints = food;
	}


	private void Update ()
	{
		//If it's not the player's turn, exit the function.
		if(!GameManager.instance.playersTurn) return;

		int horizontal = 0;     
		int vertical = 0;       

		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		if(horizontal != 0)
		{
			vertical = 0;
		}

		if(horizontal != 0 || vertical != 0)
		{
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove (horizontal, vertical);
		}
	}


	//AttemptMove overrides the AttemptMove function in the base class MovingObject.
	protected override void AttemptMove (int xDir, int yDir)
	{
		base.AttemptMove (xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//If Move returns true, meaning Player was able to move into an empty space.
		//If not, it will wait until player makes a valid move.
		if (Move (xDir, yDir, out hit)) 
		{
			
			print (food);
			//Since the player has moved and lost food points, check if the game has ended.
			CheckIfGameOver ();
			//Checks if sprite needs to change.
			CheckSprite ();
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.instance.playersTurn = false;
		}
	}


	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		if(other.tag == "Exit")
		{
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}

		else if(other.tag == "Food")
		{
			food += pointsPerFood;

			//Remove food from foods list in Game Manager.
			FoodScript fScript = other.GetComponent<FoodScript>();
			GameManager.instance.foods.Remove (fScript);

			//Disable the food object the player collided with.
			other.gameObject.SetActive (false);
		}
	}


	//Changes player sprite if needed.
	private void CheckSprite(){
		
	}


	//Restart reloads the scene when called.
	private void Restart ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}


	//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		if (food >= 100) 
		{
			GameManager.instance.GameOver ();
		}
	}
}