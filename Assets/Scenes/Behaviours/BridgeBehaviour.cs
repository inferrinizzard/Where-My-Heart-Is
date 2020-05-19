using UnityEngine;

[CreateAssetMenu(fileName = "BridgeBehaviour", menuName = "Levels/Behaviours/BridgeBehaviour")]
public class BridgeBehaviour : LevelBehaviour
{
	public void TutorialPrompt()
	{
		GameManager.Instance.prompt.SetText("Press and Hold E or Right Click to use Window");
	}
}
