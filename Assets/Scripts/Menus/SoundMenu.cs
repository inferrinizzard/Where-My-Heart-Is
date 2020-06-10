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

    /// <summary> Toggle for master volume control. </summary>
    public Toggle masterToggle;
    /// <summary> Toggle for sfx volume control. </summary>
    public Toggle sfxToggle;
    /// <summary> Toggle for voice volume control. </summary>
    public Toggle voiceToggle;
    /// <summary> Toggle for music volume control. </summary>
    public Toggle musicToggle;

    /// <summary> Toggle value for multiplying with master input values. </summary>
    /// <remarks> 1 for on, 0 for off. </remarks>
    private int masterToggleValue;
    /// <summary> Toggle value for multiplying with sfx input values. </summary>
    /// <remarks> 1 for on, 0 for off. </remarks>
    private int sfxToggleValue;
    /// <summary> Toggle value for multiplying with voice input values. </summary>
    /// <remarks> 1 for on, 0 for off. </remarks>
    private int voiceToggleValue;
    /// <summary> Toggle value for multiplying with music input values. </summary>
    /// <remarks> 1 for on, 0 for off. </remarks>
    private int musicToggleValue;

    /// <summary> Sets the master volume control and the input field based on the input from the slider. </summary>
    public void SetMasterSlider()
    {
        masterInputField.text = masterSlider.value.ToString();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master Volume", (masterSlider.value * masterToggleValue) / 100);
        GameManager.Instance.pause.masterValue = masterSlider.value;
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
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master Volume", (masterSlider.value * masterToggleValue) / 100);
        GameManager.Instance.pause.masterValue = masterSlider.value;
    }

    /// <summary> Sets the toggle for master volume and sets the audio levels accordingly. </summary>
    public void SetMasterToggle()
    {
        if (masterToggle.isOn) masterToggleValue = 1;
        else masterToggleValue = 0;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Master Volume", (masterSlider.value * masterToggleValue) / 100);
    }

    /// <summary> Sets the sfx volume control and the input field based on the input from the slider. </summary>
    public void SetSFXSlider()
    {
        sfxInputField.text = sfxSlider.value.ToString();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX Volume", (sfxSlider.value * sfxToggleValue) / 100);
        GameManager.Instance.pause.sfxValue = sfxSlider.value;
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
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX Volume", (sfxSlider.value * sfxToggleValue) / 100);
        GameManager.Instance.pause.sfxValue = sfxSlider.value;
    }

    /// <summary> Sets the toggle for sfx volume and sets the audio levels accordingly. </summary>
    public void SetSFXToggle()
    {
        if (sfxToggle.isOn) sfxToggleValue = 1;
        else sfxToggleValue = 0;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("SFX Volume", (sfxSlider.value * sfxToggleValue) / 100);
    }

    /// <summary> Sets the voice volume control and the input field based on the input from the slider. </summary>
    public void SetVoiceSlider()
    {
        voiceInputField.text = voiceSlider.value.ToString();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Dialog Volume", (voiceSlider.value * voiceToggleValue) / 100);
        GameManager.Instance.pause.voiceValue = voiceSlider.value;
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
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Dialog Volume", (voiceSlider.value * voiceToggleValue) / 100);
        GameManager.Instance.pause.voiceValue = voiceSlider.value;
    }

    /// <summary> Sets the toggle for voice volume and sets the audio levels accordingly. </summary>
    public void SetVoiceToggle()
    {
        if (voiceToggle.isOn) voiceToggleValue = 1;
        else voiceToggleValue = 0;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Dialog Volume", (voiceSlider.value * voiceToggleValue) / 100);
    }

    /// <summary> Sets the music volume control and the input field based on the input from the slider. </summary>
    public void SetMusicSlider()
    {
        musicInputField.text = musicSlider.value.ToString();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music Volume", (musicSlider.value * musicToggleValue) / 100);
        GameManager.Instance.pause.musicValue = musicSlider.value;
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
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music Volume", (musicSlider.value * musicToggleValue) / 100);
        GameManager.Instance.pause.musicValue = musicSlider.value;
    }

    /// <summary> Sets the toggle for music volume and sets the audio levels accordingly. </summary>
    public void SetMusicToggle()
    {
        if (musicToggle.isOn) musicToggleValue = 1;
        else musicToggleValue = 0;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Music Volume", (musicSlider.value * musicToggleValue) / 100);
    }

    /// <summary> Syncs the UI elements with the current values of all settings. </summary>
    public void RefreshSettings()
    {
        // *** MASTER VOLUME SETTING *** //
        float masterVal = 0;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Master Volume", out masterVal);
        float masterStoredVal = GameManager.Instance.pause.masterValue;
        if (masterVal > 0)
        {
            masterToggle.isOn = true;
            masterToggleValue = 1;
        }
        else
        {
            masterToggle.isOn = false;
            masterToggleValue = 0;

        }
        masterSlider.value = masterStoredVal;
        masterInputField.text = masterStoredVal.ToString();

        // *** SFX VOLUME SETTING *** //
        float sfxVal = 0;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("SFX Volume", out sfxVal);
        float sfxStoredVal = GameManager.Instance.pause.sfxValue;
        if (sfxVal > 0)
        {
            sfxToggle.isOn = true;
            sfxToggleValue = 1;
        }
        else
        {
            sfxToggle.isOn = false;
            sfxToggleValue = 0;

        }
        sfxSlider.value = sfxStoredVal;
        sfxInputField.text = sfxStoredVal.ToString();


        // *** VOICE VOLUME SETTING *** //
        float voiceVal = 0;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Dialog Volume", out voiceVal);
        float voiceStoredVal = GameManager.Instance.pause.voiceValue;
        if (voiceVal > 0)
        {
            voiceToggle.isOn = true;
            voiceToggleValue = 1;
        }
        else
        {
            voiceToggle.isOn = false;
            voiceToggleValue = 0;

        }
        voiceSlider.value = voiceStoredVal;
        voiceInputField.text = voiceStoredVal.ToString();

        // *** MUSIC VOLUME SETTING *** //
        float musicVal = 0;
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Music Volume", out musicVal);
        float musicStoredVal = GameManager.Instance.pause.musicValue;
        if (musicVal > 0)
        {
            musicToggle.isOn = true;
            musicToggleValue = 1;
        }
        else
        {
            musicToggle.isOn = false;
            musicToggleValue = 0;

        }
        musicSlider.value = musicStoredVal;
        musicInputField.text = musicStoredVal.ToString();
    }
}
