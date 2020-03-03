using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : Pickupable
{
    public float placeDistanceThreshold;
    public GameObject placeTarget;
    public BirbAnimTester birdAnim;
    public IntroController introController;

    public override void Interact()
    {
        base.Interact();

        if (PlaceConditionsMet())
        {
            PutDown();
            transform.parent = placeTarget.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            CanvasObject canvas = gameObject.AddComponent<CanvasObject>();
            canvas.manualTarget = "Bridge";
            introController.SetCanvas(canvas);
            Destroy(this);
        }
        else if(Player.Instance.heldObject == this)
        {
            birdAnim.StartNextCurve();
        }
    }

    public bool PlaceConditionsMet()
    {
        return Vector3.Distance(transform.position, placeTarget.transform.position) < placeDistanceThreshold;
    }
}
