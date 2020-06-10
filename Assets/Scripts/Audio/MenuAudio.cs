using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [Header("Behavior")]
    public float sliderCooldown;


    [Header("Events")]

    [FMODUnity.EventRef]
    public string mouseDown;

    [FMODUnity.EventRef]
    public string mouseUp;

    [FMODUnity.EventRef]
    public string mouseUpBack;

    [FMODUnity.EventRef]
    public string mouseDragged;

    [FMODUnity.EventRef]
    public string pageTurn;

    [FMODUnity.EventRef]
    public string highlight;


    private float timeOfLastSliderChange;

    private void Start()
    {
        timeOfLastSliderChange = Time.realtimeSinceStartup;
    }

    public void playMouseDown()
    {
        FMODUnity.RuntimeManager.PlayOneShot(mouseDown);
    }

    public void playMouseUpBack()
    {
        FMODUnity.RuntimeManager.PlayOneShot(mouseUpBack);
    }

    public void playMouseUp()
    {
        FMODUnity.RuntimeManager.PlayOneShot(mouseUp);
    }

    public void playMouseDragged()
    {
        if(Time.realtimeSinceStartup > timeOfLastSliderChange + sliderCooldown)
        {
            timeOfLastSliderChange = Time.realtimeSinceStartup;
            FMODUnity.RuntimeManager.PlayOneShot(mouseDragged);
        }
    }

    public void playPageTurn()
    {
        FMODUnity.RuntimeManager.PlayOneShot(pageTurn);
    }

    public void playHighlight()
    {
        FMODUnity.RuntimeManager.PlayOneShot(highlight);
    }
}
