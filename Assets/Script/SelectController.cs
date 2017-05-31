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
		actionPool = new List<int>{ 1, 2, 3, 4 };
	}

	public void doOpponentTurn()
	{
		int i = Random.Range (0, actionPool.Count);
		var action = actionPool [i];
		actionPool.RemoveAt (i);

		Debug.Log ("opponent do action : " + action);

		var presentList = presentManager.getOpponentPresentInHand ();
		var list = chooseRandomPresentList (presentList, action);
		presentManager.setPresentGroupStore (list);

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

	private List<Present> chooseRandomPresentList(List<Present> presentList, int count)
	{
		var index = new List<int> ();
		for (int i = 0; i < presentList.Count; i++) {
			index.Add (i);
		}
		
		List<Present> retv = new List<Present> ();
		for (int i = 0; i < count; i++) {
			var randomIndex = Random.Range (0, index.Count);
			retv.Add (presentList [index [randomIndex]]);

			index.RemoveAt (randomIndex);
		}

		return retv;
	}
}
