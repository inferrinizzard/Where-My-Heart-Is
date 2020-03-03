using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public event Action OnPlayerEnter;
    public bool destroyAfterTrigger;
    public DialogueSystem dialogueSystem;

    public string flavor;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
        if(flavor != "")
        {
            StartCoroutine(dialogueSystem.WriteDialogue(flavor));
        }
            if(destroyAfterTrigger)
            {
                Destroy(this);
            }
        }
    }
}
