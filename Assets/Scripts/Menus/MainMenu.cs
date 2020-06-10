using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary> Local instance of main menu canvas objects. </summary>
    public GameObject mainMenuUI;
    /// <summary> Local instance of options menu canvas objects. </summary>
    public GameObject optionsMenuUI;
    /// <summary> Local instance of credits menu canvas objects. </summary>
    public GameObject creditsMenuUI;
    /// <summary> PIP raw image for displaying of game scene. </summary>
    public RawImage pip;
    /// <summary> Whether the main menu is active or not. </summary>
    bool mainMenuOpen = true;

    [FMODUnity.EventRef]
    public string titleEvent;

    [FMODUnity.EventRef]
    public string introEvent;

    void Start()
    {
        OpenMainMenu();

        FindObjectOfType<AudioMaster>().PlaySongEvent(titleEvent);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && mainMenuOpen) OpenMainMenu();
    }

    /// <summary> Returns to the main menu. </summary>
    public void OpenMainMenu()
    {
        optionsMenuUI.GetComponent<OptionsMenu>().RefreshSettings();
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);

        pip.texture = GameManager.Instance.pause.pip.texture;
    }

    /// <summary> Opens the options menu and closes other menus. </summary>
    public void OpenOptions()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        creditsMenuUI.SetActive(false);
    }

    /// <summary> Opens the credits menu and closes other menus. </summary>
    public void OpenCredits()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }
    
    /// <summary> Starts the game. </summary>
    public void Play()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
        mainMenuOpen = false;
        GameManager.Instance.pause.MainMenuEnd();
        OpenSketchbook.PlayCameraSetup();

        // deactivate music event
        FindObjectOfType<AudioMaster>().PlaySongEvent(introEvent);
    }

    /// <summary> Quits the game. </summary>
	public void Quit()
    {
        GameManager.QuitGame();
    }
}
