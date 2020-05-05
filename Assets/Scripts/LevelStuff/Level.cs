using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/Level", order = 2)]
public class Level : ScriptableObject
{
	// string of dialogue
	public string dialogue;

	// level behaviors
	public List<LevelBehaviour> behaviours;

	public string name;
	// scene
#if UNITY_EDITOR
	public UnityEditor.SceneAsset scene;

	void OnValidate() => name = scene?.name;
#endif

	public string GetSceneName() => name;

	public void StartBehaviors()
	{
		foreach (LevelBehaviour behavior in behaviours)
			behavior.OnLevelLoad();
	}

	public void EndBehaviours()
	{
		foreach (LevelBehaviour behaviour in behaviours)
			behaviour.OnLevelUnload();
	}

}
