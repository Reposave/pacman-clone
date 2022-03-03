using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicPlayer : MonoBehaviour {
	public  AudioClip[] AudioArray;
	public int AudioArrNum = 0;

	private AudioSource audiosource;
	private AudioReverbFilter audioReverber;
	// Use this for initialization

	void Start () {
		audiosource = this.GetComponent<AudioSource>();
		audioReverber = this.GetComponent<AudioReverbFilter>();

		audiosource.clip =	AudioArray[0];
		audiosource.Play();
	}

	void Awake () {
		DontDestroyOnLoad(gameObject);


  		Debug.logger.logEnabled = false;

		Debug.Log("Don't destroy on load: " + name);


	}
	// Update is called once per frame
	void Update () {

	}

	public void SwitchAudio(int num){
		
		AudioArrNum = num;
		audiosource.clip = AudioArray[num];
		audiosource.loop = true;
		audiosource.Play();
	}
	public void Died(int num){
		//-1 plays Alive Audio, +1 Plays Death Audio.
		if(num==1){
			audiosource.volume = 0.2f;
			audioReverber.enabled = true;
		}else{
			audioReverber.enabled = false;
			audiosource.volume = 1f;
		}
	}
}
