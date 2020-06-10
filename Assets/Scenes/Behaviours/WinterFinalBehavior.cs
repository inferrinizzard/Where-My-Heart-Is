using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WinterFinalBehavior", menuName = "Levels/Behaviours/WinterFinalBehavior")]
public class WinterFinalBehavior : LevelBehaviour
{
    public void Init()
    {
        FindObjectOfType<AudioMaster>().SetMusicParameter("End", 1);
    }
}
