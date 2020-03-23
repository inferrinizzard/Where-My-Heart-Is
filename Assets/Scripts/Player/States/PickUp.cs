using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Pick up item state. </summary>
public class PickUp : PlayerState
{
	Pickupable target;

	/// <summary> Constructor. </summary>
	/// <param name="_player"> Reference to player. </param>
	public PickUp(Player _player, Pickupable obj) : base(_player) { target = obj; }

	public override void Start()
	{
		player.pickedUpFirst = true;
		if (target.GetComponent<Placeable>() && target.GetComponent<Placeable>().PlaceConditionsMet())
			return;
		target.Interact();
		player.heldObject = target;
		// Store the held object.
		player.heldObject.active = true;

		//player.canMove = false;
	}

	public override void End()
	{
		// Drop the object.
		player.heldObject.Interact();

		player.heldObject.active = false;
		player.heldObject = null;
	}
}
