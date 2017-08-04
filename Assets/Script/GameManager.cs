using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ActionState { NONE, ACTION1, ACTION2, ACTION3, ACTION4 };

public class GameManager : MonoBehaviour {

	[SerializeField]
	private PresentManager presentManager;
	[SerializeField]
	private HimeManager himeManager;
	[SerializeField]
	private SelectController selectController;
	[SerializeField]
	private ButtonManager buttonManager;

	public enum GameState { PLAYER_TURN, PLAYER_SELECTION, OPPONENT_TURN, OPPONENT_SELECTION};
	public enum NGameState { PlayerTurn, PlayerSelect, OpponentTurn, OpponentSelect}; 
	private GameState currentState;
	public GameState CurrentState {
		set { currentState = value; }
		get { return currentState; }
	}

	void Awake() {
		himeManager.heartReset();
		presentManager.presentReset ();
		selectController.actionPoolReset ();
	}

	void Start() {
		fullInit ();
	}

	public void fullInit()
	{
		himeManager.heartReset();
		presentManager.presentReset ();
		selectController.actionPoolReset ();


		// TODO
		// random player turn start or opponent turn start
		buttonManager.nextButtonActive();
//		playerTurnStart ();
	}

	public void softReset()
	{
		presentManager.presentReset ();
		selectController.actionPoolReset ();
		buttonManager.resetActionButton ();

		playerTurnStart ();
		// TODO
		// player turn or opponent turn start by before round
	}

	public void playerTurnStart()
	{
		currentState = GameState.PLAYER_TURN;
		presentManager.drawToHand ();

	}

	private void opponentTurnStart()
	{
		currentState = GameState.OPPONENT_TURN;
		presentManager.drawToOpponent ();
		selectController.doOpponentTurn ();
		if (currentState != GameState.PLAYER_SELECTION) {
//			nextTurn ();
		}
	}

	private bool endCheck ()
	{
		if (presentManager.IsLeftPresentEmpty ()) {
			return true;
		} else {
			return false;
		}
	}

	public void nextTurn()
	{

		Debug.Log ("next turn call");
		if (currentState == GameState.PLAYER_SELECTION) {
			return;
		}

		if (endCheck ()) {
			Debug.Log ("end");

			scoringRound ();
			return;
		} 

		if (currentState == GameState.PLAYER_TURN) {
			opponentTurnStart ();
		} else {
			playerTurnStart ();
		}
	}

	public void yesButtonClicked()
	{
		nextTurn ();
	}

	private void scoringRound()
	{
		presentManager.secretToHime ();
		himeManager.moveHeart ();
		int myScore = 0;
		int myHeart = 0;
		int opponentHeart = 0;
		int opponentScore = 0;

		himeManager.getScore (out myScore, out myHeart, out opponentScore, out opponentHeart);

		Debug.Log ("my score : " + myScore + ", my heart count : " + myHeart);
		Debug.Log ("opp score : " + opponentScore + ", opp heart count : " + opponentHeart);

		if (myScore >= 11) {
			Debug.Log ("you win!");
		} else if (opponentScore >= 11) {
			Debug.Log ("you lose");
		} else if (myHeart >= 4) {
			Debug.Log ("you win!");
		} else if (opponentHeart >= 4) {
			Debug.Log ("you lose");
		} else {
			buttonManager.nextButtonActive ();
		}
	}
}
