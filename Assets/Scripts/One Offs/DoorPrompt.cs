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

    /// <summary>
    /// controlled through door prompt buttons
    /// </summary>
    public void HidePrompt()
    {
        displayPrompt = !displayPrompt;
        Prompt.SetActive(displayPrompt);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Load credits scene
    public void StartCredits()
    {
        StartCoroutine(Player.Instance.mask.PreTransition());
    }
}
