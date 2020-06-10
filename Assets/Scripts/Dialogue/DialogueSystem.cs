using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles the inputting of text from a json file and displaying of text onto the screen. </summary>
public class DialogueSystem : MonoBehaviour
{
	/// <summary> The Unity UI text object for the text to be displayed through. </summary>
	public Text uiText = default;
	FMOD.Studio.EventInstance currentDialogue;
	DialogueText.LevelText currentLevelText;
	int millis = -1, timestampIndex = 0;

	void Update()
	{
		if (millis > -1)
		{
			currentDialogue.getTimelinePosition(out millis);
			uiText.text = currentLevelText.text[timestampIndex];
			if (timestampIndex == currentLevelText.timestamps.Count - 1)
			{
				Debug.Log("stopped");
				timestampIndex = 0;
				// currentDialogue.stop();
				currentDialogue.release();
				gameObject.SetActive(false);
				millis = -1;
				// return;
			}
			if (millis > currentLevelText.timestamps[timestampIndex + 1] * 1000)
				timestampIndex++;
			// Debug.Log(millis);
		}
	}

	public void PlayScript(DialogueText.LevelText levelText)
	{
		gameObject.SetActive(true);
		currentLevelText = levelText;
		GameManager.Instance.StartCoroutine(WaitAndPlay());
	}

	IEnumerator WaitAndPlay(float time = 1)
	{
		currentDialogue = FMODUnity.RuntimeManager.CreateInstance(currentLevelText.fmod);
		yield return new WaitForSeconds(time);
		currentDialogue.start();
		millis = 0;
		// FMODUnity.RuntimeManager.PlayOneShot(levelText.fmod);
	}
}
