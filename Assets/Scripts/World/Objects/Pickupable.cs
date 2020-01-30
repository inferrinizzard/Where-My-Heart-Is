using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles the behavior of an object that can be picked up. </summary>
public class Pickupable : InteractableObject
{

    private Transform oldParent;

	void Update()
	{
		if (active)
		{
			// If the object is being held, run Holding.
			//if (player.holding)Holding();

			// If the object is being inspected, run Looking.
			if (player.looking)Looking();
		}
	}

	/// <summary> Manages behaviour of the object when being held. </summary>
	public void Holding()
	{
		// Move the object to the target position.
		/*targetPos = player.GetHeldObjectLocation().position;
		transform.position = targetPos;*/

		// If the player is not inspecting the object, set its rotation relative to the held object location.
		/*if (!player.looking)
		{
            // Rotate the object based on the player camera
            oldParent = transform.parent;
            transform.parent = player.GetHeldObjectLocation();
		}*/
	}

	/// <summary> Manages behavior of the object when being inspected. </summary>
	public void Looking()
	{
		// Set the rotations based on the mouse movement.
		float rotX = Input.GetAxis("Mouse X") * 2f;
		float rotY = Input.GetAxis("Mouse Y") * 2f;

		// Rotate the object based on previous rotations.
		transform.rotation = Quaternion.AngleAxis(-rotX, player.GetHeldObjectLocation().up) * transform.rotation;
		transform.rotation = Quaternion.AngleAxis(rotY, player.GetHeldObjectLocation().forward) * transform.rotation;
	}

	public override void Interact()
	{
		if (!player.holding)
		{
			player.holding = true;

            // save the old parent to revert to later
            oldParent = transform.parent;
            Debug.Log(oldParent);
            transform.parent = player.GetHeldObjectLocation();// set the new parent to the hold object location object
            transform.localPosition = Vector3.zero;// set the position to local zero to match the position of the hold object location target
        }
		else if (player.looking)
		{
			player.looking = false;
		}
		else
		{
			player.holding = false;
			transform.parent = oldParent;
		}
	}
}
