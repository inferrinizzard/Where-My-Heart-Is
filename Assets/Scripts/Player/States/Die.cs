using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Die state. </summary>
public class Die : PlayerState
{
    /// <summary> Constructor. </summary>
    /// <param name="_player"> Reference to player. </param>
    public Die(Player _player) : base(_player) {}

    public override void Start()
    {
        if (player.characterController.velocity.y < -30)
        {
            if (player.lastSpawn == null) Debug.LogWarning("Missing spawn point");
            player.transform.position = player.lastSpawn == null ? Vector3.zero : player.lastSpawn.position; // Reset the player to the spawnpoint.
            player.verticalVelocity = 0;
        }
    }
}
