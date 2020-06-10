using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorPrompt : InteractableObject
{
    public GameObject Prompt;
    public GameObject Credits;
    private bool displayPrompt = false;


    public override void Interact()
    {
        displayPrompt = !displayPrompt;
        Prompt.SetActive(displayPrompt);
        Cursor.visible = displayPrompt;

        if (displayPrompt)
        {
            Cursor.lockState = CursorLockMode.None;
            Player.Instance.canMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// controlled through door prompt buttons
    /// </summary>
    public void HidePrompt()
    {
        displayPrompt = !displayPrompt;
        Prompt.SetActive(displayPrompt);
        Cursor.lockState = CursorLockMode.Locked;
        Player.Instance.canMove = true;
    }

    // Load credits scene
    public void StartCredits()
    {
        Credits.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Player.Instance.canMove = false;
        GameManager.Instance.pause.gameplayUI.SetActive(false);
    }
}
