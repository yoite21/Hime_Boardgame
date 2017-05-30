﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {

	[SerializeField]
	private PresentManager presentManager;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	private Button yesButton;

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
		resetSelectState (true);

		cancelButton.interactable = true;
		button.interactable = false;
		lastActionButton = button;
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
		presentManager.resetPresentSelectLimit (value);
	}

}
