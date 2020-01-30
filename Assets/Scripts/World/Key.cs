using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Pickupable
{

    public Lock _lock;

    private Transform originPosition;

    void Awake()
    {
        originPosition = transform;
    }

    void Update()
    {
        if (active)
        {
            // If the object is being held, run Holding.
            if (player.holding) Holding();

            // If the object is being inspected, run Looking.
            if (player.looking) Looking();
        }
    }

    public override void Interact()
    {
        if (!player.holding)
        {
            player.holding = true;
        }
        else if (player.looking)
        {
            player.looking = false;
        }
        else
        {
            transform.parent = null;

            if(Vector3.Distance(transform.position, _lock.transform.position) < 2 && _lock != null)
            {
                this.gameObject.SetActive(false);
                _lock.Interact();
            }
            else
            {
                Debug.Log("Oops you dropped this");
                transform.position = originPosition.position;
                transform.rotation = originPosition.rotation;
            }
            player.holding = false;
        }
    }
}
