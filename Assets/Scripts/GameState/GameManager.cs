using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Constrols app macro and scene manipulations </summary>
public class GameManager : Singleton<GameManager>
{
	/// <summary> UI Image wrapper for Loading Screen  </summary>
	GameObject loadingScreen;
	/// <summary> Slider for Loading Bar  </summary>
	Slider loadingBar;

	void Start()
	{
		// get Loading Screen UI ref
		loadingScreen = transform.GetChild(0).GetChild(0).gameObject; // better find
		// get Slider ref
		loadingBar = loadingScreen.GetComponentInChildren<Slider>();
	}

	/// <summary> Closes the Application </summary>
	public static void QuitGame()
	{
		// prompt
		Application.Quit();
	}

	public void ChangeLevel(string scene) => Transition(scene); // temp, to be deleted

	/// <summary> Starts Coroutine to load scene async  </summary>
	/// <param name="scene"> Name of scene to load  </param>
	public static void Transition(string scene)
	{
		instance.StartCoroutine(LoadScene(scene));
	}

	/// <summary> Loads scene asynchronously, will transition when ready </summary>
	/// <param name="scene"> Name of scene to load  </param>
	static IEnumerator LoadScene(string name)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;
		while (!asyncLoad.isDone)
		{

			instance.loadingScreen.SetActive(true);
			instance.loadingBar.normalizedValue = asyncLoad.progress / .9f;

			if (asyncLoad.progress >= .9f)
			{
				asyncLoad.allowSceneActivation = true;
				instance.loadingScreen.SetActive(false);
			}
			yield return null;
		}
	}
}
