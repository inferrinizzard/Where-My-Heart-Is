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

	public void TogglePause(bool pause) => currentDialogue.setPaused(pause);

	public void Stop()
	{
		currentDialogue.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		currentDialogue.release();
		gameObject.SetActive(false);
	}

	public void PlayScript(DialogueText.LevelText levelText)
	{
        Stop();
		gameObject.SetActive(true);
		currentLevelText = levelText;
        GameManager.Instance.StartCoroutine(WaitAndPlay());
	}

	IEnumerator WaitAndPlay(float time = 1)
	{
		currentDialogue = FMODUnity.RuntimeManager.CreateInstance(currentLevelText.fmod);
		yield return new WaitForSeconds(time);
		currentDialogue.start();

        for(int i = 0; i < currentLevelText.timestamps.Count; i++)
        {
            StartCoroutine(SetText(currentLevelText.text[i], currentLevelText.timestamps[i], currentLevelText));
        }
	}

    IEnumerator SetText(string text, float delay, DialogueText.LevelText currText)
    {
        yield return new WaitForSeconds(delay);
        if(currText.text[0] == currentLevelText.text[0])
        {
            uiText.text = text;
        }
    }
}
