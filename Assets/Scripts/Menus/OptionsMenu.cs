using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public GameObject controlsMenu;
    public GameObject soundMenu;
    public GameObject textMenu;

    public void OpenControlsMenu()
    {
        controlsMenu.SetActive(true);
        soundMenu.SetActive(false);
        textMenu.SetActive(false);
    }

    public void OpenSoundMenu()
    {
        controlsMenu.SetActive(false);
        soundMenu.SetActive(true);
        textMenu.SetActive(false);
    }

    public void OpenTextMenu()
    {
        controlsMenu.SetActive(false);
        soundMenu.SetActive(false);
        textMenu.SetActive(true);
    }
}
