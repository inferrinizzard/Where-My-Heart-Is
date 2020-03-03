using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public PlayerTrigger trigger;
    public BirbAnimTester birdAnim;
    public CanvasObject canvas;

    private bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        trigger.OnPlayerEnter += birdAnim.StartNextCurve;
    }

    public void SetCanvas(CanvasObject canvas)
    {
        this.canvas = canvas;
        canvas.OnInteract += PlayTheme;
        Player.Instance.playerCanMove = false;
        Player.Instance.windowEnabled = true;
        Player.Instance.characterController.Move(Vector3.zero);
        Player.Instance.interactPrompt.GetComponent<Text>().text = "Right Click or Left CTRL to Open Lens";
        InputManager.OnAltAimKeyDown += SetPrompt;
        InputManager.OnRightClickDown += SetPrompt;
    }

    public void PlayTheme()
    {
        AudioMaster.Instance.StartMainTheme();
    }

    public void SetPrompt()
    {
        Player.Instance.interactPrompt.GetComponent<Text>().text = "Left Click to Materialize";
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            AudioMaster.Instance.SetAmbientVariable("Play Song", 0);
        }

    }
}
