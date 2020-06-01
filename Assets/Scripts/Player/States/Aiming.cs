using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Aiming heart window state. </summary>
public class Aiming : PlayerState
{
	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public Aiming(Player _player) : base(_player) { }

	public override void Start()
	{
		// Make the window visible.
		// player.heartWindow.SetActive(true);
		// Player.VFX.ToggleMask(true);
		// player.audioController.OpenWindow();
	}

	public void DeferredStart()
	{
		player.heartWindow.SetActive(true);
		Player.VFX.ToggleMask(true);
		player.audioController.OpenWindow();
	}

	public override void End()
	{
		player.hands.ToggleHands(false);
		player.hands.RevertAim();
		player.heartWindow.SetActive(false);
		Player.VFX.ToggleMask(false);
		player.audioController.CloseWindow();
	}
}
