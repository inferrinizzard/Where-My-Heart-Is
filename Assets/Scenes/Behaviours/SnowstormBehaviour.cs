using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	[FMODUnity.EventRef]
	public string musicEvent;

	[FMODUnity.EventRef]
	public string shiverEvent;

	private FMOD.Studio.EventInstance shiverInstance;

	public void Init()
	{
		Player.Instance.windowEnabled = false;
		Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
		Player.Instance.audioController.SetWindowWorld(0);
		AudioMaster audioMaster = FindObjectOfType<AudioMaster>();
		audioMaster.PlaySongEvent(musicEvent);
		audioMaster.SetWinter();

		shiverInstance = FMODUnity.RuntimeManager.CreateInstance(shiverEvent);
		shiverInstance.start();
	}
	public void EnableFog() => Player.VFX.ToggleFog(true);
	public void DisableFog() => Player.VFX.ToggleFog(false);

	public void Exit()
	{
		Player.Instance.windowEnabled = true;
		Destroy(Player.Instance.GetComponentInChildren<Snowstorm>());
		shiverInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		shiverInstance.release();
	}
}
