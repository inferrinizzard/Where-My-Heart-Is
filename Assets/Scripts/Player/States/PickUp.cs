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
		// Raycast for what the player is looking at.
		RaycastHit hit;

		// Make sure it is in the right layer
		int layerMask = 1 << 9;

		// Raycast to see what the object's tag is. If it is a Pickupable object...
		if (Physics.Raycast(player.cam.transform.position, player.cam.transform.forward, out hit, player.playerReach, layerMask) && hit.transform.GetComponent<InteractableObject>())
		{
            player.pickedUpFirst = true;
            if (hit.transform.GetComponent<Placeable>() && hit.transform.GetComponent<Placeable>().PlaceConditionsMet())
            {
                return;
            }
			// Store the held object.
			player.heldObject = hit.collider.gameObject.GetComponent<InteractableObject>();
			player.heldObject.Interact();
			player.heldObject.active = true;

			//player.playerCanMove = false;
		}
	}
}
