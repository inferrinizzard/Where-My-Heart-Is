using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Controls the behavior of the pause menu UI elements. </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary> Whether the game is paused or not. </summary>
    public bool GameIsPaused = false;

    /// <summary> Local instance of pause menu canvas objects. </summary>
    public GameObject pauseMenuUI;
    /// <summary> Local instance of options menu canvas objects. </summary>
    public GameObject optionsMenuUI;
    /// <summary> Local instance of crosshair object. </summary>
    public GameObject gameplayUI;
    /// <summary> Local instance of keysetter object. </summary>
    public KeySetter keySetter;

    void Start()
    {
        keySetter = GetComponentInChildren<KeySetter>();
        InputManager.OnPauseKeyDown += PauseAction;
        Resume(); // When the game starts, make sure we aren't paused.
    }

    /// <summary> Function to bind to pause input action. </summary>
    private void PauseAction()
    {
        if(GameIsPaused) Resume(); else Pause();
    }

    /// <summary> Resumes the game. </summary>
    public void Resume()
    {
        if(!keySetter.wasLookingForKey)
        {
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            gameplayUI.SetActive(true);
            Time.timeScale = 1f;
            GameIsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            keySetter.wasLookingForKey = false;
        }
    }

    /// <summary> Pauses the game. </summary>
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        gameplayUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary> Resets the game back to the beginning. </summary>
    public void ResetGame()
    {
        pauseMenuUI.SetActive(false);
        gameplayUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //manager.LoadScene("Intro");
    }

    /// <summary> Resets the current level. </summary>
    public void ResetLevel()
    {
        pauseMenuUI.SetActive(false);
        gameplayUI.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.ReloadScene();
    }

    /// <summary> Quits the game. </summary>
    public void Quit()
    {
        GameManager.QuitGame();
    }

    /// <summary> Opens the options UI. </summary>
    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    /// <summary> Closes the options UI. </summary>
    public void CloseOptions()
    {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}
