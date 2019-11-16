using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    // for now a count will do, but this needs to be replaced with something better
    public List<CollectableObject> expectedCollections;

    public List<CollectableObject> collections;

    private void Start()
    {
        
    }

    public void Collect(CollectableObject collectable)
    {
        collections.Add(collectable);
        // send interact text to dialog machine

    }
}
