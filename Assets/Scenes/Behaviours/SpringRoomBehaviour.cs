using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "SpringRoomBehaviour", menuName = "Levels/Behaviours/SpringRoomBehaviour")]
public class SpringRoomBehaviour : LevelBehaviour
{
	public void Init(Mesh wholeHeart)
	{
		// Debug.Log("hy");
		Player.Instance.mask.SetHalf(false);
		FindObjectOfType<AudioMaster>().SetSpring();
        Player.Instance.heartWindow.GetComponent<MeshFilter>().mesh = wholeHeart;
        Player.Instance.heartWindow.transform.Translate(Vector3.right * .25f);

        Player.Instance.mask.SetHalf(false);
        FindObjectOfType<EnableAllHands>().Go();
    }

	// void Credits()
	// {
	//     Player.Instance.cam.gameObject.AddComponent<FadeOut>();
	//     FadeOut();
	// }

	// IEnumerator FadeOut()
	// {
	//     float fadeTime = Player.Instance.cam.gameObject.GetComponent<FadeOut>().BeginFade(1);
	//     yield return new WaitForSeconds(fadeTime);
	// }

}
