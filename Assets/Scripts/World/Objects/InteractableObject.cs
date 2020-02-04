using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public abstract void Interact();
}
