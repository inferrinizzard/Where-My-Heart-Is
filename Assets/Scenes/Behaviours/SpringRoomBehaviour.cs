using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SpringRoomBehaviour", menuName = "Levels/Behaviours/SpringRoomBehaviour")]
public class SpringRoomBehaviour : LevelBehaviour
{

    void Credits()
    {
        Player.Instance.cam.gameObject.AddComponent<FadeOut>();
        FadeOut();
    }

    IEnumerator FadeOut()
    {
        float fadeTime = Player.Instance.cam.gameObject.GetComponent<FadeOut>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
    }
    
}
