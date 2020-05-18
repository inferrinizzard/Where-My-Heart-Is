using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary> Instantiates tree array with two tree prefabs and gets inactive trees in tree pool. </summary>
public class TreePool : MonoBehaviour
{
	static int numTrees = 2000;
	[SerializeField] GameObject[] treePrefabs;
	static GameObject[] trees;

	// Start is called before the first frame update
	void Start()
	{
		trees = new GameObject[numTrees];
		for (int i = 0; i < numTrees; i++)
		{
			trees[i] = GameObject.Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], Vector3.zero, Quaternion.identity, transform);
			trees[i].transform.Rotate(0f, Random.Range(0f, 180f), 0f);
			trees[i].SetActive(false);
		}
	}

	static public GameObject GetTree()
	{
		for (int i = 0; i < numTrees; i++)
			if (!trees[i].activeSelf)
				return trees[i];
		return null;
	}
}
