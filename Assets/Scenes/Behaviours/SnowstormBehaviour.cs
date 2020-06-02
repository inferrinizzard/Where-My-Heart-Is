using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	public void AddSnowstormComponent() => Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
	public void EnableFog() => Effects.Instance.ToggleFog(true);
	public void DisableFog() => Effects.Instance.ToggleFog(false);
}
