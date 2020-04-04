using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AudioMaster : Singleton<AudioMaster>
{
	[FMODUnity.EventRef]
	public string AutumnAmbientEvent;

	[FMODUnity.EventRef]
	public string WinterAmbientEvent;

	[FMODUnity.EventRef]
	public string TensionMusicEvent;

	[FMODUnity.EventRef]
	public string IntroMusicEvent;

	[FMODUnity.EventRef]
	public string MainThemeEvent;

	private FMOD.Studio.EventInstance ambientInstance;
	private FMOD.Studio.EventInstance musicInstance;

	// Start is called before the first frame update
	void Start()
	{
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(AutumnAmbientEvent);
		ambientInstance.setParameterByName("Play Song", 1);
		ambientInstance.start();
		StartIntroTheme();
	}

	public void StartTensionTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		musicInstance = FMODUnity.RuntimeManager.CreateInstance(TensionMusicEvent);
		musicInstance.start();
	}

	public void StartIntroTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		musicInstance = FMODUnity.RuntimeManager.CreateInstance(IntroMusicEvent);
		musicInstance.start();
	}

	public void StartMainTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		musicInstance = FMODUnity.RuntimeManager.CreateInstance(MainThemeEvent);
		musicInstance.start();
	}

	public void SetMusicVariable(string name, float value)
	{
		musicInstance.setParameterByName(name, value);
	}

	public void SetAmbientVariable(string name, float value)
	{
		ambientInstance.setParameterByName(name, value);
	}

	public void SetWinter()
	{
		ambientInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(WinterAmbientEvent);
		ambientInstance.start();
	}

	public void StopAll()
	{
		ambientInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	public void SetSpring()
	{
		ambientInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(WinterAmbientEvent);
		ambientInstance.start();
	}

	public void SetNoSong()
	{
		ambientInstance.setParameterByName("Play Song", 0);
	}
}
