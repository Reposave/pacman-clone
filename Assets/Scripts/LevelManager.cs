using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public float splashscreenduration = 2f;

	// Use this for initialization
	void Start () {

		if (splashscreenduration <= 0f){
			Debug.Log ("Level auto load disabled");
		}
		else{
			Invoke ("LoadNextLevel", splashscreenduration);
		}

	}
	void Awake () {
		DontDestroyOnLoad(gameObject);
		Debug.Log("Don't destroy on load: " + name); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void LoadLevel(string name){
		Application.LoadLevel (name);
	}

	public void LoadNextLevel() {
		
		Application.LoadLevel(Application.loadedLevel + 1);
	}
}
