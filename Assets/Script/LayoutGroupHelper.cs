using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupHelper : MonoBehaviour {

	[SerializeField]
	private int startPosition;

	private TextAnchor childAlignment;
	private HorizontalOrVerticalLayoutGroup layoutGroup;

	private int childCount = 0;

	void Awake() {
		layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup> ();
		if (layoutGroup) {
			childAlignment = layoutGroup.childAlignment;
		} else {
			Debug.Log ("no layout group");
		}
	}


	public void getNextPosition(int count, out Vector3[] positions) {
		positions = new Vector3[count];

		switch (childAlignment) {

		case TextAnchor.UpperCenter:
			// my secret, trash, opponent secret, opponent hand ( vertical ) 
			// my present in hime ( vertical ) 
			{
				float space = layoutGroup.spacing;
				int topPadding = layoutGroup.padding.top;

				for (int i = 0; i < count; i++) {
					positions [i] = transform.position;
					positions [i].y = positions [i].y + startPosition - (25 + (50 + space) * childCount + topPadding + (50 + space) * i);
				}
			}
			break;
		case TextAnchor.LowerCenter:
			{
				// opponent present in hime ( vertical )
				float space = layoutGroup.spacing;
				int bottomPadding = layoutGroup.padding.bottom;

				for (int i = 0; i < count; i++) {
					positions [i] = transform.position;
					positions [i].y = positions [i].y - startPosition + (25 + (50 + space) * childCount + bottomPadding + (50 + space) * i);
				}
			}
			break;
		default:
			break;
		}

		childCount += count;
	}

	public void getNextPositionForDrawHand(int priority, out Vector3 position) {
		position = new Vector3 ();

		int insertIndex = transform.childCount;
		for (int i = 0; i < transform.childCount; i++) {
			if (priority < transform.GetChild (i).gameObject.GetComponent<Present> ().Priority) {
				Debug.Log (priority);
				insertIndex = i;
				break;
			}
		}

		if (insertIndex == 0) {
			position = transform.GetChild (0).transform.position;
			position.x -= 25 - layoutGroup.spacing / 2;
		} else if (insertIndex == transform.childCount) {
			position = transform.GetChild (insertIndex-1).transform.position;
			position.x += 25 + layoutGroup.spacing / 2;
		} else {
			Vector3 rightPosition = transform.GetChild (insertIndex).transform.position;
			Vector3 leftPosition = transform.GetChild (insertIndex - 1).transform.position;

			position = (rightPosition + leftPosition) / 2;
			
		}
	}
}
