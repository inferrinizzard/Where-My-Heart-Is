using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutumnFinalController : MonoBehaviour
{
    public PlayerTrigger zone1;
    public PlayerTrigger zone2;
    public PlayerTrigger zone3;
    public PlayerTrigger zone4;
    public BirbAnimTester birbAnim;

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
            zone4.OnPlayerEnter += FinalBirdMove;

            birbAnim.StopChirps();
        }
    }

    public void ProgressSongTo1()
    {
        if(songState < 1)
        {
            birbAnim.StartNextCurve();
            songState = 1;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 1);
        }
    }

    public void ProgressSongTo2()
    {
        if (songState < 2)
        {
            birbAnim.StartNextCurve(); 
            songState = 2;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 2);
        }
    }

    public void ProgressSongTo3()
    {
        if (songState < 3)
        {
            birbAnim.StartNextCurve();
            songState = 3;
            AudioMaster.Instance.SetMusicVariable("Autumn Tension State", 3);
        }
    }

    public void FinalBirdMove()
    {
        birbAnim.StartNextCurve();
    }

   
}
