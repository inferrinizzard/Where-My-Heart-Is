using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Pickupable
{

    public Lock _lock;

    private Transform oldParent;

    void Awake()
    {
    }

    void Update()
    {
        if (active)
        {
            // If the object is being held, run Holding.
            //if (player.holding) Holding();

            // If the object is being inspected, run Looking.
            if (player.looking) Looking();
        }
    }

    public override void Interact()
    {
        if (!player.holding)
        {
            player.holding = true;

            // save the old parent to revert to later
            oldParent = transform.parent;
            transform.parent = player.GetHeldObjectLocation();// set the new parent to the hold object location object
            transform.localPosition = Vector3.zero;// set the position to local zero to match the position of the hold object location target
        }
        else if (player.looking)
        {
            player.looking = false;
        }
        else
        {   
            player.holding = false;
            transform.parent = oldParent;

            if (Vector3.Distance(transform.position, _lock.transform.position) < 2 && _lock != null && _lock.gameObject.layer == 9)
            {
                this.gameObject.SetActive(false);
                _lock.Interact();
            }
            else
            {
                Debug.Log("Oops you dropped this");
            }
        }
    }
}
