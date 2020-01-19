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

    private int messageChoice = 0;

    public override void Interact(PlayerMovement player)
    {
        if(messages.Count > 0)
        {
            FindObjectOfType<MessageWriter>().WriteMessage(SelectMessage());
        }

        if(sound != null)
        {
            GetComponent<AudioSource>().PlayOneShot(sound);
        }
    }

    private string SelectMessage()
    {
        if(messageChoice == 0)
        {
            messageChoice = 1;
        }
        else
        {
            messageChoice = 0;
        }
        return messages[messageChoice];
        //return messages[Mathf.RoundToInt(Random.Range(0, messages.Count))];
    }
}
