using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MirrorPushableBehavior", menuName = "Levels/Behaviours/MirrorPushableBehavior")]
public class MirrorPushableBehavior : LevelBehaviour
{
    // Start is called before the first frame update
    public void Init()
    {
        FindObjectOfType<AudioMaster>().SetMusicParameter("Final Level", 1);
    }
}
