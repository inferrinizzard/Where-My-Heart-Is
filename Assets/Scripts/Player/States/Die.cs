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
		if (!player.deathPlane)
		{
			Debug.LogWarning("Missing death plane!");
			return;
		}

		if (player.transform.position.y < player.deathPlane.position.y)
		{
			if (player.lastSpawn)
			{
				// Set the player position to the spawnpoint
				player.transform.position = player.lastSpawn ? player.lastSpawn.position : Vector3.zero;
				player.verticalVelocity = 0;

				// Set the player rotation to the spawnpoint
				player.rotationX = player.lastSpawn.rotation.x;
				player.rotationY = player.lastSpawn.rotation.y;
			}
			else
				Debug.LogWarning("Missing spawn point!");
		}
	}
}
