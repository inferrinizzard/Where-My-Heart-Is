using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
	[HideInInspector] public Text textComponent;
	Player player;

	void Start()
	{
		player = Player.Instance;
		textComponent = GetComponent<Text>();
	}

	public void UpdateText()
	{
		if (!(player.State is Aiming))
		{
			var hit = player.RaycastInteractable();
			if (player.heldObject is Placeable && (player.heldObject as Placeable).PlaceConditionsMet())
				SetText("Press "+ ParseKey(InputManager.interactKey.ToString()) + " to Place Canvas");
			else if (hit && !player.heldObject && player.canMove)
			{
				if (hit.TryComponent<Bird>())
					SetText("Press " + ParseKey(InputManager.interactKey.ToString()) + " to Interact with Bird");
				else if (hit.TryComponent<Pushable>())
					if (!hit.GetComponent<Pushable>().isPushing)
						SetText("Press " + ParseKey(InputManager.interactKey.ToString()) + " to Start Pushing");
					else
						SetText("Press " + ParseKey(InputManager.interactKey.ToString()) + " to Stop Pushing");
				else
					SetText(hit.prompt);

				if (hit.TryComponent(out Placeable obj) && obj.PlaceConditionsMet())
				{
					Disable();
					return;
				}
			}
			else
				Disable();
		}
	}

	public void SetText(string t)
	{
		textComponent.enabled = true;
		textComponent.text = t;
	}
	public void Disable() => textComponent.enabled = false;

	public string ParseKey(string input)
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
