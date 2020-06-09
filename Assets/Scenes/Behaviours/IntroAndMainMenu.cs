using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "IntroAndMainMenu", menuName = "Levels/Behaviours/IntroAndMainMenu")]
public class IntroAndMainMenu : LevelBehaviour
{
	private GameObject mainMenuCamera;
	private GameObject introCamera;

	public void CameraSetup()
	{
		//introCamera = GameObject.Find("Main Camera");
		//mainMenuCamera = GameObject.Find("MainMenuCamera");

		//introCamera.GetComponent<Camera>().enabled = false;
		//mainMenuCamera.GetComponent<Camera>().enabled = true;

		//GameManager.Instance.pause.MainMenuStart();
	}

	public void LoadAdditiveScene()
	{
		if (SceneManager.sceneCount == 1)
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
	}
}
