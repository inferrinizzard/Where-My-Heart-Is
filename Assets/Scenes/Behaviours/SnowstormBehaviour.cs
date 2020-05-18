using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	public static float walkDistance = 100;
	public void AddSnowstormComponent() => Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
}
