using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnableAllHands : MonoBehaviour
{
    public List<GameObject> stuff;

    public void Go()
    {
        stuff.ForEach(thing => thing.SetActive(true));
    }
}
