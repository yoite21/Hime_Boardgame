using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum HeartType {OPPONENT, NONE, MINE};

public class Hime : MonoBehaviour {

	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Image heartImage;
	[SerializeField]
	private VerticalLayoutGroup myPresent;
	[SerializeField]
	private VerticalLayoutGroup opponentPresent;


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


	Vector3[] heartPosition = new Vector3[] {
		new Vector3(0,40,0),
		new Vector3(0,0,0),
		new Vector3(0,-40,0)
	};

	private HeartType heartState = HeartType.NONE;
	public HeartType HeartState {
		get { return heartState; }
		set { 
			heartState = value;
			setHeartState (value);
		}
	}


	private void setHeartState(HeartType state)
	{
		heartImage.transform.localPosition = heartPosition [(int)state];
	}

	public void init()
	{
		scoreText.text = score.ToString ();
		GetComponent<Image> ().color = backgroundColor;
	}

	public void addMyPresent(Present present)
	{
		present.transform.SetParent (myPresent.transform);
		if (myPresent.transform.childCount > 3) {
			myPresent.spacing = -20;
		} else {
			myPresent.spacing = 10;
		}
	}

	public void addOpponentPresent(Present present)
	{
		present.transform.SetParent (opponentPresent.transform);
		if (opponentPresent.transform.childCount > 3) {
			opponentPresent.spacing = -20;
		} else {
			opponentPresent.spacing = 10;
		}
	}

	public void moveHeart()
	{
		var myPresentCount = myPresent.transform.childCount;
		var opponentPresentCount = opponentPresent.transform.childCount;

		if (myPresentCount > opponentPresentCount) {
			HeartState = HeartType.MINE;
		} else if (myPresentCount < opponentPresentCount) {
			HeartState = HeartType.OPPONENT;
		}
	}
}
