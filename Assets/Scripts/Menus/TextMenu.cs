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
    public Toggle textToggle;
    Textbox dialogueText;
    DialogueSystem dialogueSystem;

    private void Start()
    {
        dialogueText = GameManager.Instance.gameObject.GetComponentInChildren<Textbox>(true);
        dialogueSystem = GameManager.Instance.gameObject.GetComponentInChildren<DialogueSystem>(true);
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

    public void SetTextToggle()
    {
        if(textToggle.isOn)
        {
            dialogueSystem.gameObject.SetActive(true);
            dialogueText.gameObject.SetActive(true);
        }
        else
        {
            dialogueSystem.gameObject.SetActive(false);
            dialogueText.gameObject.SetActive(false);
        }
    }

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
            textToggle.isOn = dialogueSystem.gameObject.activeSelf;
        }
    }
}
