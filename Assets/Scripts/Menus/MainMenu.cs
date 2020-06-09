using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary> Local instance of main menu canvas objects. </summary>
    public GameObject mainMenuUI;
    /// <summary> Local instance of options menu canvas objects. </summary>
    public GameObject optionsMenuUI;
    /// <summary> Local instance of credits menu canvas objects. </summary>
    public GameObject creditsMenuUI;

    void Start()
    {
        OpenMainMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenMainMenu();
    }

    /// <summary> Returns to the main menu. </summary>
    public void OpenMainMenu()
    {
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
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

    /// <summary> Quits the game. </summary>
	public void Quit()
    {
        GameManager.QuitGame();
    }
}
