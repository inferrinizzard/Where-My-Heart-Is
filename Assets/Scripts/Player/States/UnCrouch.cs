using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Uncrouch state. </summary>
public class UnCrouch : PlayerState
{
	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public UnCrouch(Player _player) : base(_player) { }

	public override void Start()
	{
		player.audioController.CrouchUp();
		player.characterController.height = player.playerHeight; // Make the player stand.
		player.crouching = false;
	}
}
