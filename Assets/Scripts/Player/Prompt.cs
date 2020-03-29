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
				SetText("Press E to Place Canvas");
			else if (hit && !player.heldObject && player.canMove)
			{
				if (hit.GetComponent<BirbAnimTester>())
					SetText("Press E to Interact with Bird");
				else
					SetText(hit.prompt);

				if (hit.GetComponent<Placeable>() && hit.GetComponent<Placeable>().PlaceConditionsMet())
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
}
