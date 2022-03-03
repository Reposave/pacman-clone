using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {
	private Animator anim;

	private float moveSpeed = 9f;
	private float normalMoveSpeed = 9f;
	private float frightenedMoveSpeed = 5.9f;
	private float ConsumedMoveSpeed = 15f;
	private float previousMoveSpeed;

	private int frightenedModeDuration = 5;
	private int startBlinkingAt = 3;
	private float frightenedModeTimer = 0;//Keeps track of elapsed time for frightened mode timer.

	private float ghostReleaseTimer = 0;
	private int pinkyReleaseTimer = 1;
	private int inkyReleaseTimer =2;
	private int clydeReleaseTimer = 3;

	public Node homeNode;//The place the ghosts will run to when in scatter mode.

	public bool isInGhostHouse = false;
	public bool canMove = true;

	public GhostType ghostType;

	public Node startingPosition;
	public Node GhostHouse;

	public int[] scatterModeTimer = new[]{7,8,9,10};
	public int[] chaseModeTimer = new[] {20,21,22,23};

	public RuntimeAnimatorController GhostBlue;
	public RuntimeAnimatorController Default;
	public RuntimeAnimatorController ConsumedAnim;

	private int modeChangeIteration = 1;
	private float modeChangeTimer = 0;

	public enum Mode {
		Chase, Scatter, Frightened, Consumed
	}

	Mode currentMode = Mode.Scatter;
	Mode previousMode;

	public enum GhostType{
		Red, Pink, Blue, Orange
	}

	private GameObject pacMan;

	private Node CurrentNode, targetNode, previousNode;
	private Vector2 direction, NextDirection;

	private AudioSource BackgroundAudio;

	private GameBoard gamebrd;

	public bool belongsToPlayerOne = false;

	public Sprite PacMan;// Determines size of collision box for pac man and the ghost.
	public Sprite Ghosts;

	// Use this for initialization
	void Start () {

		gamebrd = GameObject.Find("Game").transform.GetComponent<GameBoard>();
		//This is for the prefabs as they cannot use instance references.

		BackgroundAudio = GameObject.Find("Game").transform.GetComponent<AudioSource>();

		anim = GetComponent<Animator>();
		pacMan = GameObject.FindGameObjectWithTag("PacMan");

		Node node = GetNodeAtPosition(transform.localPosition);

		startingPosition = node;

		if(node != null){
			CurrentNode = node;
		}

		if(isInGhostHouse){
			direction = Vector2.up;
			targetNode = CurrentNode.neighbours[0];

		}
		direction = Vector2.right;

		previousNode = CurrentNode;

		targetNode = ChooseNextNode();

		UpdateAnimatorController();

		if(GameBoard.isPlayerOneUp){

			SetDifficultyForLevel(GameBoard.playerOneLevel);

		} else {

			SetDifficultyForLevel(GameBoard.playerTwoLevel);
		}

		gamebrd.IntimidationText.text = frightenedModeDuration.ToString();
	}

	public void SetDifficultyForLevel( int level){
		if(level==1){
			//slow ghosts
			//slow intimidation timers
			//basics
			scatterModeTimer[0] = 7;
			scatterModeTimer[1] = 7;
			scatterModeTimer[2] = 5;
			scatterModeTimer[3] = 5;

			chaseModeTimer[0] = 20;
			chaseModeTimer[1] = 20;
			chaseModeTimer[2] = 20;
			chaseModeTimer[3] = 20;

			frightenedModeDuration = 23;
			startBlinkingAt = 20;

			pinkyReleaseTimer = 16;
			inkyReleaseTimer = 5;
			clydeReleaseTimer = 11;

			moveSpeed = 7f;
			normalMoveSpeed = 7f;
			frightenedMoveSpeed = 2.9f;
			ConsumedMoveSpeed = 15f;
		}
		else if(level==2){
			//faster ghosts
			//slightly faster pacman
			//slow intimidation
			//quick thinking
			scatterModeTimer[0] = 7;
			scatterModeTimer[1] = 7;
			scatterModeTimer[2] = 5;
			scatterModeTimer[3] = 1;

			chaseModeTimer[0] = 20;
			chaseModeTimer[1] = 20;
			chaseModeTimer[2] = 20;
			chaseModeTimer[3] = 20;

			frightenedModeDuration = 20;
			startBlinkingAt = 17;

			pinkyReleaseTimer = 12;
			inkyReleaseTimer = 4;
			clydeReleaseTimer = 8;

			moveSpeed = 8f;
			normalMoveSpeed = 8f;
			frightenedMoveSpeed = 3.9f;
			ConsumedMoveSpeed = 18f;

		}
		if(level==3){
			//faster pacman
			//normal ghosts
			//much faster intimidation timer
			//hordes
			scatterModeTimer[0] = 7;
			scatterModeTimer[1] = 7;
			scatterModeTimer[2] = 5;
			scatterModeTimer[3] = 1;

			chaseModeTimer[0] = 30;
			chaseModeTimer[1] = 30;
			chaseModeTimer[2] = 120;
			chaseModeTimer[3] = 30;

			frightenedModeDuration = 10;
			startBlinkingAt = 7;

			pinkyReleaseTimer = 10;
			inkyReleaseTimer = 2;
			clydeReleaseTimer = 4;

			moveSpeed = 8f;
			normalMoveSpeed = 8f;
			frightenedMoveSpeed = 4.9f;
			ConsumedMoveSpeed = 20f;
		}
		if(level==4){
			//faster ghosts, near your speeds.
			//long intimidation
			//sacrifice safety
			scatterModeTimer[0] = 7;
			scatterModeTimer[1] = 7;
			scatterModeTimer[2] = 5;
			scatterModeTimer[3] = 5;

			chaseModeTimer[0] = 10;
			chaseModeTimer[1] = 10;
			chaseModeTimer[2] = 10;
			chaseModeTimer[3] = 10;

			frightenedModeDuration = 14;
			startBlinkingAt = 11;

			pinkyReleaseTimer = 4;
			inkyReleaseTimer = 2;
			clydeReleaseTimer = 6;

			moveSpeed = 11f;
			normalMoveSpeed = 11f;
			frightenedMoveSpeed = 5.9f;
			ConsumedMoveSpeed = 22f;
		}
		if(level==5){
			//slower ghosts
			//faster pacman
			//QUICK TIMER
			//Deadly level
			scatterModeTimer[0] = 5;
			scatterModeTimer[1] = 5;
			scatterModeTimer[2] = 5;
			scatterModeTimer[3] = 1;

			chaseModeTimer[0] = 20;
			chaseModeTimer[1] = 20;
			chaseModeTimer[2] = 120;
			chaseModeTimer[3] = 20;

			frightenedModeDuration = 6;
			startBlinkingAt = 3;

			pinkyReleaseTimer = 3;
			inkyReleaseTimer = 1;
			clydeReleaseTimer = 2;

			moveSpeed = 9f;
			normalMoveSpeed = 9f;
			frightenedMoveSpeed = 5.9f;
			ConsumedMoveSpeed = 24f;
		}
	}
	// Update is called once per frame

	void Update () {

	if(canMove){
		ModeUpdate2();
		Move();
		ReleaseGhosts();
		CheckCollisions();
		CheckIsInGhostHouse();
		}
	}
	public void MoveToStartingPosition(){

		transform.GetComponent<Animator>().runtimeAnimatorController = Default;

		currentMode = Mode.Scatter;
		//return to scatter speed.

		moveSpeed = normalMoveSpeed;

		previousMoveSpeed = 0;

		transform.position = startingPosition.transform.position; //reset Ghost's position.

		ghostReleaseTimer = 0;
		modeChangeIteration = 1;
		modeChangeTimer = 0;

		if(transform.name != "Ghost Blinky")
			isInGhostHouse = true;

		CurrentNode = startingPosition;

		if(isInGhostHouse){
			direction = Vector2.up;
			targetNode = CurrentNode.neighbours[0];
		}else{
			direction = Vector2.left;
			targetNode = ChooseNextNode();
		}
		previousNode = CurrentNode;

	}
	public void Restart(){

		canMove = true;

		if(ghostType!=GhostType.Red){
			isInGhostHouse = true;
		}else{
			isInGhostHouse = false;
		}

		ghostReleaseTimer = 0;

		transform.GetComponent<Animator>().enabled = true;
	}

	void CheckCollisions(){
		Rect ghostRect = new Rect(transform.position, Ghosts.bounds.size / 4);
		// THE Rectangle size is divided by 4 just to make the collision box smaller.
		Rect pacManRect = new Rect(pacMan.transform.position, PacMan.bounds.size / 4);

		if(ghostRect.Overlaps (pacManRect)){
			Debug.Log("COLLIDED");
			Debug.Log(currentMode);

			if(belongsToPlayerOne&&GameBoard.isPlayerOneUp){
			//If it belongs to player one and  it's player one's turn
				if(currentMode == Mode.Frightened){
					Consumed();
					}
				else{
					//PacMan Should die.
					if(currentMode != Mode.Consumed){
						GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();
						}
					}
				}else if(!belongsToPlayerOne&&!GameBoard.isPlayerOneUp){
				//If it doesn't belong to player one and it's not player one's turn.
					if(currentMode == Mode.Frightened){
						Consumed();
						}
					else{
						//PacMan Should die.
						if(currentMode != Mode.Consumed){
							GameObject.Find("Game").transform.GetComponent<GameBoard>().StartDeath();
							}
						}
				}
			}
		}

	void Consumed(){

		if(GameMenu.isOnePlayerGame){

			GameBoard.playerOneScore +=GameBoard.ghostConsumedRunningScore;
			GameMenu.playerOneGhostsConsumed+=1;

		} else {
			if(GameBoard.isPlayerOneUp){

				GameBoard.playerOneScore +=GameBoard.ghostConsumedRunningScore;
				GameMenu.playerOneGhostsConsumed+=1;

			} else {

				GameBoard.playerTwoScore +=GameBoard.ghostConsumedRunningScore;
				GameMenu.playerTwoGhostsConsumed+=1;
			}
		}

		//ChangeMode(Mode.Consumed); //Don't change into consumed mode.

		GameObject.Find("Game").transform.GetComponent<GameBoard>().StartConsumed(this.GetComponent<Ghost>());

		GameBoard.ghostConsumedRunningScore = GameBoard.ghostConsumedRunningScore +200;
	}

	void CheckIsInGhostHouse(){
		if(currentMode == Mode.Consumed){

			GameObject tile = GetTileAtPosition(transform.position);

			if(tile != null){

				if(tile.transform.GetComponent<Tile>() != null){

					if(tile.transform.GetComponent<Tile>().IsGhostHouse){
						moveSpeed = normalMoveSpeed;
						//reset the move speed.
						Node node = GetNodeAtPosition(transform.position);

						if (node != null){
							CurrentNode = node;

							direction = Vector2.up;
							targetNode = CurrentNode.neighbours[0];
							//the only exit out of the ghost house is upwards and there is only one neighbour.

							previousNode = CurrentNode;
							//return to Chase Mode.
							currentMode = Mode.Chase;

							transform.GetComponent<Animator>().runtimeAnimatorController = Default;
						} 

					}
				}
			}
		}
	}

	void UpdateAnimatorController(){
		if(currentMode != Mode.Frightened){
		//There is no need for these animators when in the frightened mode.
			if(direction == Vector2.up){
				//transform.GetComponent<Animator>().runtimeAnimatorController = GhostUp;
				anim.SetTrigger("Up");
			}
			else if(direction == Vector2.down){
				//transform.GetComponent<Animator>().runtimeAnimatorController = GhostDown;
				anim.SetTrigger("Down");
			}
			else if(direction == Vector2.left){
				//transform.GetComponent<Animator>().runtimeAnimatorController = GhostLeft;
				anim.SetTrigger("left");
			}
			else if(direction == Vector2.right){
				//transform.GetComponent<Animator>().runtimeAnimatorController = GhostRight;
				anim.SetTrigger("Right");
			}
			else{
				anim.SetTrigger("left");;
			}
		}else{
			//Change animation to frightened state.
			transform.GetComponent<Animator>().runtimeAnimatorController = GhostBlue;
		}
	}

	void Move(){
		if(targetNode != CurrentNode && targetNode != null && !isInGhostHouse){

			if(OverShotTarget()){

				CurrentNode = targetNode; //since we overshot our target.

				transform.position = CurrentNode.transform.position;

				GameObject otherPortal = GameObject.Find("PacMan").GetComponent<PacMan>().GetPortal(CurrentNode.transform.position);
				//using pacman's get portal as it has an audiosource that can be played.

				if(otherPortal != null){
					transform.position = otherPortal.transform.position;
					CurrentNode = otherPortal.GetComponent<Node>();

				}
					targetNode = ChooseNextNode();

					previousNode = CurrentNode;

					CurrentNode = null;

					UpdateAnimatorController();

				}
				else {
					transform.position +=  (Vector3)direction * moveSpeed * Time.deltaTime;
				}
			
			}
		}

	Node ChooseNextNode(){

		Vector2 targetTile = Vector2.zero;

		if(currentMode == Mode.Chase){
			targetTile = GetTargetTile();
		}
		else if(currentMode == Mode.Scatter){
			targetTile = homeNode.transform.position;
		}
		else if(currentMode == Mode.Frightened){
			targetTile = GetRandomTile();
		}
		else if(currentMode == Mode.Consumed){
			targetTile = GhostHouse.transform.position;
		}

		Node moveToNode = null;

		Node[] foundNodes = new Node[4];

		Vector2[] foundNodesDirection = new Vector2[4];

		int nodeCounter = 0;

		for (int i = 0; i < CurrentNode.neighbours.Length; i++){

			if(CurrentNode.validDirections[i]!=direction *-1){
			//Our ghosts are not allowed to move backwards.
				if(currentMode != Mode.Consumed){

					GameObject tile = GetTileAtPosition(CurrentNode.transform.position);

					if(tile.transform.GetComponent<Tile>().IsGhostHouseEntrance  == true){
					//do not allow movement downwards if not in consumed mode.
						if(CurrentNode.validDirections[i] != Vector2.down){

							foundNodes[nodeCounter] = CurrentNode.neighbours[i];
							foundNodesDirection [nodeCounter] = CurrentNode.validDirections[i];
							nodeCounter++;
						}
					} else {
					//if it's not an entrance.
							foundNodes[nodeCounter] = CurrentNode.neighbours[i];
							foundNodesDirection [nodeCounter] = CurrentNode.validDirections[i];
							nodeCounter++;
					}
				} else {
				//therefore you are in consumed mode and all movements are allowed.
					foundNodes[nodeCounter] = CurrentNode.neighbours[i];
					foundNodesDirection [nodeCounter] = CurrentNode.validDirections[i];
					nodeCounter++;
				}

			}

		}

		if(foundNodes.Length ==1){ //if only one node was found

			moveToNode= foundNodes[0];
			direction = foundNodesDirection[0];
		}

		if(foundNodes.Length >1){

			float leastDistance = 100000f;

			for (int i = 0; i < foundNodes.Length; i++){

				if(foundNodesDirection[i]!=Vector2.zero){

					float Distance = GetDistance (foundNodes[i].transform.position, targetTile);

					if(Distance < leastDistance){
						leastDistance = Distance;
						moveToNode = foundNodes[i];
						direction = foundNodesDirection [i];
					}
				}
			}

		}
		return moveToNode;

	}

	float GetDistance (Vector2 posA, Vector2 posB){
		float dx = posA.x - posB.x;
		float dy = posA.y - posB.y;

		float distance = Mathf.Sqrt(dx * dx + dy * dy);

		return distance;

	}

	void ModeUpdate2(){
	//Credit Stephanie Hallberg
		if (currentMode != Mode.Frightened) {
            modeChangeTimer += Time.deltaTime; //Records Time elapsed. 
        }
        switch (currentMode) {
            case Mode.Frightened: //If we are in the frightened mode.
            	frightenedModeTimer+=Time.deltaTime;

				gamebrd.IntimidationText.text = (frightenedModeDuration - frightenedModeTimer).ToString("F"); // Two decimal places.


            	if(frightenedModeTimer >= frightenedModeDuration){
					GameObject.Find("Game").transform.GetComponent<GameBoard>().RespawnPellets();

					BackgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudio_normal;
					BackgroundAudio.Play();

            		//when frightened mode is over return audio back to normal.
            		frightenedModeTimer = 0;
					gamebrd.IntimidationText.text = frightenedModeDuration.ToString();
					transform.GetComponent<Animator>().runtimeAnimatorController = Default;

					//reset to default animator controller.
            		ChangeMode(previousMode);

            		return;
            		}

            	if(frightenedModeTimer >= startBlinkingAt){
            		anim.SetTrigger("IsFrightened");
            	}
            	else{
            		anim.SetTrigger("Reset");
            	}

                break;
 
            case Mode.Scatter:
                if (modeChangeTimer > scatterModeTimer[modeChangeIteration]) {
                    ChangeMode (Mode.Chase);
                    modeChangeTimer = 0; //reset clock.
                }
                break;

            case Mode.Chase:
                if(modeChangeTimer > chaseModeTimer[modeChangeIteration]) {
                    modeChangeIteration = (modeChangeIteration + 1)%4; // so if it is 3 to go to 0 for next one
                    ChangeMode (Mode.Scatter);
                    modeChangeTimer = 0;
                }
                break;
            case Mode.Consumed:
				transform.GetComponent<Animator>().runtimeAnimatorController = ConsumedAnim;
				previousMoveSpeed = moveSpeed;
				moveSpeed = ConsumedMoveSpeed;
				break;
        }
    }

	void ChangeMode(Mode m){

		if(currentMode == Mode.Frightened){
			moveSpeed = previousMoveSpeed;
		}
		else if(currentMode!= Mode.Frightened){
		//if it's not already in frightened mode, save the mode.
			previousMode = currentMode;

		}
		if(m == Mode.Frightened){

			previousMoveSpeed = moveSpeed;
			moveSpeed = frightenedMoveSpeed;

		}

		currentMode = m;

		//Used to keep track of previous mode so we can return to it.
		UpdateAnimatorController();

	}
	public void StartFrightenedMode(){//this method will be called by PacMan.

		if(currentMode != Mode.Consumed){
		//only frighten ghosts if not consumed.

			GameBoard.ghostConsumedRunningScore = 200;

			frightenedModeTimer = 0;

			BackgroundAudio.clip = GameObject.Find("Game").transform.GetComponent<GameBoard>().backgroundAudio_frightened;
			BackgroundAudio.loop = false;
			BackgroundAudio.Play();

			ChangeMode(Mode.Frightened);
			}
	}

	GameObject GetTileAtPosition(Vector2 pos){
	 
		int tileX = Mathf.RoundToInt (pos.x);
		int tileY = Mathf.RoundToInt (pos.y); 

		GameObject tile = GameObject.Find("Game").transform.GetComponent<GameBoard>().board[tileX,tileY];

		if(tile!=null){
			return tile;
		}
		return null;
	}

	Vector2 GetRedGhostTargetTile(){
		//The red ghost chases pac man's position.
		Vector2 targetTile = Vector2.zero;
		Vector2 pacManPosition = pacMan.transform.position;

		targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));

		return targetTile;
	}
	Vector2 GetPinkGhostTargetTile(){
		//Has to be four tiles ahead of pac man. If pac man is facing the left, point towards 4 tiles in front of pac man.
		Vector2 targetTile = Vector2.zero;
		Vector2 pacManPosition = pacMan.transform.position;
		Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;//Getting the direction pac man is currently facing.

		targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
		targetTile = targetTile + (4*pacManOrientation);

		return targetTile;
	}
	Vector2 GetBlueGhostTargetTile(){
		//Select the position two tiles in front of Pac-Man
		//Draw Vector from Blinky to that position
		//Double the length of the vector
		Vector2 targetTile = Vector2.zero;
		Vector2 pacManPosition = pacMan.transform.position;
		Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;//Getting the direction pac man is currently facing.

		targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
		targetTile = targetTile + (2*pacManOrientation);

		//-Temporary Blinky Position
		//Vector2 tempBlinkyPosition = GameObject.Find("Ghost Blinky").transform.localPosition; //Name is different.
		Vector2 tempBlinkyPosition = this.transform.localPosition;

		tempBlinkyPosition = new Vector2(Mathf.RoundToInt(tempBlinkyPosition.x),Mathf.RoundToInt(tempBlinkyPosition.y));

		float distance = GetDistance(tempBlinkyPosition, targetTile);
		distance *=2;

		targetTile = new Vector2( tempBlinkyPosition.x + distance, tempBlinkyPosition.y + distance);

		return targetTile;
	}
	Vector2 GetOrangeGhostTargetTile(){
		//Calculate the distance from Pac-Man
		//-If the distance is greater than eight tiles targetting is the same as Blinky
		//if the distance is less than eight tiles, then target is his home node, so scatter mode
		Vector2 targetTile = Vector2.zero;
		Vector2 pacManPosition = pacMan.transform.position;
		//Vector2 pacManOrientation = pacMan.GetComponent<PacMan>().orientation;//Getting the direction pac man is currently facing.
		float distance = GetDistance (transform.localPosition, pacManPosition);

		if(distance > 8){
			targetTile = new Vector2 (Mathf.RoundToInt(pacManPosition.x), Mathf.RoundToInt(pacManPosition.y));
		}
		else if(distance <=8){
			targetTile = homeNode.transform.position;
		}
		return targetTile;
	}

	Vector2 GetTargetTile(){

		Vector2 targetTile = Vector2.zero;

		if(Random.Range(1,10)<=6){ //70% of the time this algorithm is followed.

			if(ghostType == GhostType.Red){

				targetTile = GetRedGhostTargetTile();

			}else if(ghostType == GhostType.Pink){

				targetTile = GetPinkGhostTargetTile();
			}
			 else if(ghostType == GhostType.Blue){

				targetTile = GetBlueGhostTargetTile();
			}
			else if(ghostType == GhostType.Orange){

				targetTile = GetOrangeGhostTargetTile();
			}
		} else {
			targetTile = GetRandomTile();
		}

		return targetTile;
	}

	Vector2 GetRandomTile () {
	//in frightened mode, the ghosts move randomly.
		int x = Random.Range (0,28); //width of the board.
		int y = Random.Range (0, 36); //height of the board.

		return new Vector2 (x,y);
	}

	void ReleasePinkGhost(){
		if((ghostType == GhostType.Pink) && isInGhostHouse){
			isInGhostHouse = false;
		}
	}
	void ReleaseBlueGhost(){
		if((ghostType == GhostType.Blue) && isInGhostHouse){
			isInGhostHouse = false;
		}
	}
	void ReleaseOrangeGhost(){
		if((ghostType == GhostType.Orange) && isInGhostHouse){
			isInGhostHouse = false;
		}
	}

	void ReleaseGhosts(){
		ghostReleaseTimer += Time.deltaTime;

		if(ghostReleaseTimer > pinkyReleaseTimer)
			ReleasePinkGhost();

		if(ghostReleaseTimer > inkyReleaseTimer)
			ReleaseBlueGhost();

		if(ghostReleaseTimer > clydeReleaseTimer)
			ReleaseOrangeGhost();
		
	}
	//Use Vector2.Distance(a,b) to get the difference between two points.

	Node GetNodeAtPosition(Vector2 pos){
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];
		//Find the GameObject named "Game" in the scene and get it's GameBoard component,
		//pass a position into it's array then store it into tile

		if(tile != null){
		if(tile.GetComponent<Node>() != null){
			return tile.GetComponent<Node>();
			//return the Node Component
			}
		}
		return null;


	}
	GameObject GetPortal(Vector2 pos){
		GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x,(int)pos.y];//retrieve object
		//at this position of the array.
		if(tile !=null){

			if(tile.GetComponent<Tile>()!=null){

				if(tile.GetComponent<Tile>().isPortal){//If the isPortal field is true

					GameObject otherPortal = tile.GetComponent<Tile>().portalReceiver; //Get
					//the portalReceiver GameObject that was stored and return it.
					return otherPortal;
					
					}

				}

			}
			return null;
		}

	float LengthFromNode(Vector2 targetPosition){
		Vector2 vec = targetPosition - (Vector2)previousNode.transform.position;
		return vec.sqrMagnitude;
		}

	bool OverShotTarget(){
		float nodeToTarget = LengthFromNode(targetNode.transform.position);
		float nodeToSelf= LengthFromNode(transform.position);

		return nodeToSelf > nodeToTarget; //Comparing the distance between Gelbeeck and the previous node
		//versus the previous Node and the next Node. If Gelbeeck is larger that means we have overshot and
		//returns true.

		}
	public void DestroyGhost(){
		Destroy(this.gameObject);
	}


	}

