using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public event Action OnPlayerEnter;
    public bool destroyAfterTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnPlayerEnter();
            if(destroyAfterTrigger)
            {
                Destroy(this);
            }
        }
    }
}
