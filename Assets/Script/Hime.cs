﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum HeartType {OPPONENT, NONE, MINE};

public class Hime : MonoBehaviour {

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

	public Text scoreText;

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
		transform.GetChild (0).transform.localPosition = heartPosition [(int)state];
	}

	public void init()
	{
		scoreText.text = score.ToString ();
		GetComponent<Image> ().color = backgroundColor;
	}

}
