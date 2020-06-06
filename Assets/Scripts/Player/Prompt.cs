using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
	[HideInInspector] public Text textComponent;
	Player player;
	public bool disabled { get => !textComponent.enabled; }

	void Start()
	{
		player = Player.Instance;
		textComponent = GetComponent<Text>();
	}

	public void UpdateText()
	{
		if (!(player.State is Aiming))
		{
			var hit = InteractableObject.Raycast();
			// if (player.heldObject is Placeable && (player.heldObject as Placeable).PlaceConditionsMet())
			// 	SetText(player.heldObject.prompt);
			// else 
			if (hit && !player.heldObject && player.canMove)
			{
				SetText(hit.prompt);

				// if (hit.TryComponent(out Placeable obj) && obj.PlaceConditionsMet())
				// {
				// 	Disable();
				// 	return;
				// }
			}
			else if (!BridgeBehaviour.forcePrompt)
				Disable();
		}
	}

	public void SetText(string t)
	{
		textComponent.enabled = true;
		textComponent.text = t;
	}
	public void Disable() => textComponent.enabled = false;

	public static string ParseKey(string input)
	{
		switch (input)
		{
			case "LeftControl":
				return "L Ctrl";
			case "RightControl":
				return "R Ctrl";
			case "LeftAlt":
				return "L Alt";
			case "RightAlt":
				return "R Alt";
			default:
				return input;
		}
	}
}
