using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestBehaviour", menuName = "Levels/Behaviours/TestBehaviour")]
public class TestBehaviour : LevelBehaviour
{
    public override void onLevelLoad()
    {
        base.onLevelLoad();

        Debug.Log("i will do the laundry eventually");
    }

    public override void onLevelUnload()
    {
        base.onLevelUnload();

        Debug.Log("i want pizza hut stuffed crust please");
    }
}
