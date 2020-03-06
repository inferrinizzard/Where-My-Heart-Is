using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Constrols app macro and scene manipulations </summary>
public class GameManager : Singleton<GameManager>
{
	public readonly string[] levels = new string[] { "Intro", "Bridge", "Disappear", "SimpleGate", "Swap", "OneCut", "ComplexGate", "HalfCut 1", "AutumnFinal" };
	public int sceneIndex = -1;
	public bool duringLoad;

    [HideInInspector]
    public float loadProgress;


	void Start()
	{
		sceneIndex = levels.ToList().FindIndex(name => name == SceneManager.GetActiveScene().name);
		World.Instance.name += $" [{SceneManager.GetActiveScene().name}]";
        SceneManager.sceneLoaded += CreateSceneLoader;
	}

    /// <summary> Closes the Application </summary>
    public static void QuitGame()
	{
		// prompt
		Application.Quit();
	}

    /// <summary> Will delegate sub Reset calls </summary>
    public override void OnBeginTransition()
	{
        GetIPersistents().ForEach(singleton => singleton.OnBeginTransition());
	}

	public void ChangeLevel(string scene) => Transition(scene); // temp, to be deleted

	/// <summary> Starts Coroutine to load scene async  </summary>
	/// <param name="scene"> Name of scene to load  </param>
	public static void Transition(string scene)
	{
		instance.StartCoroutine(LoadScene(scene));
	}

    private List<IPersistent> GetIPersistents()
    {
        List<IPersistent> result = FindObjectsOfType<MonoBehaviour>().OfType<IPersistent>().Where(persistent => (object)persistent != this).ToList();
        return result;
    }

	/// <summary> Loads scene asynchronously, will transition when ready </summary>
	/// <param name="scene"> Name of scene to load  </param>
	static IEnumerator LoadScene(string name, float minDuration = 3)
	{
		instance.duringLoad = true;
        instance.loadProgress = 0;
        instance.OnBeginTransition();

        List<IPersistent> persistents = instance.GetIPersistents();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;

		float startTime = Time.time;
		bool inProgress = true;

		Material transitionMat = Player.Instance.mask.transitionMat;
        int _CutoffID = Shader.PropertyToID("_Cutoff");

		while (inProgress)
		{
			float currentTime = Time.time - startTime;
            instance.loadProgress = Mathf.Min(asyncLoad.progress / .9f, currentTime / minDuration);
			transitionMat.SetFloat(_CutoffID, instance.loadProgress * 2); // add curve here

            persistents.ForEach(persistent => persistent.TransitonUpdate());

			if (asyncLoad.progress >= .9f && currentTime > minDuration)
			{
				asyncLoad.allowSceneActivation = true;
				Player.Instance.mask.transitionMat = null;

				inProgress = false;
				instance.duringLoad = false;
			}
			yield return null;
		}
    }

    public void CreateSceneLoader(Scene from, LoadSceneMode mode = LoadSceneMode.Single)
    {
        Instantiate(new GameObject()).AddComponent<SceneLoader>();
    }

    public override void OnCompleteTransition()
    {
        ++sceneIndex;
        World.Instance.name += $"[{levels[sceneIndex]}]";
        GetIPersistents().ForEach(persistent => persistent.OnCompleteTransition());
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
