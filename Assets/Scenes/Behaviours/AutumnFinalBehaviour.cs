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
		Player.Instance.hands.leftHandOn = false;
	}

	public void AssignHeartMesh(Mesh halfHeart)
	{
		Player.Instance.heartWindow.GetComponent<MeshFilter>().mesh = halfHeart;
		Player.Instance.heartWindow.transform.Translate(Vector3.right * .25f);

		// var maskRT = Player.Instance.mask.mask;

		// var half = new Texture2D(maskRT.width, maskRT.height);

		// RenderTexture.active = maskRT;
		// half.ReadPixels(new Rect(0, 0, maskRT.width, maskRT.height), 0, 0);
		// for (int i = 0; i < maskRT.width; i++)
		// 	for (int j = 0; j < maskRT.height; j++)
		// 		half.SetPixel(i, j, i < maskRT.width / 2 ? Color.black : Color.white);
		// half.Apply();
		// RenderTexture.active = null;

		// Graphics.Blit(Player.Instance.mask.mask, maskRT);
		// Player.Instance.mask.SetMask(maskRT);

		Player.Instance.mask.SetHalf(true);
	}
}
