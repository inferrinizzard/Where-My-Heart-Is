using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Pick up item state. </summary>
public class PickUp : PlayerState
{
	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public PickUp(Player _player) : base(_player) { }

	public override void Start()
	{
		player.pickedUpFirst = true;
		if (player.heldObject.GetComponent<Placeable>() && player.heldObject.GetComponent<Placeable>().PlaceConditionsMet())
			return;
		// Store the held object.
		player.heldObject.active = true;
		player.heldObject.Interact();

		//player.playerCanMove = false;
	}

	public override void End()
	{
		// Drop the object.
		player.heldObject.Interact();

		player.heldObject.active = false;
		player.heldObject = null;
	}
}
