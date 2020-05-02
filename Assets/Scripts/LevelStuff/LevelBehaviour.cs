using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBehaviour : ScriptableObject
{
    public virtual void OnLevelLoad() { }
    public virtual void OnLevelUnload() { }
}
