using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Instantiates tree array with two tree prefabs and gets inactive trees in tree pool.
/// </summary>
/// <returns> Tree if available, null if all trees are unavailable </returns>
public class TreePool : MonoBehaviour
{
	static int numTrees = 500;
	public GameObject firstTreePrefab;
	public GameObject secondTreePrefab;
	static GameObject[] trees;

	// Start is called before the first frame update
	void Start()
	{
		trees = new GameObject[numTrees];
		for (int i = 0; i < numTrees; i++)
		{
			if (Random.Range(0.0f, 1.0f) < 0.5)
				trees[i] = (GameObject) Instantiate(firstTreePrefab, Vector3.zero, Quaternion.identity);
			else
				trees[i] = (GameObject) Instantiate(secondTreePrefab, Vector3.zero, Quaternion.identity);

			trees[i].transform.Rotate(0f, Random.Range(0f, 180f), 0f);
			trees[i].SetActive(false);
		}
	}

	static public GameObject GetTree()
	{
		for (int i = 0; i < numTrees; i++)
		{
			if (!trees[i].activeSelf)
			{
				return trees[i];
			}
		}
		return null;
	}
}
