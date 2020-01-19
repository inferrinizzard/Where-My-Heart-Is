using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Pickupable
{
    public WorldManager worldManager;
    public GameObject FieldOfView;

    CSG.Operations csgOperator;

    void Start()
    {
        csgOperator = GetComponent<CSG.Operations>();
    }

    public override void Interact()
    {
        base.Interact();
        if (player.holding == false)
        {
            ApplyCut();
        }
    }

    public void ApplyCut()
    {
        // Debug.Log(worldManager.GetDreamObjects().Count);
        foreach (ClipableObject clippableObject in worldManager.GetRealObjects())
        {
            clippableObject.UnionWith(FieldOfView, csgOperator);
        }

        foreach (ClipableObject clippableObject in worldManager.GetDreamObjects())
        {
            clippableObject.Subtract(FieldOfView, csgOperator);
        }
    }
}
