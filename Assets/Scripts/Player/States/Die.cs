using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Die state. </summary>
public class Die : PlayerState
{
	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public Die(Player _player) : base(_player) { }

	public override void Start()
	{
		if (player.transform.position.y < player.deathPlane.position.y)
		{
			if (player.lastSpawn == null)Debug.LogWarning("Missing spawn point");

			// Set the player position to the spawnpoint
			player.transform.position = player.lastSpawn == null ? Vector3.zero : player.lastSpawn.position;
			player.verticalVelocity = 0;

			// Set the player rotation to the spawnpoint
			player.rotationX = player.lastSpawn.rotation.x;
			player.rotationY = player.lastSpawn.rotation.y;
		}
	}
}
