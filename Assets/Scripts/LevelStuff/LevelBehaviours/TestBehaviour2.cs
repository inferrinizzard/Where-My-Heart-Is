using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestBehaviour2", menuName = "Levels/Behaviours/TestBehaviour2")]
public class TestBehaviour2 : LevelBehaviour
{
    public override void OnLevelLoad()
    {
        base.OnLevelLoad();
        Debug.Log("we get those dubs bois");
    }

    public override void OnLevelUnload()
    {
        base.OnLevelUnload();
        Debug.Log("imagine having a tail amirite");
    }
}
