using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	public void Init()
	{
		Player.Instance.windowEnabled = false;
		Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
	}
	public void EnableFog() => Effects.Instance.ToggleFog(true);
	public void DisableFog() => Effects.Instance.ToggleFog(false);

	public void Exit()
	{
		Player.Instance.windowEnabled = true;
	}
}
