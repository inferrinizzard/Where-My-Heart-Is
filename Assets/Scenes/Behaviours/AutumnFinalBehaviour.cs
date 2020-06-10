using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "AutumnFinalBehaviour", menuName = "Levels/Behaviours/AutumnFinalBehaviour")]
public class AutumnFinalBehaviour : LevelBehaviour
{
    [FMODUnity.EventRef]
    public string musicEvent;

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

    public void PlayMusic()
    {
        FindObjectOfType<AudioMaster>().PlaySongEvent(musicEvent);
    }

	public void HeartBreak()
	{
		Player.Instance.hands.leftHandOn = false;;
	}

	public void AssignHeartMesh(Mesh halfHeart)
	{
		Player.Instance.heartWindow.GetComponent<MeshFilter>().mesh = halfHeart;
	}
}
