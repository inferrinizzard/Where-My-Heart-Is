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
    public SceneAsset sceneName;

    // string of dialogue
    public string dialogue;

    // level behaviors
    public List<LevelBehaviour> behaviours;

    public string getSceneName()
    {
        return sceneName.name;
    }

    public void StartBehaviors()
    {
        if (behaviours.Count > 0)
        {
            foreach(LevelBehaviour behavior in behaviours)
            {
                behavior.onLevelLoad();
            }
        }
    }

    public void EndBehaviours()
    {
        if(behaviours.Count > 0)
        {
            foreach (LevelBehaviour behaviour in behaviours)
            {
                behaviour.onLevelUnload();
            }
        }
    }

}
