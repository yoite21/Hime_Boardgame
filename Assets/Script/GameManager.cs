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

	private enum GameState { PLAYER_TURN, PLAYER_SELECTION, OPPONENT_TURN, OPPONENT_SELECTION};
	private GameState gameState;


	private bool isPlayerTurn = true;

	private bool isPlayerSelect = false;
	public bool IsPlayerSelect {
		set { isPlayerSelect = value; }
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
		gameState = GameState.PLAYER_TURN;
		isPlayerTurn = true;
		presentManager.drawToHand ();

	}

	private void opponentTurnStart()
	{
		isPlayerTurn = false;
		presentManager.drawToOpponent ();
		selectController.doOpponentTurn ();
		if (!isPlayerSelect) {
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

		if (isPlayerTurn) {
			opponentTurnStart ();
		} else {
			playerTurnStart ();
		}
	}

	public void yesButtonClicked()
	{
		if (isPlayerSelect) {
			isPlayerSelect = false;
		} else {
			nextTurn ();
		}
	}
}
