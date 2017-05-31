using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour {

	[SerializeField]
	private PresentManager presentManager;
	[SerializeField]
	private GameManager gameManager;

	public void chooseOnePresent(List<Present> presentList, out List<Present> opponentPresent)
	{
		int randomIndex = Random.Range (0, presentList.Count);
		opponentPresent = new List<Present> ();
		opponentPresent.Add (presentList [randomIndex]);

		presentList.RemoveAt (randomIndex);
	}

	public void choosePresentSet(ref Present[] presentList, ref Present[] opponentPresent)
	{
		if (Random.value < 0.5) {
			Present[] tmp = opponentPresent;
			opponentPresent = presentList;
			presentList = tmp;
		}
	}


	private List<int> actionPool;

	public void actionPoolReset ()
	{
//		actionPool = new List<int>{ 1, 2, 3, 4 };

		//		actionPool = new List<int>{ 1,2,1,2,1 };
				actionPool = new List<int>{ 4,4,4,4,4 };
	}

	public void doOpponentTurn()
	{
		int i = Random.Range (0, actionPool.Count);
		var action = actionPool [i];
		actionPool.RemoveAt (i);

		Debug.Log ("opponent do action : " + action);
		var list = presentManager.selectRandomOpponentPresent (action);
		switch (action) {
		case 1:
			presentManager.presentToOpponentSecret (list [0]);
			break;
		case 2:
			presentManager.presentToOpponentTrash (list);
			break;
		case 3:
			presentManager.presentToPlayerSelect (list);
			gameManager.CurrentState = GameManager.GameState.PLAYER_SELECTION;
			break;
		case 4:
			presentManager.presentToPlayerSelect (list);
			gameManager.CurrentState = GameManager.GameState.PLAYER_SELECTION;
			break;
		}
	}
}
