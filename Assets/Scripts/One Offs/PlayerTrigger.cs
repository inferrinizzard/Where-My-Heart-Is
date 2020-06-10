using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
	public event Action OnPlayerEnter;
	public event Action<PlayerTrigger> OnPlayerEnterID;
	public bool destroyAfterTrigger;
	DialogueSystem dialogueSystem;

	public string flavor;

	void Start() => dialogueSystem = GameManager.Instance.dialogue;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			// if (flavor != "")
			// {
			// 	StartCoroutine(dialogueSystem.WriteDialogue(flavor));
			// }
			OnPlayerEnter?.Invoke();
			OnPlayerEnterID?.Invoke(this);
			if (destroyAfterTrigger)
			{
				Destroy(this);
			}
		}
	}
}
