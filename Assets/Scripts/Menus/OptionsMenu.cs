using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles switching between different options menus. </summary>
public class OptionsMenu : MonoBehaviour
{
    /// <summary> GameObject group for the controls menu UI elements. </summary>
    public GameObject controlsMenu;
    /// <summary> GameObject group for the sound menu UI elements. </summary>
    public GameObject soundMenu;
    /// <summary> GameObject group for the text menu UI elements. </summary>
    public GameObject textMenu;

    /// <summary> Makes the controls menu UI elements visible and hides the rest. </summary>
    public void OpenControlsMenu()
    {
        controlsMenu.SetActive(true);
        soundMenu.SetActive(false);
        textMenu.SetActive(false);
    }

    /// <summary> Makes the sound menu UI elements visible and hides the rest. </summary>
    public void OpenSoundMenu()
    {
        controlsMenu.SetActive(false);
        soundMenu.SetActive(true);
        textMenu.SetActive(false);
    }

    /// <summary> Makes the text menu UI elements visible and hides the rest. </summary>
    public void OpenTextMenu()
    {
        controlsMenu.SetActive(false);
        soundMenu.SetActive(false);
        textMenu.SetActive(true);
    }
}
