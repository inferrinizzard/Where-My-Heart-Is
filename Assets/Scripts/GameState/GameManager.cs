using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Constrols app macro and scene manipulations </summary>
public class GameManager : Singleton<GameManager>
{
	[HideInInspector] public bool duringLoad;
	[HideInInspector] public PauseMenu pause;
	[HideInInspector] public DialogueSystem dialogue;
	[HideInInspector] public Prompt prompt;
	public LevelOrder levelOrder;
	public float transitionTime = 3f;

	// public Level test;

	public override void Awake()
	{
		base.Awake();
		pause = GetComponentInChildren<PauseMenu>();
		dialogue = GetComponentInChildren<DialogueSystem>();
		prompt = GetComponentInChildren<Prompt>();
	}

	void Start()
	{
		World.Instance.name += $" [{SceneManager.GetActiveScene().name}]";
		SceneManager.sceneLoaded += new UnityEngine.Events.UnityAction<Scene, LoadSceneMode>((scene, _) => OnEnterScene());
		// SceneManager.activeSceneChanged += new UnityEngine.Events.UnityAction<Scene, Scene>((_, __) => this.Print("ActiveSceneChanged", SceneManager.GetActiveScene().name));

		levelOrder.Start();
		// test?.StartBehaviors();
	}

	/// <summary> Closes the Application </summary>
	public static void QuitGame()
	{
		// prompt
		Application.Quit();
	}

	public void ChangeLevel()
	{
		levelOrder.End();
		levelOrder.NextScene();
		Transition(levelOrder.Name);
	}

	public IEnumerator ChangeLevelManual()
	{
		levelOrder.End();
		levelOrder.NextScene();
		return LoadScene(levelOrder.Name);
	}

	/// <summary> Starts Coroutine to load scene async  </summary>
	/// <param name="scene"> Name of scene to load  </param>
	static void Transition(string name) => instance.StartCoroutine(LoadScene(name));

	/// <summary> Loads scene asynchronously, will transition when ready </summary>
	/// <param name="scene"> Name of scene to load  </param>
	public static IEnumerator LoadScene(string name)
	{
		instance.duringLoad = true;
		Player.Instance.OnExitScene();

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;

		float startTime = Time.time;
		bool inProgress = true;

		while (inProgress)
		{
			yield return null;
			float currentTime = Time.time - startTime;
			float loadProgress = Mathf.Min(asyncLoad.progress / .9f, currentTime / instance.transitionTime);

			Player.Instance.TransitionUpdate();

			if (asyncLoad.progress >= .9f && currentTime > instance.transitionTime)
				inProgress = false;
		}
		asyncLoad.allowSceneActivation = true;
		instance.duringLoad = false;
	}

	public override void OnEnterScene()
	{
		instance.pause.gameObject.SetActive(true); // TODO: not pause but just gameplayUI
		//World.Instance.name += $"[{levels[++sceneIndex]}]";
		Player.Instance.OnEnterScene();
		levelOrder.OnEnterScene();
	}

	/// <summary> Unloads scene asynchronously </summary>
	/// <param name="scene"> Name of scene to unload  </param>
	static IEnumerator UnloadScene(string name)
	{
		AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(name);
		while (!asyncUnload.isDone)
			yield return null;
	}

	public static void ReloadScene()
	{
		// Instance.sceneIndex--;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
