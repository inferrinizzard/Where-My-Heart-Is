using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestBehaviour", menuName = "Levels/Behaviours/TestBehaviour")]
public class TestBehaviour : LevelBehaviour
{
    public override void OnLevelLoad()
    {
        base.OnLevelLoad();

        Debug.Log("i will do the laundry eventually");
    }

    public override void OnLevelUnload()
    {
        base.OnLevelUnload();

        Debug.Log("i want pizza hut stuffed crust please");
    }
}
