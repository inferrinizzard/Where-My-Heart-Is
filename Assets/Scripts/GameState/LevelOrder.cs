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

    public string getSceneName(int index)
    {
        Debug.Log(index);
        Debug.Log(levels[index].getSceneName());
        return levels[index].getSceneName();
    }
}
