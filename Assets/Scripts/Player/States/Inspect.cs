using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Inspect object state. </summary>
public class Inspect : PlayerState
{
    /// <summary> Constructor. </summary>
    /// <param name="_player"> Reference to player. </param>
    public Inspect(Player _player) : base(_player) {}

    public override void Start()
    {
        // Stop inspecting the object (??)
        player.heldObject.Interact();

        player.playerCanMove = true;
    }
}
