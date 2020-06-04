using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of various text settings associated with the text page. </summary>
public class TextMenu : MonoBehaviour
{
    /// <summary> Slider for text size. </summary>
    public Slider textSizeSlider;
    /// <summary> Text to display current text size. </summary>
    public Text textSizeText;

    private void Start()
    {
        SetTextSizeSlider(); // Set the text at the beginning.
    }

    /// <summary> Set the text size as well as the text UI based on the input from the slider. </summary>
    public void SetTextSizeSlider()
    {
        int input = (int) textSizeSlider.value;
        if (input == 0)
            textSizeText.text = "small";
        if (input == 1)
            textSizeText.text = "medium";
        if (input == 2)
            textSizeText.text = "large";
    }
}
