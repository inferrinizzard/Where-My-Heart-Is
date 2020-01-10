using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : InteractableObject
{
    public RealConfig realConfiguration;
    public DreamConfiguration dreamConfiguration;
    public GameObject FieldOfView;
    public override void Interact(PlayerManager player)
    {
        if(transform.parent == null)
        {
            ApplyCut();
            transform.parent = player.transform;
        }
        else
        {
            transform.parent = null;
        }
    }

    public void ApplyCut()
    {
        // for all real configuration children
            // cut using CSG.Operations.Union
                //
            // object 1: the current child
            // object 2: Test field
        foreach(ClipableObject clippableObject in realConfiguration.GetClipableObjects())
        {
            clippableObject.UnionWith(FieldOfView);
        }

        // for all dream configuration children
            // cut using CSG.Operations.Subtract (object - field)// when available
            // object 1: the current child
            // object 2: Test field


        // remove all previous cut results

    }
}
