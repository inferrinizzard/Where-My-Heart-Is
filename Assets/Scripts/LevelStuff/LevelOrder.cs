using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelOrder", menuName = "Levels/LevelOrder", order = 1)]
public class LevelOrder : ScriptableObject
{
    // list of levels
    public List<Level> levels = new List<Level>();

    Level currentLevel;
    int index;

    public void Start()
    {
        index = 0;
        currentLevel = levels[index];
        currentLevel.StartBehaviors();
    }

    public void End()
    {
        currentLevel.EndBehaviours();
    }

    public void NextScene()
    {
        if(index >= levels.Count - 1) index = 0; 
        else index++;
        currentLevel = levels[index];
    }

    public string GetSceneName()
    {
        return currentLevel.GetSceneName();
    }

    public void OnEnterScene()
    {
        currentLevel.StartBehaviors();
    }
}
