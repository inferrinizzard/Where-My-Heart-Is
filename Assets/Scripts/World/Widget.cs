using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * <summary>
 * A Widget is any interactable object that only exists to give the player flavor.
 * It can optionally: play a sound, trigger a dialog
 * </summary>
 * */
[RequireComponent(typeof(AudioSource))]
public class Widget : InteractableObject
{
    public List<string> messages;
    public AudioClip sound;

    public override void Interact(PlayerManager player)
    {
        if(messages.Count > 0)
        {
            // send messages to dialog system
        }

        if(sound != null)
        {
            GetComponent<AudioSource>().PlayOneShot(sound);
        }
    }
}
