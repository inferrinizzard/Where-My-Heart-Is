using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutumnFinalController : MonoBehaviour
{
    public PlayerTrigger zone1;
    public PlayerTrigger zone2;
    public PlayerTrigger zone3;

    private float songState = 0;

    private bool initialized = false;

    private void Update()
    {
        if(!initialized)
        {
            AudioMaster.Instance.SetNoSong();
            Player.Instance.audioController.SetWindowWorld(1);
            initialized = true;

            AudioMaster.Instance.StartTensionTheme();

            zone1.OnPlayerEnter += ProgressSongTo1;
            zone2.OnPlayerEnter += ProgressSongTo2;
            zone3.OnPlayerEnter += ProgressSongTo3;
        }
    }

    public void ProgressSongTo1()
    {
        if(songState < 1)
        {
            songState = 1;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 1);
        }
    }

    public void ProgressSongTo2()
    {
        if (songState < 2)
        {
            songState = 2;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 2);
        }
    }

    public void ProgressSongTo3()
    {
        if (songState < 3)
        {
            songState = 3;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 3);
        }
    }
}
