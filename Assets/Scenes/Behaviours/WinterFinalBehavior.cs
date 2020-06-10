using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WinterFinalBehavior", menuName = "Levels/Behaviours/WinterFinalBehavior")]
public class WinterFinalBehavior : LevelBehaviour
{
    public void Init()
    {
        AudioMaster audioMaster = FindObjectOfType<AudioMaster>();
        audioMaster.SetMusicParameter("End", 1);
        Player.Instance.audioController.SetWindowWorld(2);
        Player.Instance.audioController.realSurface = 3;
        Player.Instance.audioController.heartSurface = 2;
    }
}
