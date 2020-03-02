using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Constrols app macro and scene manipulations </summary>
public class GameManager : Singleton<GameManager>, IResetable
{
	/// <summary> UI Image wrapper for Loading Screen  </summary>
	GameObject loadingScreen;
	/// <summary> Slider for Loading Bar  </summary>
	Slider loadingBar;
	public readonly string[] levels = new string[] { "Intro", "Bridge", "Disappear", "SimpleGate", "Swap", "ComplexGate", "HalfCut 1", "AutumnFinal" };
	public int sceneIndex = -1;
	public ApplyOutline outlineManager;

	void Start()
	{
		outlineManager = GetComponentInChildren<ApplyOutline>();
		sceneIndex = levels.ToList().FindIndex(name => name == SceneManager.GetActiveScene().name);
		// get Loading Screen UI ref
		loadingScreen = transform.GetChild(0).GetChild(0).gameObject; // better find
		// get Slider ref
		loadingBar = loadingScreen.GetComponentInChildren<Slider>();

		World.Instance.name += $" [{SceneManager.GetActiveScene().name}]";

		outlineManager.cam = Player.Instance.cam;
		outlineManager.root = World.Instance.transform;

		SceneManager.activeSceneChanged += instance.InitScene;
	}

	/// <summary> Closes the Application </summary>
	public static void QuitGame()
	{
		// prompt
		Application.Quit();
	}

	/// <summary> SceneManager.activeSceneChanged Delegate wrapper </summary>
	void InitScene(Scene from, Scene to) => instance.Init();
	//maybe call unload here

	/// <summary> Will delegate sub Init calls </summary>
	public void Init()
	{
		World.Instance.Init();
		World.Instance.name += $"[{levels[++sceneIndex]}]";
		Player.Instance.Init();
		outlineManager.cam = Player.Instance.cam;
		outlineManager.root = World.Instance.transform;
	}

	/// <summary> Will delegate sub Reset calls </summary>
	public void Reset()
	{
		World.Instance.Reset();
		Player.Instance.Reset();
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
	static IEnumerator LoadScene(string name, float time = 3)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;

		float start = Time.time;
		bool inProgress = true;

		var transitionMat = Effects.mask.transitionMat;
		int _CutoffID = Shader.PropertyToID("_Cutoff");

		while (inProgress)
		{
			float step = Time.time - start;
			float minProgress = Mathf.Min(asyncLoad.progress / .9f, step / time);
			transitionMat.SetFloat(_CutoffID, minProgress * 2); // add curve here

			if (asyncLoad.progress >= .9f && step > time)
			{
				inProgress = false;

				instance.loadingScreen.SetActive(false);
				instance.Reset();
				asyncLoad.allowSceneActivation = true;

				Effects.mask.transitionMat = null;

				// instance.StartCoroutine(UnloadScene(name));
				// instance.Init();
			}
			yield return null;
		}
	}

	/// <summary> Unloads scene asynchronously </summary>
	/// <param name="scene"> Name of scene to unload  </param>
	static IEnumerator UnloadScene(string name)
	{
		AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(name);
		while (!asyncUnload.isDone)
			yield return null;
	}
}
