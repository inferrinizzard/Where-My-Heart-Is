using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public ClippableObject hitboxObject;
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public string prompt = "Press E to Interact";
	public bool hasFlavorText;
	public string flavorText;
	public DialogueSystem dialogue;

	public virtual void Interact()
	{
		if (hasFlavorText)
		{
			StartCoroutine(dialogue.WriteDialogue(flavorText));
		}
	}

	protected virtual void Start()
	{
		dialogue = GameManager.Instance.dialogue;
		player = Player.Instance;
		if (hitboxObject) hitboxObject.GetComponent<ClippableObject>().tiedInteractable = this;
	}
}
