using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	[SerializeField]
	private PresentManager presentManager;
	[SerializeField]
	private GameManager gameManager;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private Button yesButton;

	[SerializeField]
	private GameObject actionButtons;

	[SerializeField]
	private Button nextButton;

	private Button lastActionButton = null;
	public Button LastActionButton {
		get { return lastActionButton; }
	}

	public void yesButtonClicked()
	{
		
		presentManager.actionPresent ();
		resetSelectState (false);
	}

	public void cancelButtonClicked()
	{
		resetSelectState (true);
	}

	public void actionButtonClicked(Button button)
	{
		if (gameManager.CurrentState == GameManager.GameState.PLAYER_TURN) {
			resetSelectState (true);

			cancelButton.interactable = true;
			button.interactable = false;
			lastActionButton = button;
		}
	}

	private void resetSelectState(bool resetLastActionButton)
	{
		yesButton.interactable = false;
		cancelButton.interactable = false;
		if (lastActionButton && resetLastActionButton) {
			lastActionButton.interactable = true;
		}
		lastActionButton = null;

		presentManager.resetPresentSelectLimit (0);
	}

	public void actionButtonSetValue(int value)
	{
		if (gameManager.CurrentState == GameManager.GameState.PLAYER_TURN) {
			presentManager.resetPresentSelectLimit (value);
		}
	}

	public void resetActionButton()
	{
		for (int i = 0; i < actionButtons.transform.childCount; i++) {
			var button = actionButtons.transform.GetChild (i).gameObject.GetComponent<Button> ();
			button.interactable = true;
		}
	}

	public void nextButtonClicked()
	{
		nextButton.interactable = false;
		gameManager.playerTurnStart ();
//		gameManager.softReset ();
	}

	public void nextButtonActive()
	{
		nextButton.interactable = true;
	}
}
