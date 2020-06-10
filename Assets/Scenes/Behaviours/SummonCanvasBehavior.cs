using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonCanvasBehavior", menuName = "Levels/Behaviours/SummonCanvasBehavior")]
public class SummonCanvasBehavior : LevelBehaviour
{
    [FMODUnity.EventRef]
    public string musicEvent;

    public void Init()
    {
        FindObjectOfType<AudioMaster>().PlaySongEvent(musicEvent);
    }
}
