using System.Linq;

using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
	/// <summary> Reference to the player. </summary>
	[HideInInspector] public Player player;
	public virtual string prompt { get => $"Press {InputManager.InteractKey} to Interact"; }

	[HideInInspector] public Renderer[] renderers;

	public virtual void Interact() { }

	protected virtual void Start()
	{
		renderers = GetComponentsInChildren<Renderer>();
		player = Player.Instance;
	}

	public static InteractableObject Raycast() => Physics.SphereCastAll(Player.Instance.cam.transform.position, .25f, Player.Instance.cam.transform.forward, Player.Instance.playerReach, 1 << 9).Take(3).FirstOrDefault(i => i.transform.TryComponent<InteractableObject>()).transform?.GetComponent<InteractableObject>();
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
