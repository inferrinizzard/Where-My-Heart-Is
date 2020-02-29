using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKey : Pickupable
{
    public Gate gate;
    public float distanceThreshold;


    public void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();

        if (Vector3.Distance(transform.position, gate.keyHole.transform.position) < distanceThreshold)
        {
            gate.Open();
            Destroy(gameObject);
        }
    }
}
