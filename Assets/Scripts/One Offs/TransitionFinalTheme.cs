using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFinalTheme : PlayerTrigger
{
    protected override void PlayerEnter(Collider other)
    {
        FindObjectOfType<AudioMaster>().SetMusicParameter("End", 1);
    }
}
