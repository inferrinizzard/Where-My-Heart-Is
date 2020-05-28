using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	bool inside = false;
	DoorController door;
	void Start()
	{
		door = GetComponentInParent<DoorController>();
	}

	void Update()
	{
		if (inside && Physics.Raycast(Player.Instance.transform.position, Player.Instance.cam.transform.forward, out RaycastHit hit, 10, layerMask : 1 << LayerMask.NameToLayer("Immutable")))
		{
			Debug.Log(hit.transform.gameObject);
			if (hit.transform.GetComponentInParent<DoorController>() == door)
			{
				GameManager.Instance.prompt.SetText("Press E to Open Door");
				if (Input.GetKeyDown(KeyCode.E))
				{
					door.Open();
					Destroy(gameObject);
				}
			}
		}
	}

	void OnTriggerEnter() { inside = true; }
	void OnTriggerExit() { inside = false; }
}
