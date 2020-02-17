using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Drop item state. </summary>
public class Drop : PlayerState
{
    /// <summary> Constructor. </summary>
    /// <param name="_player"> Reference to player. </param>
    public Drop(Player _player) : base(_player) {}

    public override void Start()
    {
        // Drop the object.
        player.heldObject.Interact();

        player.heldObject.active = false;
    }
}
