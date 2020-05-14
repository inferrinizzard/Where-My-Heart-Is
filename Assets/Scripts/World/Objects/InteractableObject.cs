using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public ClippableObject hitboxObject;
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public virtual string prompt { get => "Press E to Interact"; set { } }
	string flavorText = "";
	DialogueSystem dialogue;
	[HideInInspector] public Renderer[] renderers;

	public virtual void Interact()
	{
		if (flavorText != "")
			StartCoroutine(dialogue.WriteDialogue(flavorText));
	}

	protected virtual void Start()
	{
		renderers = GetComponentsInChildren<Renderer>();
		dialogue = GameManager.Instance.dialogue;
		player = Player.Instance;
		if (hitboxObject) hitboxObject.GetComponent<ClippableObject>().tiedInteractable = this;
	}

	void OnMouseEnter()
	{
		if (!player.heldObject && !this.TryComponent<OutlineObject>() && (transform.position - player.transform.position).sqrMagnitude < player.playerReach * player.playerReach)
			Effects.Instance.SetGlow(this);
	}

	void OnMouseExit() => Effects.Instance.SetGlow(null);
}
