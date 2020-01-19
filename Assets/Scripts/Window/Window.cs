using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Pickupable
{
    public WorldManager worldManager;
    public GameObject FieldOfView;
    public override void Interact(PlayerMovement player)
    {
        base.Interact(player);
        if(holding == false)
        {
            ApplyCut();
        }
    }

    public void ApplyCut()
    {
        Debug.Log(worldManager.GetDreamObjects().Count);
        foreach(ClipableObject clippableObject in worldManager.GetRealObjects())
        {
            clippableObject.UnionWith(FieldOfView, GetComponent<CSG.Operations>());
        }
        
        foreach (ClipableObject clippableObject in worldManager.GetDreamObjects())
        {
            clippableObject.Subtract(FieldOfView, GetComponent<CSG.Operations>());
        }
    }
}
