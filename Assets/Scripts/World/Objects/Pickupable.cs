using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of an object that can be picked up.
/// </summary>
public class Pickupable : InteractableObject
{
	/// <summary>
	/// Where the object should be moving towards.
	/// </summary>
	private Vector3 targetPos;

	void Update()
	{
		if (active)
		{
			// If the object is being held, run Holding.
			if (player.holding)Holding();

			// If the object is being inspected, run Looking.
			if (player.looking)Looking();
		}
	}

	/// <summary>
	/// Manages behaviour of the object when being held.
	/// </summary>
	public void Holding()
	{
		// Move the object to the target position.
		targetPos = player.GetHeldObjectLocation().position;
		transform.position = targetPos;

		// If the player is not inspecting the object, set its rotation relative to the held object location.
		if (!player.looking)
		{
			// Rotate the object based on the player camera
			transform.parent = player.GetHeldObjectLocation();
		}
	}

	/// <summary>
	/// Manages behavior of the object when being inspected.
	/// </summary>
	public void Looking()
	{
		// Set the rotations based on the mouse movement.
		float rotX = Input.GetAxis("Mouse X") * 2f;
		float rotY = Input.GetAxis("Mouse Y") * 2f;

		// Rotate the object based on previous rotations.
		transform.rotation = Quaternion.AngleAxis(-rotX, player.GetHeldObjectLocation().up) * transform.rotation;
		transform.rotation = Quaternion.AngleAxis(rotY, player.GetHeldObjectLocation().right) * transform.rotation;
	}

	public override void Interact()
	{
		if (!player.holding)
		{
			player.looking = true;
			player.holding = true;
		}
		else if (player.looking)
		{
			player.looking = false;
		}
		else
		{
			player.holding = false;
			transform.parent = null;
		}
	}
}
