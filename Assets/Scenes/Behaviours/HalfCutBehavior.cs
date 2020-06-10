using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HalfCutBehavior", menuName = "Levels/Behaviours/HalfCutBehavior")]
public class HalfCutBehavior : LevelBehaviour
{
    [FMODUnity.EventRef]
    public string musicEvent;

    public void PlaySecondAutumnTheme()
    {
        FindObjectOfType<AudioMaster>().PlaySongEvent(musicEvent);
    }
}
