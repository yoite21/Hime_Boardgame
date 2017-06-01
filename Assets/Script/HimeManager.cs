using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HimeInfo
{
	public int score;
	public Color backgroundColor;

	public HimeInfo(int _score, Color _backgroundColor)
	{
		score = _score;
		backgroundColor = _backgroundColor;
	}
};

public class HimeManager : MonoBehaviour {

	// himeInfo pool
	private HimeInfo[] himeInfoPool = new HimeInfo[] {
		new HimeInfo(5, Color.blue),
		new HimeInfo(4, Color.cyan),
		new HimeInfo(3, Color.green),
		new HimeInfo(3, Color.magenta),
		new HimeInfo(2, Color.red),
		new HimeInfo(2, Color.yellow),
		new HimeInfo(2, Color.white)
	};

	public HimeInfo[] HimeInfoPool {
		get {
			return himeInfoPool;
		}
	}

	void Start()
	{
		// hime init
		for (int i = 0; i < transform.childCount; i++) {
			Hime hime = transform.GetChild (i).GetComponent<Hime>();
			hime.Score = himeInfoPool [i].score;
			hime.BackgroundColor = himeInfoPool [i].backgroundColor;
			hime.init ();

			hime.HeartState = HeartType.NONE;
		}
	}

	public void heartReset()
	{
		for (int i = 0; i < transform.childCount; i++) {
			Hime hime = transform.GetChild (i).GetComponent<Hime>();
			hime.HeartState = HeartType.NONE;
		}
	}

	public void presentToHime(List<Present> presents, List<Present> opponentPresents)
	{
		foreach (var present in presents) {
			var myPresents = getCorrectHime (present.BackgroundColor).transform.FindChild ("MyPresents");
			present.transform.SetParent (myPresents);
		}

		if (opponentPresents != null) {
			foreach (var present in opponentPresents) {
				var opponentPresent = getCorrectHime (present.BackgroundColor).transform.FindChild ("OpponentPresents");
				present.transform.SetParent (opponentPresent);
			}
		}
	}

	public void moveHeart()
	{
		for (int i = 0; i < transform.childCount; i++) {
			var hime = transform.GetChild (i).gameObject.GetComponent<Hime> ();
			int myPresent = hime.transform.FindChild ("MyPresents").transform.childCount;
			int opponentPresent = hime.transform.FindChild ("OpponentPresents").transform.childCount;

			if (myPresent > opponentPresent) {
				hime.HeartState = HeartType.MINE;
			} else if (myPresent < opponentPresent) {
				hime.HeartState = HeartType.OPPONENT;
			}
		}
	}

	public void getScore (out int myScore, out int myHeart, out int opponentScore, out int opponentHeart)
	{
		myScore = 0;
		myHeart = 0;
		opponentHeart = 0;
		opponentScore = 0;

		for (int i = 0; i < transform.childCount; i++) {
			var hime = transform.GetChild (i).gameObject.GetComponent<Hime> ();
			if (hime.HeartState == HeartType.MINE) {
				myHeart++;
				myScore += hime.Score;
			} else if (hime.HeartState == HeartType.OPPONENT) {
				opponentHeart++;
				opponentScore += hime.Score;
			}
		}
		
	}

	private Hime getCorrectHime(Color color)
	{
		for (int i = 0; i < transform.childCount; i++) {
			var hime = transform.GetChild (i).GetComponent<Hime> ();
			if (hime.BackgroundColor == color) {
				return hime;
			}
		}

		return null;
	}

}
