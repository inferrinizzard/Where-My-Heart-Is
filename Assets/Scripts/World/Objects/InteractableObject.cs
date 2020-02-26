using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public ClipableObject hitboxObject;
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public abstract void Interact();

	protected virtual void Start()
	{
        player = Player.Instance;
        if (hitboxObject) hitboxObject.GetComponent<ClipableObject>().tiedInteractable = this;
	}
}
