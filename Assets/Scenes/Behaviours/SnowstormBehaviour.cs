using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
    [FMODUnity.EventRef]
    public string musicEvent;

	public void Init()
	{
		Player.Instance.windowEnabled = false;
		Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
        FindObjectOfType<AudioMaster>().PlaySongEvent(musicEvent);
	}
	public void EnableFog() => Player.VFX.ToggleFog(true);
	public void DisableFog() => Player.VFX.ToggleFog(false);

	public void Exit()
	{
		Player.Instance.windowEnabled = true;
	}
}
