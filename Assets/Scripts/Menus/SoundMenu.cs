using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    public InputField masterInputField;
    public InputField sfxInputField;
    public InputField voiceInputField;
    public InputField musicInputField;

    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;
    public Slider musicSlider;

    public void SetMasterSlider()
    {
        masterInputField.text = masterSlider.value.ToString();
    }

    public void SetMasterInputField()
    {
        float input;
        if (masterInputField.text == "")
            input = masterSlider.minValue;
        else
            input = float.Parse(masterInputField.text);
        input = Mathf.Clamp(input, masterSlider.minValue, masterSlider.maxValue);
        masterSlider.value = input;
        masterInputField.text = input.ToString();
    }

    public void SetSFXSlider()
    {
        sfxInputField.text = sfxSlider.value.ToString();
    }

    public void SetSFXInputField()
    {
        float input;
        if (sfxInputField.text == "")
            input = sfxSlider.minValue;
        else
            input = float.Parse(sfxInputField.text);
        input = Mathf.Clamp(input, sfxSlider.minValue, sfxSlider.maxValue);
        sfxSlider.value = input;
        sfxInputField.text = input.ToString();
    }

    public void SetVoiceSlider()
    {
        voiceInputField.text = voiceSlider.value.ToString();
    }

    public void SetVoiceInputField()
    {
        float input;
        if (voiceInputField.text == "")
            input = voiceSlider.minValue;
        else
            input = float.Parse(voiceInputField.text);
        input = Mathf.Clamp(input, voiceSlider.minValue, voiceSlider.maxValue);
        voiceSlider.value = input;
        voiceInputField.text = input.ToString();
    }

    public void SetMusicSlider()
    {
        musicInputField.text = musicSlider.value.ToString();
    }

    public void SetMusicInputField()
    {
        float input;
        if (musicInputField.text == "")
            input = musicSlider.minValue;
        else
            input = float.Parse(musicInputField.text);
        input = Mathf.Clamp(input, musicSlider.minValue, musicSlider.maxValue);
        musicSlider.value = input;
        musicInputField.text = input.ToString();
    }
}
