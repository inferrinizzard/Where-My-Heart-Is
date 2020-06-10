using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
	public float landingDistanceThreshold;

    public float maxWalkSpeed;
    public float minWalkSpeed;

    public float maxFallSpeed;
    public float minFallSpeed;

    public float dieVelocityThreshold;
    public float dieDistanceThreshold;

    public float minCutLength;


    [Header("Movement Events")]
	[FMODUnity.EventRef]
	public string jumpLiftoffEvent;

	[FMODUnity.EventRef]
	public string jumpLandingEvent;

    [FMODUnity.EventRef]
    public string fallEvent;

    [FMODUnity.EventRef]
	public string walkEvent;

    [FMODUnity.EventRef]
    public string dieEvent;

    [FMODUnity.EventRef]
    public string cutEvent;

    [Header("Window Events")]
	[FMODUnity.EventRef]
	public string windowEvent;

	private FMOD.Studio.EventInstance walkInstance;
	private FMOD.Studio.EventInstance windowInstance;
	private FMOD.Studio.EventInstance fallInstance;
	private FMOD.Studio.EventInstance cutInstance;

    private WalkingSurface.Surface currentSurface;

    private bool falling = false;
    private bool jumping = false;
    private bool fallVoicePlayed = false;

    private float timeOfLastCutStart;

	private void Start()
	{
		walkInstance = FMODUnity.RuntimeManager.CreateInstance(walkEvent);
		walkInstance.start();

		windowInstance = FMODUnity.RuntimeManager.CreateInstance(windowEvent);
		windowInstance.setParameterByName("Heart World Type", 2);
		windowInstance.start();

        cutInstance = FMODUnity.RuntimeManager.CreateInstance(cutEvent);

        fallInstance = FMODUnity.RuntimeManager.CreateInstance(fallEvent);

        currentSurface = WalkingSurface.Surface.Stone;

        Player player = Player.Instance;
        player.OnJump += JumpLiftoff;
        player.OnApplyCut += ApplyCut;
        Debug.Log(player.window);
        player.GetComponent<Window>().OnCompleteCut += CompleteCut;

        //FMODUnity.RuntimeManager.PlayOneShot("event:/Music/Autumn 1");

    }

    private void Update()
	{
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        bool raycast = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, 1 << 9);

        if (raycast && hit.distance < landingDistanceThreshold)
		{
			if (hit.transform.TryComponent(out WalkingSurface surface))
			{
				currentSurface = surface.surface;
				walkInstance.setParameterByName("Surface", (float) surface.surface);
			}
			else
			{
				currentSurface = WalkingSurface.Surface.Stone;
				walkInstance.setParameterByName("Surface", (float) WalkingSurface.Surface.Stone);
			}

            walkInstance.setParameterByName("Velocity", Mathf.Clamp((rigidbody.velocity.magnitude - minWalkSpeed) / (maxWalkSpeed - minWalkSpeed), 0, 1));

            fallInstance.setParameterByName("Speed", 0);

            if(jumping && rigidbody.velocity.y <= 0 && Player.Instance.IsGrounded())
            {
                JumpLanding();
                jumping = false;
                fallVoicePlayed = false;
            }

            falling = false;
        }
        else
        {

            if (!falling)
            {
                falling = true;
                fallInstance.start();
            }
            walkInstance.setParameterByName("Velocity", 0);

            fallInstance.setParameterByName("Speed", Mathf.Clamp((-rigidbody.velocity.y - minFallSpeed) / (maxFallSpeed - minFallSpeed), 0, 1));

            if(!fallVoicePlayed && rigidbody.velocity.y < dieVelocityThreshold && 
                (hit.distance > dieDistanceThreshold || !raycast))
            {
                FMODUnity.RuntimeManager.PlayOneShot(dieEvent);
                fallVoicePlayed = true;
            }
        }

        walkInstance.setParameterByName("Velocity", Mathf.Clamp((GetComponent<Rigidbody>().velocity.magnitude - minWalkSpeed) / (maxWalkSpeed - minWalkSpeed), 0, 1));
	}

	public void SetWindowWorld(float worldIndex)
	{
		windowInstance.setParameterByName("Heart World Type", worldIndex);
	}

	public void OpenWindow()
	{
		windowInstance.setParameterByName("WindowState", 1);
	}

	public void PlaceWindow()
	{
		windowInstance.setParameterByName("WindowState", 1);
		CloseWindow();
	}

	public void CloseWindow()
	{
		windowInstance.setParameterByName("WindowState", 0);
	}

	public void JumpLiftoff()
	{
		FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(jumpLiftoffEvent);
		instance.setParameterByName("Surface", (float) currentSurface);
		instance.start();
		windowInstance.setParameterByName("WindowState", 0);
		jumping = true;
	}

	public void JumpLanding()
	{
		FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(jumpLandingEvent);
		instance.setParameterByName("Surface", (float) currentSurface);
		instance.start();
	}

	public void CrouchDown()
	{
		//FMODUnity.RuntimeManager.PlayOneShot(CrouchDownEvent, transform.position);
	}

	public void CrouchUp()
	{
		//FMODUnity.RuntimeManager.PlayOneShot(CrouchUpEvent, transform.position);
	}

    private void ApplyCut()
    {
        timeOfLastCutStart = Time.time;
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cut In Progress", 1);
        cutInstance.start();
    }

    private void CompleteCut()
    {
        if(Time.time > timeOfLastCutStart + minCutLength)
        {
            SetCutToZero();
        }
        else
        {
            Invoke("SetCutToZero", timeOfLastCutStart + minCutLength - Time.time);
        }
    }

    private void SetCutToZero()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cut In Progress", 0);
    }

    public void SetWalkingVelocity(float value)
	{
		if (jumping)
		{
			walkInstance.setParameterByName("Velocity", 0);
			return;
		}
		walkInstance.setParameterByName("Velocity", value);
	}
}
