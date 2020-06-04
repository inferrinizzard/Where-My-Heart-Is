using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	public ClippableObject hitboxObject;
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	/// <summary> Whether or not this is the active item </summary>
	[HideInInspector] public bool active;
	public virtual string prompt { get => "Press " + InputManager.interactKey + " to Interact"; set { } }
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

	public static string ParseKey(string input)
	{
		if (input == "LeftControl")
		{
			return "L Ctrl";
		}
		if (input == "RightControl")
		{
			return "R Ctrl";
		}
		if (input == "LeftAlt")
		{
			return "L Alt";
		}
		if (input == "RightAlt")
		{
			return "R Alt";
		}
		return input;
	}
}
