using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	public void Init()
	{
		Player.Instance.windowEnabled = false;
		Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
	}
	public void EnableFog() => Player.VFX.ToggleFog(true);
	public void DisableFog() => Player.VFX.ToggleFog(false);

	public void Exit()
	{
		Player.Instance.windowEnabled = true;
	}
}
