using UnityEngine;

public class Phone : InteractableObject
{
	public override string prompt { get => "Press E to Interact with Phone"; }
	public override void Interact()
	{
		Effects.Instance.ToggleFog(false);
		StartCoroutine(Player.Instance.mask.PreTransition());
	}
}
