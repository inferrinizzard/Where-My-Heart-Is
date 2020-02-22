using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public ClipableObject hitboxObject;
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public abstract void Interact();

	private void Awake()
	{
		player = Player.Instance;
	}

	protected virtual void Start()
	{
		if (hitboxObject) hitboxObject.GetComponent<ClipableObject>().tiedInteractable = this;
	}
}
