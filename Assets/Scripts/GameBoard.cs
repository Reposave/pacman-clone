using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour {
//Two PlayerGames are broken
	public GameObject Blinky;
	public GameObject Pinky;
	public GameObject Clyde;
	public GameObject Inky;

	private static int boardWidth = 28;
	private static int boardHeight = 36;

	private bool didStartDeath = false;
	private bool didStartConsumed = false;
	public static bool isPlayerOneUp = true;
	public static bool isPlayerTwoFirstTurn = true;

	public static int playerOneLevel=1;
	public static int playerTwoLevel=1;

	public static int ghostConsumedRunningScore;

	public AudioClip WinSound;
	public AudioClip YouWin;
	public AudioClip GameOver;
	public AudioClip Congratulations;

	public AudioClip backgroundAudio_normal;

	public AudioClip backgroundAudio_frightened;

	public AudioClip backgroundAudioPacManDeath;
	public AudioClip backgroundAudioPacManDeath2;

	public AudioClip consumedGhostAudioClip;

	public static int totalPellets = 0;
	public static int totalNormPellets = 0;
	public static int totalSuperPellets = 0;
	public static int totalGhosts = 0; //Win Condition.

	public float score = 0;
	private float blinkIntervalTimer = 0;
	public float blinkIntervalTime = 1f;

	public static float playerOneScore = 0;
	public static float playerTwoScore = 0;
	public static int RankPoint = 0;

	public Text playerText;
	public Text ReadyText;

	public Text IntimidationText;
	public Text playerOneUp;
	public Text playerTwoUp;
	public Text playerOneScoreText;
	public Text playerTwoScoreText;
	public Image playerLives2;

	public Text playerLivesText;
	public Text playerLevelText;

	public Text consumedGhostScoreText;

	public GameObject [,] board = new GameObject[boardWidth, boardHeight];
	private List<GameObject> PlayerOneGhostsRemaining = new List<GameObject>();
	private List<GameObject> PlayerTwoGhostsRemaining = new List<GameObject>(); //Stores Ghosts remaining after Death.

	public bool shouldBlink = false;
	private bool didIncrementLevel = false; 

	private bool isPaused = false;
	public GameObject PauseMenu;

	public Sprite MazeBlue;
	public Sprite MazeWhite;

	public static List<Object> objects;

	bool didSpawnBonusItem1_player1;
	bool didSpawnBonusItem2_player1;
	bool didSpawnBonusItem1_player2;
	bool didSpawnBonusItem2_player2;

	GameObject musicplayer;
	// Use this for initialization
	void Start () {
		isPlayerOneUp = true;

		musicplayer = GameObject.Find("MusicPlayer");


		objects = new List<Object>(GameObject.FindObjectsOfType(typeof(GameObject)));
		/*FindObjectsofType will search the whole scene for any type of objects
		and place them in the Object[] array*/

		foreach(GameObject o in objects){//looping through only gameobjects
			Vector2 pos = o.transform.position;

			if((o.tag=="Nodes" || o.tag=="Pellets")){

				if(o.GetComponent<Tile>().isPellet||o.GetComponent<Tile>().isSuperPellet){
					totalPellets+=1;
					if(o.GetComponent<Tile>().isSuperPellet){
						totalSuperPellets+=1;
					}
				}

				board[(int)pos.x, (int)pos.y]=o; // add the gameObject to this position of the array

			}
			else{
			 	//Debug.Log("Found PacMan at: "+pos); //pointless
			}

		}
		GameObject[] os = GameObject.FindGameObjectsWithTag("Ghost");

		foreach (GameObject ghost in os){
			//Checks for initial ghosts in the scene just in case there are none or present.

			if(isPlayerOneUp){
				ghost.GetComponent<Ghost>().belongsToPlayerOne = true;
				PlayerOneGhostsRemaining.Add(ghost);
			}else{
				ghost.GetComponent<Ghost>().belongsToPlayerOne = false;
				PlayerTwoGhostsRemaining.Add(ghost);
			}
			totalGhosts+=1;
		}

		totalNormPellets = totalPellets - 4;
		totalGhosts = 20;
		totalSuperPellets =4;

		Debug.Log(totalPellets);
		Debug.Log(totalGhosts);
		Debug.Log(totalSuperPellets);
		Debug.Log(totalNormPellets);

		StartGame ();
	}
	private void StartNextLevel(){

			
		StopAllCoroutines ();

		if(playerOneLevel>=6||playerTwoLevel>=6){
			StartCoroutine(ProcessGameOver(1.5f));
		}

		if(playerOneScore > playerTwoScore){
			if(playerOneScore>GameMenu.GetHighScore()){
				GameMenu.High_score = playerOneScore;
				}
		}else{
			if(playerTwoScore>GameMenu.GetHighScore()){
				GameMenu.High_score = playerTwoScore;
				}
		}

		GameMenu.SaveHighScore();
		GameMenu.SaveLevels();
		GameMenu.SavePlayerPrefs();

		if (isPlayerOneUp){

			ResetPelletsForPlayer (1);
			GameMenu.playerOnePelletsConsumed = 0;
			GameMenu.playerOneGhostsConsumed = 0;
			GameMenu.livesPlayerOne+=2;

			didSpawnBonusItem1_player1 = false;
			didSpawnBonusItem2_player1 = false;

		} else {

			ResetPelletsForPlayer (2);
			GameMenu.playerTwoGhostsConsumed = 0;
			GameMenu.playerTwoPelletsConsumed = 0;
			GameMenu.livesPlayerTwo+=2;

			didSpawnBonusItem1_player2 = false;
			didSpawnBonusItem2_player2 = false;
		}

		GameObject.Find ("Maze").transform.GetComponent<SpriteRenderer>().sprite = MazeBlue;

		didIncrementLevel = false;

		StartCoroutine (ProcessStartNextLevel(1));

	}
	IEnumerator ProcessStartNextLevel(float delay){

		playerText.transform.GetComponent<Text>().enabled = true;
		ReadyText.transform.GetComponent<Text>().enabled = true;

		if(isPlayerOneUp)
			StartCoroutine (StartBlinking (playerOneUp));
		else
			StartCoroutine (StartBlinking (playerTwoUp));

		RedrawBoard ();

		yield return new WaitForSeconds (delay);

		StartCoroutine (ProcessRestartShowObjectsAfterWin(1));

	}

	private void CheckShouldBlink(){
		if (shouldBlink) {
			if(blinkIntervalTimer < blinkIntervalTime){
				//Increase Time by Time.deltaTime
				blinkIntervalTimer += Time.deltaTime;
			} else {

				//Reset the timer.
				blinkIntervalTimer = 0;

				if(GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite == MazeBlue){

					GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = MazeWhite;

				} else {

					GameObject.Find("Maze").transform.GetComponent<SpriteRenderer>().sprite = MazeBlue;
				}
			}
		}
	}

	public void StartGame(){

		if(GameMenu.isOnePlayerGame){

			playerOneUp.GetComponent<Text>().enabled = true;
			playerOneScoreText.GetComponent<Text>().enabled = true;

		} else {
			playerOneUp.GetComponent<Text>().enabled = true;
			playerOneScoreText.GetComponent<Text>().enabled = true;

			playerTwoUp.GetComponent<Text>().enabled = true;
			playerTwoScoreText.GetComponent<Text>().enabled = true;
		}

		if (isPlayerOneUp){//This is used to help the player differentiate whose turn it is.

			StartCoroutine (StartBlinking (playerOneUp));

		} else {

			StartCoroutine(StartBlinking (playerTwoUp));

		}

		//Hide All Ghosts
		if(GameObject.FindObjectOfType<Ghost>()){
			GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

			foreach (GameObject ghost in o){

				ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
				ghost.transform.GetComponent<Ghost>().canMove = false;
			}
		}

		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
		pacMan.transform.GetComponent<PacMan>().canMove = false;

		StartCoroutine( ShowObjectsAfter(2.25f));
	}
	public void StartConsumed (Ghost consumedGhost){

		if (!didStartConsumed){//if did not start consumed

		didStartConsumed = true;

		//Pause all Ghosts
		if(isPlayerOneUp){
			List<GameObject> o = PlayerOneGhostsRemaining;

			foreach (GameObject ghost in o){
				ghost.transform.GetComponent<Ghost>().canMove = false;
			}
		}else{
			List<GameObject> o = PlayerTwoGhostsRemaining;

			foreach (GameObject ghost in o){
				ghost.transform.GetComponent<Ghost>().canMove = false;
			}
		}
		//Pause PacMan
		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<PacMan>().canMove = false;

		//Hide PacMan
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

		//Hide the consumed ghost
		consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = false;

		//Stop Background Music
		transform.GetComponent<AudioSource>().Stop(); 

		Vector2 pos = consumedGhost.transform.position;

		Debug.Log(pos);

		Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

		//converts the world space coordinates to canvas coordinates so the consumedGhostText Matches the actual ghost position.
		//tag camera as MainCamera
		consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
		consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

		consumedGhostScoreText.text = ghostConsumedRunningScore.ToString();

		consumedGhostScoreText.GetComponent<Text>().enabled = true;

		//play the consumed sound
		transform.GetComponent<AudioSource>().PlayOneShot (consumedGhostAudioClip);

		//Wait for the audio clip to finish
		StartCoroutine (ProcessConsumedAfter(0.75f, consumedGhost));
		}
	}

	public void StartConsumedBonusItem (GameObject bonusItem, int scorevalue){

		Vector2 pos = bonusItem.transform.position;

		Vector2 viewPortPoint = Camera.main.WorldToViewportPoint(pos);

		//converts the world space coordinates to canvas coordinates so the consumedGhostText Matches the actual ghost position.
		//tag camera as MainCamera
		consumedGhostScoreText.GetComponent<RectTransform>().anchorMin = viewPortPoint;
		consumedGhostScoreText.GetComponent<RectTransform>().anchorMax = viewPortPoint;

		consumedGhostScoreText.text = scorevalue.ToString();

		consumedGhostScoreText.GetComponent<Text>().enabled = true;

		Destroy (bonusItem.gameObject);
		//Wait for the audio clip to finish
		StartCoroutine (ProcessConsumedBonusItem(0.75f));
	}

	IEnumerator ProcessConsumedBonusItem (float delay){

		yield return new WaitForSeconds (delay);

		consumedGhostScoreText.GetComponent<Text>().enabled = false;
	}


	IEnumerator StartBlinking (Text blinkText){

		yield return new WaitForSeconds (0.25f);
		//takes in a text Object and sets it's enabled state to opposite of what it was before every 0.25 seconds.

		blinkText.GetComponent<Text>().enabled = !blinkText.GetComponent<Text>().enabled;
		StartCoroutine(StartBlinking (blinkText));
	}

	IEnumerator ProcessConsumedAfter(float delay, Ghost consumedGhost){

		yield return new WaitForSeconds (delay);

		//Hide Score.
		consumedGhostScoreText.GetComponent<Text>().enabled = false;

		//Show pacMan
		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

		//Show Consumed Ghost
		//consumedGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

		consumedGhost.GetComponent<Ghost>().DestroyGhost();//Kill the ghost.

		//Resume all ghosts
		if(isPlayerOneUp){
			List<GameObject> o = PlayerOneGhostsRemaining;

			PlayerOneGhostsRemaining.Remove(consumedGhost.gameObject);
			objects.Remove(consumedGhost.gameObject);

			foreach (GameObject ghost in o){
				ghost.transform.GetComponent<Ghost>().canMove = true;
			}
		}else{
			List<GameObject> o = PlayerTwoGhostsRemaining;

			PlayerTwoGhostsRemaining.Remove(consumedGhost.gameObject);
			objects.Remove(consumedGhost.gameObject);

			foreach (GameObject ghost in o){
				ghost.transform.GetComponent<Ghost>().canMove = true;
			}
		}

		//Resume PacMan
		pacMan.transform.GetComponent<PacMan>().canMove = true;

		//Start Background Music
		transform.GetComponent<AudioSource>().Play();

		didStartConsumed = false;


	}

	IEnumerator ShowObjectsAfter(float delay){

		yield return new WaitForSeconds (delay);

		if(GameObject.FindObjectOfType<Ghost>()){
			GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

			foreach (GameObject ghost in o){
				// make ghosts visible
				ghost.transform.GetComponent<SpriteRenderer>().enabled = true;
			}
		}

		GameObject pacMan = GameObject.Find("PacMan");

		pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().chomp;
		pacMan.transform.GetComponent<Animator>().enabled = false;
		pacMan.GetComponent<SpriteRenderer>().sprite = pacMan.transform.GetComponent<PacMan>().idleSprite;

		pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;
		//make pacman visible.

		playerText.transform.GetComponent<Text>().enabled = false;

		StartCoroutine(StartGameAfter(2));

	}
	IEnumerator StartGameAfter (float delay){

		yield return new WaitForSeconds (delay);

		if(GameObject.FindObjectOfType<Ghost>()){
			GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

			foreach (GameObject ghost in o){
				ghost.transform.GetComponent<Ghost>().canMove = true;
			}
		}
		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<PacMan>().canMove = true;

		ReadyText.transform.GetComponent<Text>().enabled = false;

		transform.GetComponent<AudioSource>().clip = backgroundAudio_normal;
		transform.GetComponent<AudioSource>().Play();

	}

	public void Restart(){

		int playerLevel = 0;

		if(isPlayerOneUp)
			playerLevel = playerOneLevel;
		else
			playerLevel = playerTwoLevel;

		GameObject.Find ("PacMan").GetComponent<PacMan>().SetDifficultyForLevel(playerLevel);

		List<GameObject> obj;

		if(isPlayerOneUp){
				 	obj = PlayerOneGhostsRemaining;
			}else{
					obj = PlayerTwoGhostsRemaining;
			}

		foreach (GameObject ghost in obj){
			ghost.transform.GetComponent<Ghost> ().SetDifficultyForLevel (playerLevel);
		}

		ReadyText.transform.GetComponent<Text>().enabled = false;

		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<PacMan>().Restart();

		foreach (GameObject ghost in obj){
			ghost.transform.GetComponent<Ghost>().Restart();
		}

		transform.GetComponent<AudioSource>().clip = backgroundAudio_normal;
		transform.GetComponent<AudioSource>().Play();

		didStartDeath = false;
	}
	public void StartDeath() {
		
		if(!didStartDeath){
		//if didStartDeath is false then set it to true.
			if(GameObject.FindObjectOfType<MusicPlayer>()){
				//If Music Player Exists.
				musicplayer.GetComponent<MusicPlayer>().Died(1);
			}
			StopAllCoroutines(); //Stop the blinking of the player text.

			 if(GameMenu.isOnePlayerGame){
			 	
			 	playerOneUp.GetComponent<Text>().enabled = true;

			 } else {

			 	playerOneUp.GetComponent<Text>().enabled = true;
			 	playerTwoUp.GetComponent<Text>().enabled = true;
			 } 

			 GameObject bonusItem = GameObject.Find("bonusItem");

			 if(bonusItem) //If found bonus item, destroy when pacman dies.
			 	Destroy (bonusItem.gameObject);

			consumedGhostScoreText.GetComponent<Text>().enabled = false;

			didStartDeath = true;

			GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
		//add all the ghosts in the scene into an array and for each element reset it.

		foreach (GameObject ghost in o){
			ghost.transform.GetComponent<Ghost>().canMove = false;
			//disable movement.
		}

		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<Animator>().enabled = false;
		//turn off pacman's animator.

		pacMan.transform.GetComponent<PacMan>().canMove = false;

		transform.GetComponent<AudioSource>().Stop(); //Stop playing GameBoard's audio.

		//Set High Score when you die.
		if(playerOneScore > playerTwoScore){
			if(playerOneScore>GameMenu.GetHighScore()){
				GameMenu.High_score = playerOneScore;
				}
		}else{
			if(playerTwoScore>GameMenu.GetHighScore()){
				GameMenu.High_score = playerTwoScore;
				}
		}

		StartCoroutine (ProcessDeathAfter(1));
		}
	}

	IEnumerator ProcessDeathAfter (float delay){

		yield return new WaitForSeconds (delay);
		//wait for 2 realtime seconds before executing what comes next.

		GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
		//add all the ghosts in the scene into an array and for each element reset it.

		foreach (GameObject ghost in o){
			ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
			//makes the ghosts disappear
		}

		StartCoroutine(ProcessDeathAnimation (1.8f));

	}
	IEnumerator ProcessDeathAnimation (float delay){
		GameObject pacMan = GameObject.Find("PacMan");

		pacMan.transform.localScale = new Vector3 (1,1,1);
		pacMan.transform.localRotation = Quaternion.Euler (0,0,0);
		//reset scale and rotation.

		pacMan.transform.GetComponent<Animator>().runtimeAnimatorController = pacMan.transform.GetComponent<PacMan>().PacManDeath;
		pacMan.transform.GetComponent<Animator>().enabled = true;
		//start the death animation.

		if(Random.Range(0,9)>=8){
			transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath;
			transform.GetComponent<AudioSource>().Play();
		}else{
			transform.GetComponent<AudioSource>().clip = backgroundAudioPacManDeath2;
			transform.GetComponent<AudioSource>().Play();
		}

		yield return new WaitForSeconds (delay);
		//wait before starting next coroutine.

		StartCoroutine (ProcessRestart (1));
	}

	IEnumerator ProcessGameOver (float delay){

			if(playerOneScore > playerTwoScore){
				if(playerOneScore>GameMenu.GetHighScore()){
					GameMenu.High_score = playerOneScore;
					}
			}else{
				if(playerTwoScore>GameMenu.GetHighScore()){
					GameMenu.High_score = playerTwoScore;
					}
			}

			GameMenu.SaveHighScore();
			GameMenu.SaveLevels();

			if(playerOneLevel==6||playerTwoLevel==6){

				transform.GetComponent<AudioSource>().clip = Congratulations;
				transform.GetComponent<AudioSource>().Play();

			}else{

				transform.GetComponent<AudioSource>().clip = GameOver;
				transform.GetComponent<AudioSource>().Play();

			}

			yield return new WaitForSeconds (delay);

			GameMenu.playerOnePelletsConsumed = 0;
			GameMenu.playerOneGhostsConsumed = 0;
			playerOneScoreText.text = "0";
			playerOneScore = 0;
			didSpawnBonusItem1_player1 = false;
			didSpawnBonusItem2_player1 = false;


			GameMenu.playerTwoPelletsConsumed = 0;
			GameMenu.playerTwoGhostsConsumed = 0;
			playerTwoScoreText.text = "0";
			playerTwoScore = 0;
			didSpawnBonusItem1_player2 = false;
			didSpawnBonusItem2_player2 = false;

			playerOneLevel=1;
		 	playerTwoLevel=1;

		 	isPlayerTwoFirstTurn = true;
		 	totalPellets = 0;
			ResetPelletsForPlayer (1);
			ResetPelletsForPlayer (2);


			SceneManager.LoadScene("Menu");
	}

	IEnumerator ProcessRestart (float delay){

		if(isPlayerOneUp){
			PlayerOneGhostsRemaining = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ghost"));
			List<GameObject> c = new List<GameObject>(PlayerOneGhostsRemaining);

			foreach(GameObject ghost in c){
				if(!ghost.GetComponent<Ghost>().belongsToPlayerOne){
					//if does not belong to player one, remove it.
					PlayerOneGhostsRemaining.Remove(ghost);

				}
			}
			GameMenu.livesPlayerOne -=1;
			}
		else{
				/*if(isPlayerTwoFirstTurn){
					SpawnGhosts(); //MoveSpawning to different time.
					isPlayerTwoFirstTurn = false;
				}*/

			PlayerTwoGhostsRemaining = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ghost"));
			List<GameObject> c = new List<GameObject>(PlayerTwoGhostsRemaining);

			foreach(GameObject ghost in c){
				if(ghost.GetComponent<Ghost>().belongsToPlayerOne){
					//if it belongs to player one, remove it.
					PlayerTwoGhostsRemaining.Remove(ghost);
				}
			}
			GameMenu.livesPlayerTwo -=1;

			}

		//If pac man dies we don't want the game to restart.
		if(GameMenu.livesPlayerOne <= 0 && GameMenu.livesPlayerTwo <= 0){

		//If both have lost all lives, end the Game.
			playerText.transform.GetComponent<Text>().enabled = true;

			ReadyText.transform.GetComponent<Text>().text = "GAME OVER";

			//RGB METHOD OF CHANGING COLOUR
			//ReadyText.transform.GetComponent<Text>().color = new Color( num/255f, num/255f, num/255f);

			//constant version
			ReadyText.transform.GetComponent<Text>().color = Color.red;

			ReadyText.transform.GetComponent<Text>().enabled = true;

			GameObject pacMan = GameObject.Find("PacMan");
			pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

			transform.GetComponent<AudioSource>().Stop();

			if(GameObject.FindObjectOfType<MusicPlayer>())
				musicplayer.GetComponent<MusicPlayer>().Died(-1);

			StartCoroutine(ProcessGameOver(1.5f));

		} else if (GameMenu.livesPlayerOne <= 0 || GameMenu.livesPlayerTwo <= 0){
			//If either of them have 0 lives but not both.
			if(!GameMenu.isOnePlayerGame){
			//If it's not a one player game, display the game over text.
				if (GameMenu.livesPlayerOne == 0){

					playerText.transform.GetComponent<Text>().text = "PLAYER 1";

				} else if (GameMenu.livesPlayerTwo == 0){

					playerText.transform.GetComponent<Text>().text = "PLAYER 2";
				}

				ReadyText.transform.GetComponent<Text>().text = "GAME OVER";
				ReadyText.transform.GetComponent<Text>().color = Color.red;

				ReadyText.transform.GetComponent<Text>().enabled = true;
				playerText.transform.GetComponent<Text>().enabled = true;

			}
				GameObject pacMan = GameObject.Find ("PacMan");
				pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

				transform.GetComponent<AudioSource>().Stop();

				if(!GameMenu.isOnePlayerGame){
				//If it's not a one player game then wait to display the text.
					yield return new WaitForSeconds (delay);
					}

				if(!GameMenu.isOnePlayerGame)
				//If it is not a one player game, Change player's turn.
					isPlayerOneUp = !isPlayerOneUp;

				if(isPlayerOneUp)
					StartCoroutine (StartBlinking (playerOneUp));
					//Blink player one Text.
				else
					StartCoroutine (StartBlinking (playerTwoUp));

				RedrawBoard ();

				if(isPlayerOneUp)
					playerText.transform.GetComponent<Text>().text = "PLAYER 1";
				else
					playerText.transform.GetComponent<Text>().text = "PLAYER 2";

				ReadyText.transform.GetComponent<Text>().enabled = true;
				playerText.transform.GetComponent<Text>().enabled = true;

				ReadyText.transform.GetComponent<Text>().text = "READY";
				//ReadyText.transform.GetComponent<Text>().color = new Color (240f/255f, 207f/255f, 101f/255f);

				yield return new WaitForSeconds (delay);

				if(GameObject.FindObjectOfType<MusicPlayer>())
					musicplayer.GetComponent<MusicPlayer>().Died(-1);

				StartCoroutine (ProcessRestartShowObjects (2));
				
			}else {
				//if no one is at zero, change turns.

				ReadyText.transform.GetComponent<Text>().enabled = true;

				GameObject pacMan = GameObject.Find("PacMan");
				pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;
				//turn off pacman's sprite renderer.

				transform.GetComponent<AudioSource>().Stop();
				//stop death audio so it doesn't loop.

				if (!GameMenu.isOnePlayerGame)
					isPlayerOneUp = !isPlayerOneUp;

				if(isPlayerOneUp){
					StartCoroutine (StartBlinking (playerOneUp));
					}
				else{
					StartCoroutine (StartBlinking (playerTwoUp));
				}

				if(!GameMenu.isOnePlayerGame){
					if(isPlayerOneUp)
						playerText.transform.GetComponent<Text>().text = "PLAYER 1";
					else
						playerText.transform.GetComponent<Text>().text = "PLAYER 2";
				}

				playerText.transform.GetComponent<Text>().enabled = true;

				RedrawBoard ();

				yield return new WaitForSeconds(delay);

				if(GameObject.FindObjectOfType<MusicPlayer>()){
					musicplayer.GetComponent<MusicPlayer>().Died(-1);
				}

				StartCoroutine (ProcessRestartShowObjects(1));

			}

	}
	IEnumerator ProcessRestartShowObjects(float delay){


		playerText.transform.GetComponent<Text>().enabled = false;

		if(GameObject.FindObjectOfType<Ghost>()){
			if(isPlayerOneUp){

				List<GameObject> o = PlayerOneGhostsRemaining;

				foreach (GameObject ghost in o){
					// make ghosts visible
					ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
					ghost.transform.GetComponent<SpriteRenderer>().enabled = true;

				}
			}else{
			if(!isPlayerTwoFirstTurn){//Do this only if it isn't player two's first game.
				List<GameObject> o = PlayerTwoGhostsRemaining;

				foreach (GameObject ghost in o){
					// make ghosts visible
					ghost.transform.GetComponent<Ghost>().MoveToStartingPosition();
					ghost.transform.GetComponent<SpriteRenderer>().enabled = true;

					}
				} else {
					SpawnGhosts();
					isPlayerTwoFirstTurn = false;
				}
			}
		}
		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

		//make pacman visible.

		yield return new WaitForSeconds (delay);

		Restart();
	}

	IEnumerator ProcessRestartShowObjectsAfterWin(float delay){


		playerText.transform.GetComponent<Text>().enabled = false;

		if(isPlayerOneUp){
			PlayerOneGhostsRemaining = new List<GameObject>(); //clear the array before accessing elements

		}else{
			PlayerTwoGhostsRemaining = new List<GameObject>();
		}

		SpawnGhosts();

		GameObject[] obj = GameObject.FindGameObjectsWithTag ("Ghost");

		foreach (GameObject ghost in obj){
			ghost.transform.GetComponent<Ghost>().canMove = false;
		}
		//Spawn New Ghosts and then freeze them.

		GameObject pacMan = GameObject.Find("PacMan");
		pacMan.transform.GetComponent<PacMan>().MoveToStartingPosition();
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = true;

		//make pacman visible.

		yield return new WaitForSeconds (delay);

		consumedGhostScoreText.GetComponent<Text>().enabled = false;

		Restart();
	}

	// Update is called once per frame
	void Update () {
		CheckPelletsConsumed();
		UpdateUI();
		CheckShouldBlink();
		BonusItems();
	}
	void BonusItems(){
		if(GameMenu.isOnePlayerGame){
			SpawnBonusItemForPlayer (1);
		} else {
			if (isPlayerOneUp){
				SpawnBonusItemForPlayer (1);
			} else {
				SpawnBonusItemForPlayer (2);
			}
		}
	}

	void SpawnBonusItemForPlayer (int playernum){

		if(playernum == 1){

			if(GameMenu.playerOnePelletsConsumed >= 100 && GameMenu.playerOnePelletsConsumed < 200){

				if (!didSpawnBonusItem1_player1){
					didSpawnBonusItem1_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			} else if (GameMenu.playerOnePelletsConsumed >= 200){
				if(!didSpawnBonusItem2_player1){

					didSpawnBonusItem2_player1 = true;
					SpawnBonusItemForLevel (playerOneLevel);
				}
			}
		} else {
			if(GameMenu.playerTwoPelletsConsumed >= 100 && GameMenu.playerTwoPelletsConsumed < 200){

				if (!didSpawnBonusItem1_player2){
					didSpawnBonusItem1_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			} else if (GameMenu.playerTwoPelletsConsumed >= 200){
				if(!didSpawnBonusItem2_player2){

					didSpawnBonusItem2_player2 = true;
					SpawnBonusItemForLevel (playerTwoLevel);
				}
			}
		}
	}

	void SpawnBonusItemForLevel (int level){
		GameObject bonusitem = null;

		bonusitem = Resources.Load ("Prefabs/bonus_apple", typeof(GameObject)) as GameObject;
	
		Instantiate (bonusitem);
	}

	void UpdateUI(){
		playerOneScoreText.text = playerOneScore.ToString();
		playerTwoScoreText.text = playerTwoScore.ToString();

		if(isPlayerOneUp){


			playerLevelText.text = playerOneLevel.ToString();
			playerLivesText.text = GameMenu.livesPlayerOne.ToString();

		}else{

			
			playerLevelText.text =	playerTwoLevel.ToString();
			playerLivesText.text = GameMenu.livesPlayerTwo.ToString();

		}


	}

	void CheckPelletsConsumed (){

		if(isPlayerOneUp){
			//-Player one is playing
			if(totalNormPellets <= GameMenu.playerOnePelletsConsumed){

				GameMenu.playerOnePelletsConsumed = 0; //reset......

				if(GameObject.FindObjectOfType<Ghost>()){
						//If there are ghosts in the game area.
						GameObject[] ghosts = GameObject.FindGameObjectsWithTag ("Ghost");
						//Searches for all objects with the ghost tag and puts them in an array.

						foreach( GameObject go in ghosts){
									go.GetComponent<Ghost>().StartFrightenedMode();
								}
							}
			}
			if(totalGhosts <= GameMenu.playerOneGhostsConsumed){
				PlayerWin(1);
			}
		} else {
			//- Player two is playing
			if(totalNormPellets<= GameMenu.playerTwoPelletsConsumed){
				
				GameMenu.playerTwoPelletsConsumed = 0; //reset num of pellets consumed.

				if(GameObject.FindObjectOfType<Ghost>()){
						GameObject[] ghosts = GameObject.FindGameObjectsWithTag ("Ghost");
						//Searches for all objects with the ghost tag and puts them in an array.
						foreach( GameObject go in ghosts){
								//If there are ghosts in the game area.
									go.GetComponent<Ghost>().StartFrightenedMode();
								}
							}
			}
			if(totalGhosts <= GameMenu.playerTwoGhostsConsumed){
				PlayerWin(2);
			}
		}
	}
	void PlayerWin(int playerNum){

		if(playerNum==1){
			if(!didIncrementLevel){
				IncreaseRank();
				didIncrementLevel = true;
				playerOneLevel++;

				//resets player one's bonus item spawner for the new level.
				StartCoroutine (ProcessWin (2,PlayerOneGhostsRemaining));//Pass in the ghost array.
			}

		} else {
			if(!didIncrementLevel){ //if level has not been incremented before, set to true;
				IncreaseRank();
				didIncrementLevel = true;
				playerTwoLevel++;

				StartCoroutine (ProcessWin (2,PlayerTwoGhostsRemaining));
				}
		} 
	}

	IEnumerator ProcessWin (float delay, List<GameObject> o){
		 GameObject pacMan = GameObject.Find("PacMan");
		 pacMan.transform.GetComponent<PacMan>().canMove = false;
		 pacMan.transform.GetComponent<Animator>().enabled = false;

		 transform.GetComponent<AudioSource>().Stop();

		 transform.GetComponent<AudioSource>().PlayOneShot(WinSound);

		 if(o!=null){
		 //if even one ghost is found. There is never a ghost when you win bruh moment.
			  //o = GameObject.FindGameObjectsWithTag ("Ghost");

			 foreach (GameObject ghost in o){
			 	ghost.transform.GetComponent<Ghost>().canMove = false;
			 	ghost.transform.GetComponent<Animator>().enabled = false;
			 	}
			 }

		 yield return new WaitForSeconds (delay);

		 StartCoroutine (BlinkBoard (2,o));
		 }

	IEnumerator BlinkBoard (float delay, List<GameObject> o){

		transform.GetComponent<AudioSource>().PlayOneShot(YouWin);
		
		GameObject pacMan = GameObject.Find ("PacMan");
		pacMan.transform.GetComponent<SpriteRenderer>().enabled = false;

		 //o = GameObject.FindGameObjectsWithTag ("Ghost");

		foreach(GameObject ghost in o){
			ghost.transform.GetComponent<SpriteRenderer>().enabled = false;

			objects.Remove(ghost);
			ghost.transform.GetComponent<Ghost>().DestroyGhost();
		}

		o = null;

		//-Blink Board
		shouldBlink = true;

		yield return new WaitForSeconds (delay);

		shouldBlink = false;
		StartNextLevel();
		//-Restart the game at the next level
	}

	void ResetPelletsForPlayer (int playerNum){
		Object[] objects = GameObject.FindObjectsOfType (typeof(GameObject));
		//Obtain all GameObjects in the scene.
		foreach (GameObject o in objects){

			if(o.GetComponent<Tile>() != null){

				if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet){

					if(playerNum == 1){
						//Reset the pellets so that another player can continue.
						o.GetComponent<Tile>().didConsumePlayerOne = false;

					}else {

						o.GetComponent<Tile>().didConsumePlayerTwo = false;
					}
				}
			}
		}
	}


	void RedrawBoard () {

		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

		foreach (GameObject o in objects){

			if(o.GetComponent<Tile>() != null){

				if(o.GetComponent<Tile>().isPellet || o.GetComponent<Tile>().isSuperPellet){

					if(isPlayerOneUp){

						if(o.GetComponent<Tile>().didConsumePlayerOne)
						//If player one consumed it, turn it off else turn it on.
							o.GetComponent<SpriteRenderer>().enabled = false;
						else
							o.GetComponent<SpriteRenderer>().enabled = true;

					} else {
					//If Player two consumed it, turn it off...etc
						if(o.GetComponent<Tile>().didConsumePlayerTwo)
							o.GetComponent<SpriteRenderer>().enabled = false;
						else
							o.GetComponent<SpriteRenderer>().enabled = true;
					}
				}
			}
		}
	}

	public void RespawnPellets() {
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

		if(isPlayerOneUp){
			GameMenu.playerOnePelletsConsumed = 0;
		}else{
			GameMenu.playerTwoPelletsConsumed = 0;
		}

		foreach (GameObject o in objects){

			if(o.GetComponent<Tile>() != null){

				if(o.GetComponent<Tile>().isPellet){

					if(isPlayerOneUp){

						if(o.GetComponent<Tile>().didConsumePlayerOne){
						//If was consumed by player turn it back on.
							
							o.GetComponent<Tile>().didConsumePlayerOne = false;
							o.GetComponent<SpriteRenderer>().enabled = true;
							}

					} else {

					//If Player two consumed it, turn it on...etc
						if(o.GetComponent<Tile>().didConsumePlayerTwo){

							o.GetComponent<Tile>().didConsumePlayerTwo = false;
							o.GetComponent<SpriteRenderer>().enabled = true;
							}
					}
				}
			}
		}
	}
	public void SpawnGhosts(){

		GameObject a = Instantiate(Blinky);
		GameObject b = Instantiate(Inky);
		GameObject c = Instantiate(Clyde);
		GameObject d = Instantiate(Pinky);

		objects.Add(a);
		objects.Add(b);
		objects.Add(c);
		objects.Add(d);

		if(isPlayerOneUp){
			a.GetComponent<Ghost>().belongsToPlayerOne = true;
			b.GetComponent<Ghost>().belongsToPlayerOne = true;
			c.GetComponent<Ghost>().belongsToPlayerOne = true;
			d.GetComponent<Ghost>().belongsToPlayerOne = true;

			PlayerOneGhostsRemaining.Add(a);
			PlayerOneGhostsRemaining.Add(b);
			PlayerOneGhostsRemaining.Add(c);
			PlayerOneGhostsRemaining.Add(d);

		}else{
			a.GetComponent<Ghost>().belongsToPlayerOne = false;
			b.GetComponent<Ghost>().belongsToPlayerOne = false;
			c.GetComponent<Ghost>().belongsToPlayerOne = false;
			d.GetComponent<Ghost>().belongsToPlayerOne = false;

			PlayerTwoGhostsRemaining.Add(a);
			PlayerTwoGhostsRemaining.Add(b);
			PlayerTwoGhostsRemaining.Add(c);
			PlayerTwoGhostsRemaining.Add(d);
		}
		/*
		Instantiate(Blinky,Blinky.GetComponent<Ghost>().startingPosition.transform.position,Quaternion.identity);
		Instantiate(Inky,Inky.GetComponent<Ghost>().startingPosition.transform.position,Quaternion.identity);
		Instantiate(Clyde,Clyde.GetComponent<Ghost>().startingPosition.transform.position,Quaternion.identity);
		Instantiate(Pinky,Pinky.GetComponent<Ghost>().startingPosition.transform.position,Quaternion.identity);
		*/
	}
	public void IncreaseRank(){
		//Ranking number is unused at the moment.
		int playerOneLevels = 0;

		if(isPlayerOneUp){
			playerOneLevels =playerOneLevel;
		}else{
			playerOneLevels =playerTwoLevel;
		}
		//This is so that it doesn't matter who wins, the level will count as completed.
		if(GameMenu.Complete1 == 0){

			if(playerOneLevels == 1){

				GameMenu.RankingNumber +=1;
				GameMenu.Complete1 = 1;

			}
		}if(GameMenu.Complete2 == 0){

			if(playerOneLevels == 2){

				GameMenu.RankingNumber +=1;
				GameMenu.Complete2 = 1;

			}
		}if(GameMenu.Complete3 == 0){

			if(playerOneLevels == 3){

				GameMenu.RankingNumber +=1;
				GameMenu.Complete3 = 1;

			}
		}if(GameMenu.Complete4 == 0){

			if(playerOneLevels == 4){

				GameMenu.RankingNumber +=1;
				GameMenu.Complete4 = 1;

				if(musicplayer.GetComponent<MusicPlayer>().AudioArrNum!=2){
					musicplayer.GetComponent<MusicPlayer>().SwitchAudio(2);
					}
			}

		}if(GameMenu.Complete5 == 0){

			if(playerOneLevels == 5){

				GameMenu.RankingNumber +=4;
				GameMenu.Complete5 = 1;

				if(musicplayer.GetComponent<MusicPlayer>().AudioArrNum!=2){
					musicplayer.GetComponent<MusicPlayer>().SwitchAudio(2);
					}
			}
		}
	}
	public void PauseGame(){

		if(isPaused){

			Time.timeScale = 1;
			PauseMenu.SetActive(false);
			isPaused = false;

		}else{
			PauseMenu.SetActive(true);
			isPaused = true;
			Time.timeScale = 0;
		}
	}
}
