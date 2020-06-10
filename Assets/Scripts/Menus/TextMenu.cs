using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Handles setting of various text settings associated with the text page. </summary>
public class TextMenu : MonoBehaviour
{
	/// <summary> Slider for text size. </summary>
	public Slider textSizeSlider;
	/// <summary> Text to display current text size. </summary>
	public Text textSizeText;
	/// <summary> Toggle to turn text on or off. </summary>
	public Toggle textToggle;
	/// <summary> Element for subtitle text. </summary>
	Textbox dialogueText;
	/// <summary> Element for dialogue system. </summary>
	DialogueSystem dialogueSystem;

	private void Start()
	{
		dialogueSystem = GameManager.Instance.GetComponentInChildren<DialogueSystem>(true);
		dialogueText = dialogueSystem.GetComponentInChildren<Textbox>(true);
		RefreshSettings();
		SetTextSizeSlider(); // Set the text at the beginning.
	}

	/// <summary> Set the text size as well as the text UI based on the input from the slider. </summary>
	public void SetTextSizeSlider()
	{
		int input = (int) textSizeSlider.value;
		if (input == 0)
		{
			textSizeText.text = "small";
			dialogueSystem.uiText.fontSize = 40;
		}
		if (input == 1)
		{
			textSizeText.text = "medium";
			dialogueSystem.uiText.fontSize = 50;
		}
		if (input == 2)
		{
			textSizeText.text = "large";
			dialogueSystem.uiText.fontSize = 60;
		}
	}

	/// <summary> Sets the visibility of the text based on a toggle. </summary>
	public void SetTextToggle()
	{
		dialogueSystem.gameObject.SetActive(textToggle.isOn);
		dialogueText.gameObject.SetActive(textToggle.isOn);
	}

	/// <summary> Syncs the UI elements with the current values of all settings. </summary>
	public void RefreshSettings()
	{
		if (dialogueSystem)
		{
			if (dialogueSystem.uiText.fontSize == 40)
				textSizeSlider.value = 0;
			if (dialogueSystem.uiText.fontSize == 50)
				textSizeSlider.value = 1;
			if (dialogueSystem.uiText.fontSize == 60)
				textSizeSlider.value = 2;

			// dialogueSystem.gameObject.SetActive(textToggle.isOn);
			this.Print(dialogueSystem.gameObject.activeSelf, textToggle.isOn);
			textToggle.isOn = dialogueSystem.gameObject.activeSelf;
		}
	}
}
