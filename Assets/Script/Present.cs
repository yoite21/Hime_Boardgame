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
	private ButtonManager buttonManager;

	private int score;
	public int Score {
		get { return score; }
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
		set { 
			presentDirection = value;
			reset ();
		}
	}

	private bool isSelected = false;
	public bool IsSelected {
		get { return isSelected; }
	}

	private enum PresentGroup { NONE = 0, GROUP1, GROUP2 };
	private PresentGroup group = PresentGroup.NONE;

	void Start()
	{
		presentManager = GameObject.FindGameObjectWithTag ("PresentManager").GetComponent<PresentManager> ();
		buttonManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<ButtonManager> ();
	}

	private void reset()
	{
		setScoreText ();
		setBackgroundColor ();
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

	private void setGroupText()
	{
		string text = "";
		switch (group) {
		case PresentGroup.NONE:
			text = "";
			break;
		case PresentGroup.GROUP1:
			text = "1";
			break;
		case PresentGroup.GROUP2:
			text = "2";
			break;
		default:
			text = "";
			break;
		}

		groupText.text = text;
	}

	public void selectPresent()
	{
		if (presentDirection == PresentDirection.BACK) {
			return;
		}

		// check state
		if (buttonManager.LastActionButton == null) {
			return;
		}

		if (presentManager.isPresentSetSelect ()) {
			if ( !isSelected )
			{
				if (!presentManager.isSelectable ()) {
					return;
				}

				var selectType = presentManager.getSelectType ();
				if (selectType == PresentManager.presentSetSelectType.SET1) {

					group = PresentGroup.GROUP1;
					presentManager.PresentSet = this;
					presentManager.increaseSelectCount (1);
				} else if (selectType == PresentManager.presentSetSelectType.SET2) {

					group = PresentGroup.GROUP2;
					presentManager.PresentSet = this;
					presentManager.increaseSelectCount (1);
				}
			} else {
				group = PresentGroup.NONE;
				presentManager.unselectFromPresentSet (this);
				presentManager.increaseSelectCount (-1);		
			}
		} else {
			group = PresentGroup.NONE;

			if (!isSelected) {
				if (presentManager.isSelectable ()) {
					presentManager.increaseSelectCount (1);
				} else {
					return;
				}
			} else {
				presentManager.increaseSelectCount (-1);
			}

		}

		isSelected = !isSelected;
		setSelectColor (isSelected);
		setGroupText ();

	}

	public void unselectPresent()
	{
		group = PresentGroup.NONE;
		isSelected = false;
		setSelectColor (false);
		setGroupText ();
	}

	private void setSelectColor(bool isSelected)
	{
		Image image = selectMarkImage.GetComponent<Image> ();
		if (isSelected) {
			image.color = new Color(0,0,0,0.7f);
		} else {
			image.color = new Color(1,1,1,0.7f);
		}
	}
}
