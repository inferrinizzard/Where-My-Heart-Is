using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles setting of various volume controls associated with the sound page. </summary>
public class SoundMenu : MonoBehaviour
{
    /// <summary> Input field for master volume control. </summary>
    public InputField masterInputField;
    /// <summary> Input field for sfx volume control. </summary>
    public InputField sfxInputField;
    /// <summary> Input field for voice volume control. </summary>
    public InputField voiceInputField;
    /// <summary> Input field for music volume control. </summary>
    public InputField musicInputField;

    /// <summary> Slider for master volume control. </summary>
    public Slider masterSlider;
    /// <summary> Slider for sfx volume control. </summary>
    public Slider sfxSlider;
    /// <summary> Slider for voice volume control. </summary>
    public Slider voiceSlider;
    /// <summary> Slider for music volume control. </summary>
    public Slider musicSlider;

    /// <summary> Sets the master volume control and the input field based on the input from the slider. </summary>
    public void SetMasterSlider()
    {
        masterInputField.text = masterSlider.value.ToString();
    }

    /// <summary> Sets the master volume control and the slider based on the input from the input field. </summary>
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

    /// <summary> Sets the sfx volume control and the input field based on the input from the slider. </summary>
    public void SetSFXSlider()
    {
        sfxInputField.text = sfxSlider.value.ToString();
    }

    /// <summary> Sets the sfx volume control and the slider based on the input from the input field. </summary>
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

    /// <summary> Sets the voice volume control and the input field based on the input from the slider. </summary>
    public void SetVoiceSlider()
    {
        voiceInputField.text = voiceSlider.value.ToString();
    }

    /// <summary> Sets the voice volume control and the slider based on the input from the input field. </summary>
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

    /// <summary> Sets the music volume control and the input field based on the input from the slider. </summary>
    public void SetMusicSlider()
    {
        musicInputField.text = musicSlider.value.ToString();
    }

    /// <summary> Sets the music volume control and the slider based on the input from the input field. </summary>
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

    /// <summary> Syncs the UI elements with the current values of all settings. </summary>
    public void RefreshSettings()
    {
        //TODO
    }
}
