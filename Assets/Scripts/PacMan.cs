using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour {

	public GameObject LeftButton;
	public GameObject RightButton;
	public GameObject UpButton;
	public GameObject DownButton;
	public GameObject EnterButton;

	private bool testmode = false;

	public AudioClip chomp1;
	public AudioClip chomp2;
	public AudioClip portalEnter;
	public AudioClip BonusItemConsumed;
	public AudioClip SuperPellet;

	private bool playedChomp1;
	public AudioSource audio;

	public float speed = 10.0f;
	public float DefaultSpeed;

	private Vector2 direction = Vector2.zero;
	public Vector2 orientation;

	public Sprite idleSprite;

	private Vector2 NextDirection;//Foreshadows to pacman where we want to move next when he reaches an intersection.

	private Node CurrentNode; //stores PacMan's current NodePosition.
	private Node targetNode;//PacMan's next Node.
	private Node previousNode; //Keep Track of where we came from.

	private Node startingPosition;

	public bool canMove = true;

	public RuntimeAnimatorController chomp;
	public RuntimeAnimatorController PacManDeath;

	public GameBoard gmboard;

	// Use this for initialization
	void Start () {

		DefaultSpeed = speed;

		audio = transform.GetComponent<AudioSource>();

		Node node = GetNodeAtPosition(transform.localPosition);

		startingPosition = node;

		if(node != null){
			CurrentNode = node;
			Debug.Log(CurrentNode);
		}

		direction = Vector2.left;
		orientation = direction;
		ChangePosition(direction);

		if(GameBoard.isPlayerOneUp){

			SetDifficultyForLevel(GameBoard.playerOneLevel);

		} else {

			SetDifficultyForLevel(GameBoard.playerTwoLevel);
		}
		gmboard = GameObject.Find("Game").transform.GetComponent<GameBoard>();
	}
	public void SetDifficultyForLevel( int level){

		if(level==1){
			speed = DefaultSpeed;
		}
		else if(level==2){
			speed = DefaultSpeed + 1f;
		}
		else if(level==3){
			speed = DefaultSpeed + 3f;
		}
		else if(level==4){
			speed = DefaultSpeed + 4f;
		}
		else if(level==5){
			speed = DefaultSpeed + 6f;
		}
	}

	public void MoveToStartingPosition(){

		transform.GetComponent<Animator>().runtimeAnimatorController = chomp;
		transform.GetComponent<Animator>().enabled = false;
		GetComponent<SpriteRenderer>().sprite = idleSprite;

		transform.position = startingPosition.transform.position; //reset PacMan's position.

		CurrentNode = startingPosition; //We don't want PacMan thinking it's previous node is it's current node.

		NextDirection = Vector2.left;

		direction= Vector2.left;
		orientation = Vector2.left;

		ChangePosition (direction);
	}

	public void Restart(){
		canMove = true;

		transform.GetComponent<Animator>().enabled = true;

	}

	// Update is called once per frame
	void Update () {

		if(canMove){

			CheckInput();
			Move();
			UpdateOrientation();
			UpdateAnimationState();
			ConsumePellet();

			}
	}
	void PlayChompSound(){
		if(playedChomp1){
			//Play Chomp2, set playedChomp1 to false
			audio.PlayOneShot(chomp2);
			playedChomp1 = false;
		}else{
			//-Play chomp 1, set playedChomp1 to true
			audio.PlayOneShot(chomp1);
			playedChomp1 = true;
		}
	}

	void CheckInput(){
		if(Input.GetKeyDown(KeyCode.LeftArrow)){

			LeftButton.GetComponent<SpriteRenderer>().color = Color.white;

			ChangePosition(Vector2.left);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow)){

			RightButton.GetComponent<SpriteRenderer>().color = Color.white;
			
			ChangePosition(Vector2.right);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow)){

			DownButton.GetComponent<SpriteRenderer>().color = Color.white;

			ChangePosition(Vector2.down);
		}
		else if(Input.GetKeyDown(KeyCode.UpArrow)){

			UpButton.GetComponent<SpriteRenderer>().color = Color.white;

			ChangePosition(Vector2.up);

		}else if(Input.GetKeyDown(KeyCode.Return)){

			EnterButton.GetComponent<SpriteRenderer>().color = Color.white;
		}
		else if(Input.GetKeyDown(KeyCode.A)){
			if(testmode){
				//Win Game.
				GameMenu.playerOneGhostsConsumed = GameBoard.totalGhosts;
			}

		}else if(Input.GetKeyDown(KeyCode.C)){
			if(testmode){
				//Enter Consumed Mode.
				GameMenu.playerOnePelletsConsumed=GameBoard.totalNormPellets;
			}

		}else if(Input.GetKeyDown(KeyCode.D)){
			if(testmode){
				//Kill PacMan.
				GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();
				}
		}else if(Input.GetKeyDown(KeyCode.F)){
			if(testmode){
				//Enter Consumed Mode.
				GameMenu.playerTwoPelletsConsumed=GameBoard.totalNormPellets;
			}
		}else if(Input.GetKeyDown(KeyCode.V)){
			if(testmode){
				//Win player 2
				GameMenu.playerTwoGhostsConsumed = GameBoard.totalGhosts;
			}
		}else if(Input.GetKeyDown(KeyCode.Escape)){

			//Quit to Main Menu.
			GameMenu.livesPlayerOne = 0;
			GameMenu.livesPlayerTwo = 0;
			GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();

		}else if(Input.GetKeyDown(KeyCode.P)){
			gmboard.PauseGame();

		}else if(Input.GetKeyDown(KeyCode.KeypadMinus)){
			testmode = !testmode;
			Debug.Log("Test Mode");
		}
		else{
			//Reset Colours to default.
			Color defaultcolor = new Color(69/255f,69/255f,69/255f);

			LeftButton.GetComponent<SpriteRenderer>().color = defaultcolor;
			RightButton.GetComponent<SpriteRenderer>().color = defaultcolor;
			DownButton.GetComponent<SpriteRenderer>().color = defaultcolor;
			UpButton.GetComponent<SpriteRenderer>().color = defaultcolor;
			EnterButton.GetComponent<SpriteRenderer>().color = defaultcolor;
		}


	}
	void UpdateAnimationState(){
		if(direction == Vector2.zero){//if not moving.

			GetComponent<Animator>().enabled = false; //Disable animator
			GetComponent<SpriteRenderer>().sprite = idleSprite; //set sprite to idleSprite

		}else{
			GetComponent<Animator>().enabled = true; //Enable animator
		}
	}

	void ConsumePellet(){
		GameObject o = GetTileAtPosition (transform.position);

		if(o != null){
			Tile tile = o.GetComponent<Tile>();

			if(tile != null){

			bool didConsume = false;

			if(GameBoard.isPlayerOneUp){
				//It is player one's turn.
					if(!tile.didConsumePlayerOne && (tile.isPellet || tile.isSuperPellet)){

						 didConsume = true;
						 tile.didConsumePlayerOne = true;

						 if(tile.isSuperPellet){
						 	GameBoard.playerOneScore += 50;
						 	}
						 else{
						 	GameBoard.playerOneScore += 10;
							GameMenu.playerOnePelletsConsumed+=1;
						 	} 
					}

					if (tile.IsBonusItem){
						ConsumedBonusItem (1, tile);
						}
			} else {
				//It is player two's turn.
					if(!tile.didConsumePlayerTwo && (tile.isPellet || tile.isSuperPellet)){

						 didConsume = true;
						 tile.didConsumePlayerTwo = true;

						if(tile.isSuperPellet){
						 	GameBoard.playerTwoScore += 50;
						 }else{
						 	GameBoard.playerTwoScore += 10;
							GameMenu.playerTwoPelletsConsumed+=1;
						 	} 
					}

					if (tile.IsBonusItem)
						ConsumedBonusItem (2, tile);
			}

				if(didConsume){//If the Object belonging to the tile class
				//has not been consumed and  is a pellet or super pellet, turn off it's sprite renderer.
					o.GetComponent<SpriteRenderer>().enabled = false;
				
					PlayChompSound();

					if(tile.isSuperPellet){
						int pelletsAround = 0;

						foreach(GameObject x in GameBoard.objects){//looping through only gameobjects

							if((x.tag=="Nodes" || x.tag=="Pellets")){

								if(x.GetComponent<Tile>().isPellet){

									if(GameBoard.isPlayerOneUp){

											if(x.GetComponent<Tile>().didConsumePlayerOne){
													pelletsAround+=1;
												}

										}else{
											if(x.GetComponent<Tile>().didConsumePlayerTwo){
													pelletsAround+=1;
												}
									}
								}
							}
						}

						if(!(GameObject.FindObjectOfType<Ghost>())||GameBoard.totalNormPellets <= pelletsAround){
							//If no ghsots are found when a super pellet is consumed, respawn pellets.
							//it will always find a ghost, in a two player game.
							GameObject.Find("Game").transform.GetComponent<GameBoard>().RespawnPellets();
						}

						GameObject.Find("Game").transform.GetComponent<GameBoard>().SpawnGhosts();
						audio.PlayOneShot(SuperPellet);
					}

				}
			}
		}
	}
	void ConsumedBonusItem (int playerNum, Tile bonusItem){

		if (playerNum == 1){

			GameBoard.playerOneScore += bonusItem.pointValue;
			GameMenu.livesPlayerOne +=1;

		} else {

			GameMenu.livesPlayerTwo +=1;
			GameBoard.playerTwoScore += bonusItem.pointValue;
		}
		audio.PlayOneShot(BonusItemConsumed);
		GameObject.Find("Game").transform.GetComponent<GameBoard>().StartConsumedBonusItem(bonusItem.gameObject, bonusItem.pointValue);

	}

	void ChangePosition(Vector2 d){

		if(d!=direction)//If there is a change in direction
			NextDirection = d;

		if(CurrentNode != null){
			Node moveToNode = CanMove(d); //Get the next Node in that direction.

			if(moveToNode != null){
				direction = d;
				targetNode = moveToNode; //Next Node
				previousNode = CurrentNode; //Current Node becomes previous node
				CurrentNode = null; //current node becomes null because as we move we are not on a node.

			}
			
		}
			
	}
	GameObject GetTileAtPosition(Vector2 pos){
		int tileX = Mathf.RoundToInt(pos.x);//We don't want to access floating point numbers as that could break the game.
		int tileY = Mathf.RoundToInt(pos.y);

		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[tileX, tileY];

		if (tile != null)
			return tile;

		return null;

	}
	void Move(){
		if(targetNode != CurrentNode && targetNode != null){
			if(NextDirection == direction *-1){ //check if the next direction is equal to the opposite of the current direction.
				direction *= -1; //if true set the direction to be opposite.

				Node tempNode = targetNode;

				targetNode = previousNode;

				previousNode = tempNode; //make the previousNode, the targetNode and the Target Node the previousNode.

			}

			if(OverShotTarget()){

				CurrentNode = targetNode; //since we overshot our target.

				transform.localPosition = CurrentNode.transform.position;

				GameObject otherPortal = GetPortal(CurrentNode.transform.position);

				if(otherPortal != null){
					transform.localPosition = otherPortal.transform.position;
					CurrentNode = otherPortal.GetComponent<Node>();

				}

				Node moveToNode = CanMove(NextDirection); //look for available nodes to move to in this direction.

				if(moveToNode != null)
					direction = NextDirection;  

				if(moveToNode == null)
					moveToNode = CanMove (direction); //If we can't find next direction, find current direction Node

				if(moveToNode != null){

					targetNode = moveToNode;
					previousNode = CurrentNode;
					CurrentNode = null;

				} else {
					direction = Vector2.zero;//If there isn't any available direction. Stop.
				}
			}
			else{
				transform.localPosition+=(Vector3)direction*speed*Time.deltaTime; //If we haven't Overshot, continue moving.
			}

			}
		}

	
	void UpdateOrientation(){
		if(direction==Vector2.left){

			transform.localRotation = Quaternion.Euler(0,0,0);
			transform.localScale = new Vector3(-1f,1f,1f);
		}
		else if(direction==Vector2.right){

			transform.localRotation = Quaternion.Euler(0,0,0);
			transform.localScale = new Vector3(1f,1f,1f);
		}
		else if(direction==Vector2.up){

			transform.localScale = new Vector3(1f,1f,1f);
			transform.localRotation = Quaternion.Euler(0,0,90f);
		}
		else if(direction==Vector2.down){

			transform.localScale = new Vector3(1f,1f,1f);
			transform.localRotation = Quaternion.Euler(0,0,270f);
		}
		orientation = direction;
	}
	Node GetNodeAtPosition (Vector2 pos){
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];
		//Find the GameObject named "Game" in the scene and get it's GameBoard component,
		//pass a position into it's array then store it into tile

		if(tile != null){
			return tile.GetComponent<Node>();
			//return the Node Component
		}
		return null;

	}

	Node CanMove(Vector2 d){
		Node moveToNode = null;

		for(int i = 0; i< CurrentNode.neighbours.Length;i++){
		//looping through each array element and checking if it's direction matches the direction
		//PacMan wants to go.
			if(CurrentNode.validDirections [i]==d){

				if(CurrentNode.GetComponent<Tile>().IsGhostHouseEntrance){
				//if it is a ghost house, don't allow movement downwards.
					if(d!=Vector2.down){
					moveToNode = CurrentNode.neighbours[i];
					break;
					}

				} else{
					moveToNode = CurrentNode.neighbours[i];
					break;
				}

			}
		}
		return moveToNode;

	}
	void MoveToNode(Vector2 d){
		Node moveToNode = CanMove(d);

		if(moveToNode != null){
			transform.localPosition = moveToNode.transform.position;
			CurrentNode = moveToNode;
			}
		}

	bool OverShotTarget(){
		float nodeToTarget = LengthFromNode(targetNode.transform.position);
		float nodeToSelf= LengthFromNode(transform.localPosition);

		return nodeToSelf > nodeToTarget; //Comparing the distance between Gelbeeck and the previous node
		//versus the previous Node and the next Node. If Gelbeeck is larger that means we have overshot and
		//returns true.

		}

	float LengthFromNode(Vector2 targetPosition){
		Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
		return vec.sqrMagnitude;

		}

	public GameObject GetPortal(Vector2 pos){//We will pass in our current position and do a check if this element is a portal.
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];//retrieve object
		//at this position of the array.



		if(tile !=null){

			if(tile.GetComponent<Tile>()!=null){

				if(tile.GetComponent<Tile>().isPortal){//If the isPortal field is true
					audio.clip = portalEnter;
					audio.Play();

					GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver; //Get
					//the portalReceiver GameObject that was stored and return it.
					return otherPortal;
					
					}

				}

			}
			return null;
		}
	}

