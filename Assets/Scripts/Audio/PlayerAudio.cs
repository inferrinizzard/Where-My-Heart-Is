using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public float landingDistanceThreshold;

    [Header("Movement Event References")]
    [FMODUnity.EventRef]
    public string JumpLiftoffEvent;

    [FMODUnity.EventRef]
    public string JumpLandingEvent;

    [FMODUnity.EventRef]
    public string CrouchDownEvent;

    [FMODUnity.EventRef]
    public string CrouchUpEvent;

    [FMODUnity.EventRef]
    public string WalkEvent;

    [Header("Window Event References")]
    [FMODUnity.EventRef]
    public string WindowEvent;

    private FMOD.Studio.EventInstance walkInstance;
    private FMOD.Studio.EventInstance windowInstance;

    private void Start()
    {
        walkInstance = FMODUnity.RuntimeManager.CreateInstance(WalkEvent);
        walkInstance.start();

        windowInstance = FMODUnity.RuntimeManager.CreateInstance(WindowEvent);
    }

    public void OpenWindow()
    {
        windowInstance.setParameterByName("WindowState", 1);
        windowInstance.start();
    }

    public void PlaceWindow()
    {
        windowInstance.setParameterByName("WindowState", 2);
    }

    public void CloseWindow()
    {
        windowInstance.setParameterByName("WindowState", 0);
    }

    public void JumpLiftoff()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpLiftoffEvent, transform.position);
    }

    public void JumpLanding()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpLandingEvent, transform.position);
    }

    public void CrouchDown()
    {
        FMODUnity.RuntimeManager.PlayOneShot(CrouchDownEvent, transform.position);
    }

    public void CrouchUp()
    {
        FMODUnity.RuntimeManager.PlayOneShot(CrouchUpEvent, transform.position);
    }

    public void SetWalkingVelocity(float value)
    {
        walkInstance.setParameterByName("Velocity", value);
    }
}