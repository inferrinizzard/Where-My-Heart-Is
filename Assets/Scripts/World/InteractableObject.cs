using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    /// <summary> Reference to the player. </summary>
    public PlayerMovement player;
    /// <summary> Whether or not this is the active item </summary>

    public bool active;
    public abstract void Interact();
}
