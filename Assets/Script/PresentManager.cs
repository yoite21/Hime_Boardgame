using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentManager : MonoBehaviour {

	[SerializeField]
	private Present presentPrefab;

	[SerializeField]
	private GameObject MyPresents;
	[SerializeField]
	private GameObject MySecret;
	[SerializeField]
	private GameObject MyTrash;
	[SerializeField]
	private GameObject OpponentPresents;
	[SerializeField]
	private GameObject OpponentSecret;
	[SerializeField]
	private GameObject OpponentTrash;
	[SerializeField]
	private GameObject LeftPresents;
	[SerializeField]
	private GameObject DeadPresent;

	[SerializeField]
	private Button yesButton;

	[SerializeField]
	private HimeManager himeManager;
	[SerializeField]
	private GameManager gameManager;
	[SerializeField]
	private SelectController selectController;

	private const int NumOfPresent = 21;
	private Present[] presentList = new Present[NumOfPresent];

	private int presentSelectCount;
	private int presentSelectLimit;

	private Present[] presentGroup1Store = new Present[2];
	private Present[] presentGroup2Store = new Present[2];
	private Present PresentGroupStore {
		set { 
			if (presentGroup1Store [0] == null) {
				presentGroup1Store [0] = value;
			} else if (presentGroup1Store [1] == null) {
				presentGroup1Store [1] = value;
			} else if (presentGroup2Store [0] == null) {
				presentGroup2Store [0] = value;
			} else if (presentGroup2Store [1] == null) {
				presentGroup2Store [1] = value;
			}
		}
	}

	void Start()
	{
		// present instantiate
		var himeInfoPool = GameObject.FindWithTag ("HimeManager").GetComponent<HimeManager> ().HimeInfoPool;
		var presentInfoList = new List<KeyValuePair<int, Color>> ();
		foreach (var himeInfo in himeInfoPool) {
			for (int i = 0; i < himeInfo.score; i++) {
				presentInfoList.Add (new KeyValuePair<int, Color> (himeInfo.score, himeInfo.backgroundColor));
			}
		}

		for (var i = 0; i < NumOfPresent; i++) {
			presentList[i] = Instantiate (presentPrefab, Vector3.zero, Quaternion.identity);
			presentList [i].Score = presentInfoList [i].Key;
			presentList [i].BackgroundColor = presentInfoList [i].Value;
			presentList [i].Priority = i;
		}
	}

	public void presentReset()
	{
		// present list random
		for (int i = 0; i < NumOfPresent; i++) {
			int k = Random.Range (i, NumOfPresent);

			Present tmp = presentList [i];
			presentList [i] = presentList [k];
			presentList [k] = tmp;
		}

		// present list share
		const int MyPresentsNum = 6;
		const int OpponentPresentsNum = 6;
		const int LeftPresentsNum = 8;
		const int DeadPresentNum = 1;

		int n = 0;
		sharePresent (LeftPresents, PresentDirection.BACK, LeftPresentsNum, ref n);
		sharePresent (MyPresents, PresentDirection.FRONT, MyPresentsNum, ref n);
		sharePresent (OpponentPresents, PresentDirection.BACK, OpponentPresentsNum, ref n);
		sharePresent (DeadPresent, PresentDirection.BACK, DeadPresentNum, ref n);

		prioritySortInHand ();
		presentGroupReset ();
	}

	public void resetPresentSelectLimit(int value)
	{
		presentSelectCount = 0;
		presentSelectLimit = value;

		unselectPresentsInMyHand ();
		presentGroupReset ();
	}

	public bool isSelectable(Present present)
	{
		if (presentSelectCount >= presentSelectLimit) {
			return false;
		}

		if ((gameManager.CurrentState == GameManager.GameState.PLAYER_TURN && present.transform.parent == MyPresents.transform)
			|| (gameManager.CurrentState == GameManager.GameState.PLAYER_SELECTION && present.transform.parent == OpponentPresents.transform)) {
			return true;
		}

		return false;
	}


	public void selectPresent(Present present)
	{
		if (gameManager.CurrentState == GameManager.GameState.PLAYER_SELECTION) {
			// for selection group
			if (present.IsSelected) {
				increaseSelectCount (-1);

				switch (present.SelectionGroup) {
				case Present.PresentGroup.GROUP0:
					present.SelectionGroup = Present.PresentGroup.UNSELECTED;
					break;
				case Present.PresentGroup.GROUP1:
					setPresentGroupByGroup (Present.PresentGroup.UNSELECTED_GROUP1);
					break;
				case Present.PresentGroup.GROUP2:
					setPresentGroupByGroup (Present.PresentGroup.UNSELECTED_GROUP2);
					break;
				default:
					present.SelectionGroup = Present.PresentGroup.UNSELECTED;
					break;
				}
			} else {
				if (isSelectable (present)) {
					increaseSelectCount (1);

					switch (present.SelectionGroup) {
					case Present.PresentGroup.UNSELECTED:
						present.SelectionGroup = Present.PresentGroup.GROUP0;
						break;
					case Present.PresentGroup.UNSELECTED_GROUP1:
						setPresentGroupByGroup(Present.PresentGroup.GROUP1);
						break;
					case Present.PresentGroup.UNSELECTED_GROUP2:
						setPresentGroupByGroup(Present.PresentGroup.GROUP2);
						break;
						default:
						present.SelectionGroup = Present.PresentGroup.GROUP0;
						break;
					}
				}
			}
		} else {
			if (present.IsSelected) {
				increaseSelectCount (-1);

				present.SelectionGroup = Present.PresentGroup.UNSELECTED;
				removeFromPresentGroupStore (present);
			} else {
				if (isSelectable (present)) {
					increaseSelectCount (1);

					present.SelectionGroup = getTypeOfEmptyPresentGroupStore ();
					PresentGroupStore = present;
				}
			}
		}
	}

	public void increaseSelectCount(int value)
	{
		presentSelectCount += value;
		if (presentSelectCount == presentSelectLimit) {
			yesButton.interactable = true;
		} else {
			yesButton.interactable = false;
		}
	}

	public void actionPresent()
	{
		if (gameManager.CurrentState == GameManager.GameState.PLAYER_SELECTION) {

			List<Present> opponentPresentList;
			List<Present> myPresentList;

			divideSelectPresentInOpponent (out opponentPresentList, out myPresentList);
			unselectPresentsInList (myPresentList);
			unselectPresentsInList (opponentPresentList);

			himeManager.presentToHime (myPresentList, opponentPresentList);

		} else {
			List<Present> opponentPresentList;
			var presentList = getSelectedPresentInMyHand ();
			unselectPresentsInList (presentList);


			switch (presentSelectCount) {
			case 1:
				presentToSecret (presentList [0]);
				break;
			case 2:
				presentToDead (presentList);
				break;
			case 3:
			
				selectController.chooseOnePresent (presentList, out opponentPresentList);
				himeManager.presentToHime (presentList, opponentPresentList);
				break;
			case 4:

				selectController.choosePresentSet (ref presentGroup1Store, ref presentGroup2Store);
				himeManager.presentToHime (new List<Present> (presentGroup1Store), new List<Present> (presentGroup2Store));
				break;
			default:
				Debug.Log ("do action nothing");
				break;
			}
		}
	}


	public void drawToHand()
	{
		var drawPresent = LeftPresents.transform.GetChild (0);

		drawPresent.GetComponent<Present> ().PresentDirection = PresentDirection.FRONT;
		drawPresent.SetParent (MyPresents.transform);
		prioritySortInHand ();
	}

	public void drawToOpponent()
	{
		LeftPresents.transform.GetChild (0).SetParent (OpponentPresents.transform);
	}

	public List<Present> getOpponentPresentInHand()
	{
		List<Present> retv = new List<Present> ();
		for (int i = 0; i < OpponentPresents.transform.childCount; i++) {
			retv.Add (OpponentPresents.transform.GetChild (i).gameObject.GetComponent<Present> ());
		}

		return retv;
	}
		
	public void setPresentGroupStore(List<Present> list)
	{
		if (list.Count == 4) {
			foreach (var present in list) {
				PresentGroupStore = present;
			}
		}
	}

	public void presentToOpponentSecret(Present present)
	{
		present.transform.SetParent (OpponentSecret.transform);
	}

	public void presentToOpponentTrash(List<Present> list)
	{
		foreach (var present in list) {
			present.transform.SetParent (OpponentTrash.transform);
		}
	}

	public void presentToPlayerSelect(List<Present> list)
	{
		presentSelectCount = 0;
		presentSelectLimit = 1;

		foreach (var present in list) {
			present.PresentDirection = PresentDirection.FRONT;
		}

		setOpponentPresentGroup ();
	}

	private void sharePresent(GameObject parent, PresentDirection direction, int count, ref int listIndex)
	{
		for (int i = 0; i < count; i++) {
			presentList [listIndex].transform.SetParent (parent.transform);
			presentList [listIndex].PresentDirection = direction;
			listIndex++;
		}
	}

	private void unselectPresentsInMyHand()
	{
		for (int i = 0; i < MyPresents.transform.childCount; i++) {
			MyPresents.transform.GetChild (i).gameObject.GetComponent<Present> ().SelectionGroup = Present.PresentGroup.UNSELECTED;
		}
	}

	private void unselectPresentsInList(List<Present> list)
	{
		foreach (var present in list) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED;
		}
	}

	private void setPresentGroupByGroup(Present.PresentGroup group)
	{
		Present[] tmp;
		if ( (group == Present.PresentGroup.UNSELECTED_GROUP1) 
			|| (group == Present.PresentGroup.GROUP1)) {
			tmp = presentGroup1Store;
		} else {
			tmp = presentGroup2Store;
		}

		foreach (var present in tmp) {
			present.SelectionGroup = group;
		}
	}

	private void divideSelectPresentInOpponent(out List<Present> opponentList, out List<Present> myList)
	{
		opponentList = new List<Present>();
		myList = new List<Present>();
		for (int i = 0; i < OpponentPresents.transform.childCount; i++) {
			Present t = OpponentPresents.transform.GetChild (i).gameObject.GetComponent<Present> ();
			if (t.PresentDirection == PresentDirection.FRONT) {
				if( t.IsSelected ) {
					myList.Add (t);
				} else {
					opponentList.Add (t);
				}
			}
		}
	}

	private List<Present> getSelectedPresentInMyHand()
	{
		List<Present> retv = new List<Present> ();
		for (int i = 0; i < MyPresents.transform.childCount; i++) {
			Present t = MyPresents.transform.GetChild (i).gameObject.GetComponent<Present> ();
			if (t.IsSelected) {
				retv.Add (t);
			}
		}

		return retv;
	}

	private void presentToSecret(Present present)
	{
		present.transform.SetParent (MySecret.transform);
	}

	private void presentToDead(List<Present> presentList)
	{
		foreach (var present in presentList) {
			present.transform.SetParent (MyTrash.transform);
		}
	}

	private void setOpponentPresentGroup()
	{
		if (presentGroup1Store [0] == null) {
			return;
		}

		foreach (var present in presentGroup1Store) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP1;
		}

		foreach (var present in presentGroup2Store) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP2;
		}
	}

	private void presentGroupReset()
	{

		for (int i = 0; i < presentGroup1Store.Length; i++) {
			presentGroup1Store [i] = null;
		}

		for (int i = 0; i < presentGroup2Store.Length; i++) {
			presentGroup2Store [i] = null;
		}
	}

	private void prioritySortInHand()
	{
		for (int i = 0; i < MyPresents.transform.childCount; i++) {
			int minPriority = NumOfPresent;
			int minIndex=0;

			for (int j = 0; j < MyPresents.transform.childCount-i; j++) {
				var t = MyPresents.transform.GetChild (j).gameObject.GetComponent<Present> ().Priority;
				if (t < minPriority) {
					minPriority = t;
					minIndex = j;
				}
			}

			MyPresents.transform.GetChild (minIndex).SetAsLastSibling();
		}
	}

	private void removeFromPresentGroupStore(Present present)
	{
		for (int i = 0; i < presentGroup1Store.Length; i++) {
			if (presentGroup1Store [i] == present) {
				presentGroup1Store [i] = null;
				return;
			}
		}

		for (int i = 0; i < presentGroup2Store.Length; i++) {
			if (presentGroup2Store [i] == present) {
				presentGroup2Store [i] = null;
				return;
			}
		}

	}

	private Present.PresentGroup getTypeOfEmptyPresentGroupStore()
	{
		if (isPresentGroupSelectInMyHand ()) {
			foreach (var present in presentGroup1Store) {
				if (present == null) {
					return Present.PresentGroup.GROUP1;
				}
			}

			foreach (var present in presentGroup2Store) {
				if (present == null) {
					return Present.PresentGroup.GROUP2;
				}
			}
			return Present.PresentGroup.UNSELECTED;
		} else {
			return Present.PresentGroup.GROUP0;
		}
	}

	private bool isPresentGroupSelectInMyHand()
	{
		return presentSelectLimit == 4;
	}


}
