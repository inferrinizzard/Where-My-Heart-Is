using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of an object that can be picked up.
/// </summary>
public class Pickupable : InteractableObject
{
    /// <summary>
    /// Checks if the object is being held.
    /// </summary>
    protected bool holding = false;

    /// <summary>
    /// Checks if the object is being inspected.
    /// </summary>
    protected bool looking = false;
    
    /// <summary>
    /// Where the object should be moving towards.
    /// </summary>
    private Vector3 targetPos;

    /// <summary>
    /// Reference to the player.
    /// </summary>
    private PlayerMovement player;

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        // If the object is being held, run Holding.
        if(holding) Holding();

        // If the object is being inspected, run Looking.
        if(looking) Looking();
    }

    /// <summary>
    /// Manages behavoir of the object when being held.
    /// </summary>
    public void Holding()
    {
        // Move the object to the target position.
        targetPos = player.GetHeldObjectLocation().position;
        transform.position = targetPos;

        // If the player is not inspecting the object, set its rotation relative to the held object location.
        if(!looking)
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

    /// <summary>
    /// Used to tell the object to stop being inspected.
    /// </summary>
    public void StopLooking()
    {
        looking = false;
    }

    /// <summary>
    /// Sets variables when the player has picked up the object.
    /// </summary>
    /// <param name="player">Reference to the player.</param>
    public void PickUp(PlayerMovement player)
    {
        this.player = player;
        looking = true;
        holding = true;
    }

    /// <summary>
    /// Drops the object and removes any parents.
    /// </summary>
    public void Drop()
    {
        holding = false;
        transform.parent = null;
    }

    public override void Interact(PlayerMovement player)
    {
        if(!holding)
        {
            PickUp(player);
        }
        else if(looking)
        {
            StopLooking();
        }
        else
        {
            Drop();
        }
    }
}
