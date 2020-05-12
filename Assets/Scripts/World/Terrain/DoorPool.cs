using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Instantiates a door 
/// </summary>
/// <returns> A door </returns>
public class DoorPool : MonoBehaviour
{
	static int numDoors = 1;
	public GameObject doorPrefab;
	static GameObject[] doors;

	// Start is called before the first frame update
	void Start()
	{
		doors = new GameObject[numDoors];
		for (int i = 0; i < numDoors; i++)
		{
			doors[i] = (GameObject) Instantiate(doorPrefab, Vector3.zero, Quaternion.identity);
			doors[i].SetActive(false);
		}
	}
	static public GameObject GetDoor()
	{
		for (int i = 0; i < numDoors; i++)
		{
			if (!doors[i].activeSelf)
			{
				return doors[i];
			}
		}
		return null;
	}
}
