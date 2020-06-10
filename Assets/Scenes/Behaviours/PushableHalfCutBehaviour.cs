using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PushableHalfCutBehaviour", menuName = "Levels/Behaviours/PushableHalfCutBehaviour")]
public class PushableHalfCutBehaviour : LevelBehaviour
{
    public void Init()
    {
        Player.Instance.audioController.SetWindowWorld(1);
    }
}
