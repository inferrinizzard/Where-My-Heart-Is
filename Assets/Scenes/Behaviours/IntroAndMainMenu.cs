using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "IntroAndMainMenu", menuName = "Levels/Behaviours/IntroAndMainMenu")]
public class IntroAndMainMenu : LevelBehaviour
{
	/// <summary> Used to load the main menu scene with the intro scene. </summary>
	public void LoadAdditiveScene()
	{
		if (SceneManager.sceneCount == 1)
			SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
	}

	/// <summary> Turn off the fade effect for the player to get a pip for the main menu. </summary>
	public void TurnOffFade()
	{
		//Player.VFX.fadeController.gameObject.SetActive(false);
	}
}
