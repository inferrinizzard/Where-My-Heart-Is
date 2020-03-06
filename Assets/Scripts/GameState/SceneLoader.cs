using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private void Update()
    {
        GameManager.Instance.OnCompleteTransition();
        Destroy(this);
    }
}
