using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPrompt : InteractableObject
{
    public GameObject Prompt;
    private bool displayPrompt = false;

    public override void Interact()
    {
        displayPrompt = !displayPrompt;
        Prompt.SetActive(displayPrompt);
        Cursor.visible = displayPrompt;
 
        if(displayPrompt)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public void hidePrompt()
    {
        displayPrompt = !displayPrompt;
        Prompt.SetActive(displayPrompt);
    }
}
