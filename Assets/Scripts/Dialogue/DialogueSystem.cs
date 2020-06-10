using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles the inputting of text from a json file and displaying of text onto the screen. </summary>
public class DialogueSystem : MonoBehaviour
{
	/// <summary> The Unity UI text object for the text to be displayed through. </summary>
	public Text uiText = default;
	FMOD.Studio.EventInstance currentDialogue;
	int test;

	void Update()
	{
		currentDialogue.getTimelinePosition(out test);
		Debug.Log(test);
	}

	public void PlayScript(DialogueText.LevelText levelText)
	{
		gameObject.SetActive(true);
		GameManager.Instance.StartCoroutine(WaitAndPlay(levelText));
	}

	IEnumerator WaitAndPlay(DialogueText.LevelText levelText, float time = 1)
	{
		currentDialogue = FMODUnity.RuntimeManager.CreateInstance(levelText.fmod);
		yield return new WaitForSeconds(time);
		currentDialogue.start();
		// FMODUnity.RuntimeManager.PlayOneShot(levelText.fmod);
	}
}
