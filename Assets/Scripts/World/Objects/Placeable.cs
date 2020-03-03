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

        if(PlaceConditionsMet())
        {
            PutDown();
            transform.parent = placeTarget.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            gameObject.AddComponent<CanvasObject>().manualTarget = "Bridge";
            Destroy(this);
        }
    }

    public bool PlaceConditionsMet()
    {
        return Vector3.Distance(transform.position, placeTarget.transform.position) < placeDistanceThreshold;
    }
}
