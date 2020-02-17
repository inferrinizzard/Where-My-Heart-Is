using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Jump state. </summary>
public class Jump : PlayerState
{
    /// <summary> Constructor. </summary>
    /// <param name="_player"> Reference to player. </param>
    public Jump(Player _player) : base(_player) {}

    public override void Start()
    {
        player.verticalVelocity = player.jumpForce;
        player.GetComponent<PlayerAudio>().JumpLiftoff();
        player.jumping = true;

        // Landing sound.
        PlayerAudio audioManager = player.GetComponent<PlayerAudio>();
        if (player.jumping)
        {
            RaycastHit hit;
            int mask = ~player.gameObject.layer;
            Physics.Raycast(new Ray(player.transform.position, Vector3.down), out hit, 5f, mask);
            if (player.verticalVelocity < 0 && hit.distance < audioManager.landingDistanceThreshold)
            {
                player.GetComponent<PlayerAudio>().JumpLanding();
                player.jumping = false;
            }
        }
    }
}
