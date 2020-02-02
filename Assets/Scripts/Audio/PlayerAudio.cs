using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public float landingDistanceThreshold;

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

    private FMOD.Studio.EventInstance walkInstance;

    private void Start()
    {
        walkInstance = FMODUnity.RuntimeManager.CreateInstance(WalkEvent);
        walkInstance.start();
    }

    public void PlayJumpLiftoff()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpLiftoffEvent, transform.position);
    }

    public void PlayJumpLanding()
    {
        FMODUnity.RuntimeManager.PlayOneShot(JumpLandingEvent, transform.position);
    }

    public void PlayCrouchDown()
    {
        FMODUnity.RuntimeManager.PlayOneShot(CrouchDownEvent, transform.position);
    }

    public void PlayCrouchUp()
    {
        FMODUnity.RuntimeManager.PlayOneShot(CrouchUpEvent, transform.position);
    }

    public void SetWalkingVelocity(float value)
    {
        walkInstance.setParameterByName("Velocity", value);
    }
}