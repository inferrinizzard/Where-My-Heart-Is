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
    public string SpringAmbientEvent;

    [FMODUnity.EventRef]
	public string TensionMusicEvent;

	[FMODUnity.EventRef]
	public string IntroMusicEvent;

	[FMODUnity.EventRef]
	public string MainThemeEvent;

	private FMOD.Studio.EventInstance ambientInstance;
	private FMOD.Studio.EventInstance musicInstance;

    private string nextEventName;
    private bool playingSong;

	// Start is called before the first frame update
	void Start()
	{
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(AutumnAmbientEvent);
		ambientInstance.setParameterByName("Play Song", 1);
		ambientInstance.start();

        playingSong = false;
    }

    public void PlaySongEvent(string eventName)
    {
        nextEventName = eventName;
        if(playingSong)
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Invoke("StartNextSong", 1);
        }
        else
        {
            StartNextSong();
        }
    }

    private void StartNextSong()
    {
        musicInstance.release();
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(nextEventName);
        musicInstance.start();

        playingSong = true;
    }

    public void StopSongEvent()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Invoke("ReleaseSongEvent", 1);
    }

    public void SetMusicParameter(string parameterName, float value)
    {
        musicInstance.setParameterByName(parameterName, value);
    }

    private void ReleaseSongEvent()
    {
        musicInstance.release();
        playingSong = false;
    }

    public void StartTensionTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		musicInstance = FMODUnity.RuntimeManager.CreateInstance(TensionMusicEvent);
		musicInstance.start();
	}

	public void StartIntroTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		musicInstance = FMODUnity.RuntimeManager.CreateInstance(IntroMusicEvent);
		musicInstance.start();
	}

	public void StartMainTheme()
	{
		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
		ambientInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(WinterAmbientEvent);
		ambientInstance.start();
	}

	public void StopAll()
	{
		ambientInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	public void SetSpring()
	{
		ambientInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		ambientInstance = FMODUnity.RuntimeManager.CreateInstance(SpringAmbientEvent);
		ambientInstance.start();
	}

	public void SetNoSong()
	{
		ambientInstance.setParameterByName("Play Song", 0);
	}
}
