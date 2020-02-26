using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : Pickupable
{
    public float placeDistanceThreshold;
    public GameObject placeTarget;

    public override void Interact()
    {
        base.Interact();

        if(Vector3.Distance(transform.position, placeTarget.transform.position) < placeDistanceThreshold)
        {
            PutDown();
            transform.parent = placeTarget.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}
