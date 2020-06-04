using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMenu : MonoBehaviour
{
    public Slider textSizeSlider;
    public Text textSizeText;

    private void Start()
    {
        SetTextSizeSlider();
    }

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
