using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Cut state. </summary>
public class Cut : PlayerState
{
	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public Cut(Player _player) : base(_player) { }

	public override void Start()
	{
		player.window.ApplyCut();
		player.audioController.PlaceWindow();
		player.heartWindow.SetActive(false);
		player.VFX.ToggleMask(false);
		player.aiming = false;
	}
}
