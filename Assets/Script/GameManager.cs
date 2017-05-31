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

	public enum GameState { PLAYER_TURN, PLAYER_SELECTION, OPPONENT_TURN, OPPONENT_SELECTION};
	private GameState currentState;
	public GameState CurrentState {
		set { currentState = value; }
		get { return currentState; }
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
		playerTurnStart ();
	}

	public void softReset()
	{
		presentManager.presentReset ();
		selectController.actionPoolReset ();

		// TODO
		// player turn or opponent turn start by before round
	}

	private void playerTurnStart()
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
			nextTurn ();
		}
	}

	private bool endCheck ()
	{
		// TODO
		return false;
	}

	public void nextTurn()
	{
		if (endCheck ()) {
			// TODO
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
}
