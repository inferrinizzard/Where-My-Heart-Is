using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	[SerializeField] DoorController door;

	void OnTriggerEnter()
	{
		door.anim.Play("Slam");
	}

	void OnTriggerExit()
	{

	}
}
