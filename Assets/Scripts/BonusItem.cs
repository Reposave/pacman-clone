using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : MonoBehaviour {

	float RandomLifeExpectancy;
	float currentLifeTime;

	// Use this for initialization
	void Start () {

		RandomLifeExpectancy = Random.Range(9f,10f);

		this.name = "bonusItem"; //Changing the name in the heirachy when instantiated.

		GameObject.Find ("Game").GetComponent<GameBoard>().board [14, 13] = this.gameObject;
		//replace this pellet or entry with the bonus item.
	}
	
	// Update is called once per frame
	void Update () {

		if (currentLifeTime < RandomLifeExpectancy){

			currentLifeTime += Time.deltaTime;

		} else {

			Destroy (this.gameObject);
		}
	}
}
