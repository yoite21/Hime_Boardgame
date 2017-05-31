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
		presentSetReset ();
	}

	private void sharePresent(GameObject parent, PresentDirection direction, int count, ref int listIndex)
	{
		for (int i = 0; i < count; i++) {
			presentList [listIndex].transform.SetParent (parent.transform);
			presentList [listIndex].PresentDirection = direction;
			listIndex++;
		}
	}

	private int presentSelectCount;
	private int presentSelectLimit;

	public void resetPresentSelectLimit(int value)
	{
		presentSelectCount = 0;
		presentSelectLimit = value;

		unselectMyPresents ();
		presentSetReset ();
	}

	private void unselectMyPresents()
	{
		for (int i = 0; i < MyPresents.transform.childCount; i++) {
			MyPresents.transform.GetChild (i).gameObject.GetComponent<Present> ().SelectionGroup = Present.PresentGroup.UNSELECTED;
		}
	}

	private void unselectPresentList(List<Present> list)
	{
		foreach (var present in list) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED;
		}
	}

	public bool isSelectable()
	{
		return presentSelectCount < presentSelectLimit;
	}

	public bool isSelectableState()
	{
		return presentSelectCount < presentSelectLimit;
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
					present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP1;
					break;
				case Present.PresentGroup.GROUP2:
					present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP2;
					break;
				default:
					present.SelectionGroup = Present.PresentGroup.UNSELECTED;
					break;
				}
				unselectFromPresentSet (present);
			} else {
				if (isSelectable (present)) {
					increaseSelectCount (1);

					present.SelectionGroup = getPresentGroupType ();
					PresentSet = present;
				}
			}
		} else {
			if (present.IsSelected) {
				increaseSelectCount (-1);

				present.SelectionGroup = Present.PresentGroup.UNSELECTED;
				unselectFromPresentSet (present);
			} else {
				if (isSelectable (present)) {
					increaseSelectCount (1);

					present.SelectionGroup = getPresentGroupType ();
					PresentSet = present;
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

			getOpponentSelectedPresent (out opponentPresentList, out myPresentList);
			unselectPresentList (myPresentList);

			switch (presentSelectCount) {
			case 1:
				himeManager.presentToHime (myPresentList, opponentPresentList);
				break;
			case 2:
				himeManager.presentToHime (myPresentList, opponentPresentList);
				break;
			default:
				break;
			}

		} else {
			List<Present> opponentPresentList;
			var presentList = getSelectedPresent ();
			unselectPresentList (presentList);


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

				selectController.choosePresentSet (ref present1Set, ref present2Set);
				himeManager.presentToHime (new List<Present> (present1Set), new List<Present> (present2Set));
				break;
			default:
				Debug.Log ("do action nothing");
				break;
			}
		}
	}

	private void getOpponentSelectedPresent(out List<Present> opponentList, out List<Present> myList)
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

	private List<Present> getSelectedPresent()
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

	public enum presentSetSelectType { NONE, SET1, SET2 };
	private Present[] present1Set = new Present[2];
	private Present[] present2Set = new Present[2];

	public Present PresentSet {
		set { 
			if (present1Set [0] == null) {
				present1Set [0] = value;
			} else if (present1Set [1] == null) {
				present1Set [1] = value;
			} else if (present2Set [0] == null) {
				present2Set [0] = value;
			} else if (present2Set [1] == null) {
				present2Set [1] = value;
			}
		}
	}

	private void setOpponentPresentGroup()
	{
		if (present1Set [0] == null) {
			return;
		}

		foreach (var present in present1Set) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP1;
		}

		foreach (var present in present2Set) {
			present.SelectionGroup = Present.PresentGroup.UNSELECTED_GROUP2;
		}
	}

	public void unselectFromPresentSet(Present present)
	{
		for (int i = 0; i < present1Set.Length; i++) {
			if (present1Set [i] == present) {
				present1Set [i] = null;
				return;
			}
		}

		for (int i = 0; i < present2Set.Length; i++) {
			if (present2Set [i] == present) {
				present2Set [i] = null;
				return;
			}
		}

	}

	public Present.PresentGroup getPresentGroupType()
	{
		if (isPresentSetSelect ()) {
			foreach (var present in present1Set) {
				if (present == null) {
					return Present.PresentGroup.GROUP1;
				}
			}

			foreach (var present in present2Set) {
				if (present == null) {
					return Present.PresentGroup.GROUP2;
				}
			}
			return Present.PresentGroup.UNSELECTED;
		} else {
			return Present.PresentGroup.GROUP0;
		}
	}

	public presentSetSelectType getSelectType()
	{

		foreach (var present in present1Set) {
			if (present == null) {
				return presentSetSelectType.SET1;
			}
		}

		foreach (var present in present2Set) {
			if (present == null) {
				return presentSetSelectType.SET2;
			}
		}
		return presentSetSelectType.NONE;
	}

	private void presentSetReset()
	{

		for (int i = 0; i < present1Set.Length; i++) {
			present1Set [i] = null;
		}

		for (int i = 0; i < present2Set.Length; i++) {
			present2Set [i] = null;
		}
	}

	public bool isPresentSetSelect()
	{
		return (presentSelectLimit == 4) || (present1Set[0] != null);
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

	public List<Present> selectRandomOpponentPresent(int count)
	{

		List<int> index = new List<int> ();
		for (int i = 0; i < OpponentPresents.transform.childCount; i++) {
			index.Add (i);
		}

		List<Present> retv = new List<Present> ();
		for (int i = 0; i < count; i++) {
			var randomIndex = Random.Range (0, index.Count);
			retv.Add (OpponentPresents.transform.GetChild (index[randomIndex]).gameObject.GetComponent<Present> ());
			if (count == 4) {
				PresentSet = retv [retv.Count - 1];
			}
			index.RemoveAt (randomIndex);
		}

		return retv;
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
}
