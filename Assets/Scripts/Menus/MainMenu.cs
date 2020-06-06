using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary> Local instance of main menu canvas objects. </summary>
    public GameObject mainMenuUI;
    /// <summary> Local instance of options menu canvas objects. </summary>
    public GameObject optionsMenuUI;

    void Start()
    {
        CloseOptions(); // Make sure we start in the main menu.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseOptions();
    }

    /// <summary> Opens the options menu and closes other menus. </summary>
    public void OpenOptions()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    /// <summary> Closes the options menu and displays the main menu. </summary>
    public void CloseOptions()
    {
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
    }

    /// <summary> Quits the game. </summary>
	public void Quit()
    {
        GameManager.QuitGame();
    }
}
