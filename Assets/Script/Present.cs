using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PresentDirection { BACK, FRONT };

public class Present : MonoBehaviour {
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Image selectMarkImage;
	[SerializeField]
	private Text groupText;

	private PresentManager presentManager;

	private int score;
	public int Score {
		set { score = value; }
	}

	private Color backgroundColor;
	public Color BackgroundColor {
		get { return backgroundColor; }
		set { backgroundColor = value; }
	}

	private int priority;
	public int Priority {
		get { return priority; }
		set { priority = value; }
	}
		
	private PresentDirection presentDirection = PresentDirection.BACK;
	public PresentDirection PresentDirection {
		get { return presentDirection; }
		set { 
			presentDirection = value;
			setDirection ();
		}
	}

	public enum PresentGroup { UNSELECTED, GROUP0, GROUP1, GROUP2, UNSELECTED_GROUP1, UNSELECTED_GROUP2 };
	private PresentGroup selectionGroup = PresentGroup.UNSELECTED;
	public PresentGroup SelectionGroup {
		get { return selectionGroup; }
		set {
			selectionGroup = value;
			setSelection ();
		}
	}

	public bool IsSelected {
		get { return ((selectionGroup == PresentGroup.GROUP0)
			|| (selectionGroup == PresentGroup.GROUP1)
			|| (selectionGroup == PresentGroup.GROUP2)); }
	}

	void Start()
	{
		presentManager = GameObject.FindGameObjectWithTag ("PresentManager").GetComponent<PresentManager> ();
	}

	public void selectPresent()
	{
		if (presentDirection == PresentDirection.BACK) {
			return;
		}

		presentManager.selectPresent (this);
		setSelection ();
	}

	public void moveToPosition(Vector3 target, Transform parent)
	{
		StartCoroutine (moveTo (target, parent));
	}

	private void setDirection()
	{
		setScoreText ();
		setBackgroundColor ();
	}

	private void setSelection()
	{
		setSelectColor ();
		setGroupText ();
	}

	private void setScoreText()
	{
		string text;
		if (presentDirection == PresentDirection.BACK) {
			text = "";
		} else {
			text = score.ToString ();
		}

		scoreText.text = text;
	}

	private void setBackgroundColor()
	{
		Color color;
		if (presentDirection == PresentDirection.BACK) {
			color = Color.black;
		} else {
			color = backgroundColor;
		}

		GetComponent<Image> ().color = color;
	}

	private void setSelectColor()
	{
		Image image = selectMarkImage.GetComponent<Image> ();
		if ( this.IsSelected ) {
			// selected
			image.color = new Color(0,0,0,0.7f);
		} else {
			image.color = new Color(1,1,1,0.7f);
		}
	}

	private void setGroupText()
	{
		string text = "";
		switch (selectionGroup) {
		case PresentGroup.GROUP1:
		case PresentGroup.UNSELECTED_GROUP1:
			text = "1";
			break;
		case PresentGroup.GROUP2:
		case PresentGroup.UNSELECTED_GROUP2:
			text = "2";
			break;
		default:
			text = "";
			break;
		}



		groupText.text = text;
	}

	public IEnumerator moveTo(Vector3 targetPosition, Transform newParent)
	{
		Debug.Log (targetPosition);

		presentManager.presentMoveAnimationStart();

		transform.SetParent (transform.parent.parent);

		Vector3 currentVelocity = Vector3.zero;
		while (Vector3.Distance( transform.position, targetPosition) > 0.1) {
			transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref currentVelocity, 0.5f);

			yield return null;
		}

		transform.SetParent (newParent);

		presentManager.presentMoveAnimationEnd ();
	}


}
