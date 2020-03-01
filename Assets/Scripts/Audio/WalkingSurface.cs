using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingSurface : MonoBehaviour
{
    public enum Surface
    {
        Leaves,
        Stone,
        Grass,
        Snow,
        Carpet
    }

    public Surface surface;
}
