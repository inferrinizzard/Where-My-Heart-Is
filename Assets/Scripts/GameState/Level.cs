using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/Level", order = 2)]
public class Level : ScriptableObject
{
    // scene
    public string sceneName;

    // string of dialogue
    public string dialogue;

    // level behaviors
    public List<LevelBehavior> behaviors;

    public string getSceneName()
    {
        return sceneName;
    }
}
