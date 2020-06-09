using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "IntroAndMainMenu", menuName = "Levels/Behaviours/IntroAndMainMenu")]
public class IntroAndMainMenu : LevelBehaviour
{

	public void LoadAdditiveScene()
	{
		if (SceneManager.sceneCount == 1)
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
	}

	public void TurnOffFade()
	{
		//Player.VFX.fadeController.gameObject.SetActive(false);
	}
}
