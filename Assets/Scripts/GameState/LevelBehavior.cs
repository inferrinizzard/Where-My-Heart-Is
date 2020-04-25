using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBehavior : MonoBehaviour
{
    public virtual void onLevelLoad() { }
    public virtual void onLevelUnload() { }
}
