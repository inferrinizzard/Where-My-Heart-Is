using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "AutumnFinalBehaviour", menuName = "Levels/Behaviours/AutumnFinalBehaviour")]
public class AutumnFinalBehaviour : LevelBehaviour
{
	// public void CloseBirdCage()
	// {
	//     Player.Instance.cam.gameObject.AddComponent<FadeOut>();
	//     FadeOut();
	// }

	// IEnumerator FadeOut()
	// {
	//     float fadeTime = Player.Instance.cam.gameObject.GetComponent<FadeOut>().BeginFade(1);
	//     yield return new WaitForSeconds(fadeTime);
	// }

	public void HeartBreak()
	{
		Player.Instance.hands.leftHandOn = false;;
	}
}
