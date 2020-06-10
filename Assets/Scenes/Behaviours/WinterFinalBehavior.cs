using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "WinterFinalBehavior", menuName = "Levels/Behaviours/WinterFinalBehavior")]
public class WinterFinalBehavior : LevelBehaviour
{
	Bird bird;
	BirdcageWinter birdCage;

	public void Init()
	{
		AudioMaster audioMaster = FindObjectOfType<AudioMaster>();
		audioMaster.SetMusicParameter("End", 1);
		Player.Instance.audioController.SetWindowWorld(2);
		Player.Instance.audioController.realSurface = 3;
		Player.Instance.audioController.heartSurface = 2;
		bird = GameObject.FindObjectOfType<Bird>();
		birdCage = GameObject.FindObjectOfType<BirdcageWinter>();

		GameManager.Instance.CustomUpdate += DeGhostBird;
	}

	public void DeGhostBird()
	{
		Debug.Log(Vector3.Distance(bird.transform.position, birdCage.transform.position));
		if (Vector3.Distance(bird.transform.position, birdCage.transform.position) < 2f)
		{
			bird.gameObject.GetComponent<BirdTrail>().enabled = false;
		}
		AudioMaster audioMaster = FindObjectOfType<AudioMaster>();
		audioMaster.SetMusicParameter("End", 1);
		Player.Instance.audioController.SetWindowWorld(2);
	}

	public void Clean() => GameManager.Instance.CustomUpdate -= DeGhostBird;
}
