using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

	public static bool isOnePlayerGame = true;

	public static int Complete1 = 0;
	public static int Complete2 = 0;
	public static int Complete3 = 0;
	public static int Complete4 = 0;
	public static int Complete5 = 0;

	const string COMPLETE_1 = "complete_1";
	const string COMPLETE_2 = "complete_2";
	const string COMPLETE_3 = "complete_3";
	const string COMPLETE_4 = "complete_4";
	const string COMPLETE_5 = "complete_5";

	public static float High_score = 0;
	const string HIGH_SCORE = "high_score";


	public static int livesPlayerOne;
	public static int livesPlayerTwo;

	public static int playerOnePelletsConsumed = 0;
	public static int playerTwoPelletsConsumed = 0;
	public static int playerOneGhostsConsumed = 0;
	public static int playerTwoGhostsConsumed = 0;

	public static int RankingNumber = 0;

	public Text playerText1;
	public Text playerText2;
	public Text playerSelector;
	public Text Ranking;
	public Text Title;

	public Image Level1;
	public Image Level2;
	public Image Level3;
	public Image Level4;
	public Image Level5;

	public GameObject Effects;

	public Text StageSelector;
	public Text High_Scr;

	public GameObject musicplayer;

	private bool isPaused = false; //This is actually for the credits. Code has been copied.
	public GameObject PauseMenu;


	// Update is called once per frame
	void Start(){

		musicplayer = GameObject.Find("MusicPlayer");

		GetLevels();
		High_score = GetHighScore();
		High_Scr.text = GetHighScore().ToString();

		int x = 0;

		if(Complete1 == 1){
			Ranking.text = "XANTHIC";
			x+=1;
			Level1.enabled = true;

		}if(Complete2 == 1){
			Ranking.text = "SPIZZERINCTUM";
			x+=1;
			Level2.enabled = true;

		}if(Complete3 == 1){
			Ranking.text = "SAGACIOUS";
			x+=1;
			Level3.enabled = true;

		}if(Complete4 == 1){
			Ranking.text = "ICHOR";
			x+=1;
			Level4.enabled = true;

			if(musicplayer.GetComponent<MusicPlayer>().AudioArrNum!=2){
				musicplayer.GetComponent<MusicPlayer>().SwitchAudio(2);
				}

		}if(Complete5 == 1){
			Ranking.text = "ZENITH";
			x+=1;

		}if(x == 5){
			Ranking.text = "THANK YOU FRIEND";
			Title.text = "IT WASN'T JUST A PAC MAN GAME";
			Effects.SetActive(false);
			Level5.enabled = true;

		}

		if(musicplayer){
			if(musicplayer.GetComponent<MusicPlayer>().AudioArrNum!=2){
				if(musicplayer.GetComponent<MusicPlayer>().AudioArrNum!=1){
					musicplayer.GetComponent<MusicPlayer>().SwitchAudio(1);
					}
				}
			}

		SavePlayerPrefs();
	}

	void Update () {
		if(Input.GetKeyUp(KeyCode.UpArrow)){

			if(!isOnePlayerGame){

				isOnePlayerGame = true;


				//moves the chevron's y position to match the text position.
			}
			playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x,playerText1.transform.localPosition.y,playerSelector.transform.localPosition.z);
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow)){

			if(isOnePlayerGame){
			//if it is a one player game.
				isOnePlayerGame = false;


			}
			playerSelector.transform.localPosition = new Vector3(playerSelector.transform.localPosition.x,playerText2.transform.localPosition.y,playerSelector.transform.localPosition.z);
		}
		else if (Input.GetKeyUp(KeyCode.Return)){
			if(!isPaused){
			//If credits menu isn't open.
				livesPlayerOne = 3;
				livesPlayerTwo = 3;

				if(isOnePlayerGame){
					livesPlayerTwo=0;
				}
			 	SceneManager.LoadScene("Level1");
		 	}
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow)){
			if(GameBoard.playerOneLevel>1){

				GameBoard.playerOneLevel-=1;
				GameBoard.playerTwoLevel-=1;

				StageSelector.text = GameBoard.playerOneLevel.ToString();
			}
		}else if (Input.GetKeyUp(KeyCode.RightArrow)){
			if(GameBoard.playerOneLevel<5){

				GameBoard.playerOneLevel+=1;
				GameBoard.playerTwoLevel+=1;

				StageSelector.text = GameBoard.playerOneLevel.ToString();
			}

		}else if(Input.GetKeyUp(KeyCode.Escape)){
			if(!isPaused){
				Application.Quit();
			}else{
				PauseGame();
				}

		}else if(Input.GetKeyUp(KeyCode.R)){

			//Resets All your progress.
			ResetLevels();
			ResetHighScore();

			Level1.enabled = false;
			Level2.enabled = false;
			Level3.enabled = false;
			Level4.enabled = false;
			Level5.enabled = false;

			Ranking.text = "";
			Title.text = "\"IT'S JUST A PAC MAN GAME\"";

			if(musicplayer){
				musicplayer.GetComponent<MusicPlayer>().AudioArrNum=3;
				}

			Start();

		}else if(Input.GetKeyUp(KeyCode.C)){
			PauseGame();
		}
	}

	public static void SaveLevels(){
		PlayerPrefs.SetInt(COMPLETE_1,Complete1);
		PlayerPrefs.SetInt(COMPLETE_2,Complete2);
		PlayerPrefs.SetInt(COMPLETE_3,Complete3);
		PlayerPrefs.SetInt(COMPLETE_4,Complete4);
		PlayerPrefs.SetInt(COMPLETE_5,Complete5);
	}

	public static void ResetLevels(){
		PlayerPrefs.SetInt(COMPLETE_1,0);
		PlayerPrefs.SetInt(COMPLETE_2,0);
		PlayerPrefs.SetInt(COMPLETE_3,0);
		PlayerPrefs.SetInt(COMPLETE_4,0);
		PlayerPrefs.SetInt(COMPLETE_5,0);
	}

	public static void SaveHighScore(){
		PlayerPrefs.SetFloat(HIGH_SCORE, High_score);
	}

	public static void ResetHighScore(){
		PlayerPrefs.SetFloat(HIGH_SCORE, 0);
	}
	public static float GetHighScore(){
		return PlayerPrefs.GetFloat(HIGH_SCORE);
	}
	public static void GetLevels(){
		Complete1 = PlayerPrefs.GetInt(COMPLETE_1);
		Complete2 = PlayerPrefs.GetInt(COMPLETE_2);
		Complete3 = PlayerPrefs.GetInt(COMPLETE_3);
		Complete4 = PlayerPrefs.GetInt(COMPLETE_4);
		Complete5 = PlayerPrefs.GetInt(COMPLETE_5);
	}
	public static void SavePlayerPrefs(){
		PlayerPrefs.Save();
	}
	/*void Awake () {
		DontDestroyOnLoad(gameObject);
		Debug.Log("Don't destroy on load: " + name); 
	}*/
	public void PauseGame(){
	//This is actually for the credits.
		if(isPaused){

			PauseMenu.SetActive(false);
			isPaused = false;

		}else{
			PauseMenu.SetActive(true);
			isPaused = true;
		}
	}
}
