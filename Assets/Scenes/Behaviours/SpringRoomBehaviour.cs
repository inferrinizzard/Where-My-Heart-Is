using System.Collections;

using UnityEngine;

[CreateAssetMenu(fileName = "SpringRoomBehaviour", menuName = "Levels/Behaviours/SpringRoomBehaviour")]
public class SpringRoomBehaviour : LevelBehaviour
{
    public void Init()
    {
        Debug.Log("hy");
        FindObjectOfType<AudioMaster>().SetSpring();
        Player.Instance.audioController.realSurface = 3;
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
