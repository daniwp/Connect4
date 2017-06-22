using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private States curGameState;
	public bool isPlayerOne;
	private BrickManager bc;
	public Text winText;
	public float waitfor;
	public Text buttonText;

	void Start () {
		curGameState = States.startGame;	
		bc = GetComponent<BrickManager> ();
	}
	
	void Update () {
		switch (curGameState) {
		case States.startGame:
			if (Random.value > 0.5f) {
				isPlayerOne = false;
			} else {
				isPlayerOne = true;
			}
			curGameState = States.playerturn;
			break;
		case States.playerturn:
			if (!bc.isActive) {
				bc.StartPlayerTurn (isPlayerOne);
			}
			break;
		case States.calculationStep:
			break;
		case States.victory:
			winText.text = "Player " + (isPlayerOne ? "1" : "2") + " won!";
			Time.timeScale = 0.5f;
			break;
		}
	}

	public IEnumerator endPlayerTurn() {
		curGameState = States.calculationStep;
		yield return new WaitForSeconds (waitfor);
		curGameState = States.playerturn;
		isPlayerOne = !isPlayerOne;
	}

	public void setVictory(bool p1Won) {
		isPlayerOne = p1Won;
		curGameState = States.victory;
	}
}
