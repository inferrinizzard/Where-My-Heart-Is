using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public PlayerTrigger trigger;
    public BirbAnimTester birdAnim;
    public CanvasObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        trigger.OnPlayerEnter += birdAnim.StartNextCurve;
    }

    public void SetCanvas(CanvasObject canvas)
    {
        this.canvas = canvas;
        Player.Instance.playerCanMove = false;
        Player.Instance.windowEnabled = true;
        Player.Instance.characterController.Move(Vector3.zero);
        Player.Instance.interactPrompt.GetComponent<Text>().text = "Right Click or Left CTRL to Open Lens";
        InputManager.OnAltAimKeyDown += SetPrompt;
        InputManager.OnRightClickDown += SetPrompt;

    }

    public void SetPrompt()
    {
        Player.Instance.interactPrompt.GetComponent<Text>().text = "Left Click to Materialize";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
