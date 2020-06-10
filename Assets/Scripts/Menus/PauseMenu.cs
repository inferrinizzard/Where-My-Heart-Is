using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Handles the pause menu UI elements and the pausing of the game. </summary>
public class PauseMenu : MonoBehaviour
{
	/// <summary> Whether the game is paused or not. </summary>
	[HideInInspector] public bool GameIsPaused = false;
	/// <summary> Whether the options menu is open or not. </summary>
	[HideInInspector] public bool OptionsMenuOpen = false;
	/// <summary> Whether the options menu is open or not. </summary>
	[HideInInspector] public bool MainMenuOpen = false;
	/// <summary> Local instance of pause menu canvas objects. </summary>
	public GameObject pauseMenuUI;
	/// <summary> Local instance of options menu canvas objects. </summary>
	public GameObject optionsMenuUI;
	/// <summary> Local instance of crosshair object. </summary>
	public GameObject gameplayUI;
	/// <summary> Local instance of keysetter object. </summary>
	[HideInInspector] public ControlsMenu keySetter;
	/// <summary> Local instance of camera for PIP. </summary>
	private Camera _camera;
	/// <summary> Raw image for PIP. </summary>
	[HideInInspector] public RawImage pip;

	/// <summary> Master volume stored value. </summary>
	[HideInInspector] public float masterValue = 80;
	/// <summary> SFX volume stored value. </summary>
	[HideInInspector] public float sfxValue = 80;
	/// <summary> Voice volume stored value. </summary>
	[HideInInspector] public float voiceValue = 80;
	/// <summary> Music volume stored value. </summary>
	[HideInInspector] public float musicValue = 80;

	public void Init()
	{
		// Components for PIP
		_camera = Player.Instance.cam;
		pip = GetComponentInChildren<RawImage>(true);

		keySetter = GetComponentInChildren<ControlsMenu>(true);

		InputManager.OnPauseKeyDown += PauseAction;
		Resume(); // When the game starts, make sure we aren't paused.
	}

	/// <summary> Function to bind to pause input action. </summary>
	private void PauseAction()
	{
		if(!MainMenuOpen)
		{
			if (GameIsPaused)
                if (OptionsMenuOpen) // Allow escape to be used to exit the options menu.
                    CloseOptions();
                else
                    Resume();
            else Pause();
        }
	}

	/// <summary> Resumes the game. </summary>
	public void Resume()
	{
		if (!keySetter.wasLookingForKey)
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
		optionsMenuUI.GetComponent<OptionsMenu>().RefreshSettings();
		StartCoroutine(PauseRoutine());
	}

	/// <summary> Sets the pause menu elements and gameplay elements for displaying the main menu. </summary>
	/// <remarks> pls no spaghetti </remarks>
	public void MainMenuStart()
	{
		MainMenuOpen = true;
		pauseMenuUI.SetActive(false);
		gameplayUI.SetActive(false);
		Time.timeScale = 1f;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Player.Instance.canMove = false;
		GameObject test = GameObject.Find("MainMenuUI");
		StartCoroutine(GetMainMenuPIP());
	}

	/// <summary> Ends the main menu state. </summary>
	/// <remarks> pls no spaghetti </remarks>
	public void MainMenuEnd()
	{
		MainMenuOpen = false;
		Player.Instance.canMove = true;
		Resume();
	}

	/// <summary> Used to set the game into a pause state. </summary>
	IEnumerator PauseRoutine()
	{
		yield return StartCoroutine(GetPIP());
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
		OptionsMenuOpen = true;
	}

	/// <summary> Closes the options UI. </summary>
	public void CloseOptions()
	{
		optionsMenuUI.SetActive(false);
		pauseMenuUI.SetActive(true);
		OptionsMenuOpen = false;
	}

	/// <summary> Takes a screenshot of the main camera and applies it to a RenderTexture. </summary>
	public IEnumerator GetPIP()
	{
		yield return new WaitForEndOfFrame();
		pip.texture = ApplyMask.Screenshot();
	}

	/// <summary> Takes a screenshot of the player's camera and applies it to a RenderTexture. </summary>
	/// <remarks> Temporarily used for Main Menu PIP UI element. </remarks>
	/// <remarks> pls no spaghetti </remarks>
	public IEnumerator GetMainMenuPIP()
	{
		yield return new WaitForEndOfFrame();
		pip.texture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
		_camera.targetTexture = (RenderTexture) pip.texture;
		_camera.Render();
		RenderTexture.active = (RenderTexture) pip.texture;
		_camera.targetTexture = null; // must set to null so that the camera will also render to main display after taking screenshot
	}
}
