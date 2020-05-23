using UnityEngine;

[CreateAssetMenu(fileName = "SnowstormBehaviour", menuName = "Levels/Behaviours/SnowstormBehaviour")]
public class SnowstormBehaviour : LevelBehaviour
{
	public void AddSnowstormComponent() => Player.Instance.cam.gameObject.AddComponent<Snowstorm>();
}
