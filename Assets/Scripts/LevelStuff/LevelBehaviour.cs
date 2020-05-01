using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBehaviour : ScriptableObject
{
    public virtual void onLevelLoad() { }
    public virtual void onLevelUnload() { }
}
