using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Generates infinite terrain. 
/// Keeps track of player position to determine whether a new plane 
/// should be spawned based on range and time.
/// </summary>

public class GenerateInfinite : MonoBehaviour
{
	public GameObject plane;
	public GameObject player;

	int planeSize = 10;
	int halfTilesX = 6;
	int halfTilesZ = 6;

	Vector3 startPos;
	Hashtable tiles = new Hashtable();
	// Start is called before the first frame update
	void Start()
	{
		this.gameObject.transform.position = Vector3.zero;
		startPos = Vector3.zero;

		float updateTime = Time.realtimeSinceStartup;

		// Create 2D array of planes with postion and current time
		for (int x = -halfTilesX; x < halfTilesX; x++)
		{
			for (int z = -halfTilesZ; z < halfTilesZ; z++)
			{
				Vector3 pos = new Vector3((x * planeSize + startPos.x), 0, (z * planeSize + startPos.z));
				GameObject t = (GameObject) Instantiate(plane, pos, Quaternion.identity);

				string tilename = "Tile_" + ((pos.x).ToString() + "_" + ((pos.z).ToString()));
				t.name = tilename;
				Tile tile = new Tile(t, updateTime);
				tiles.Add(tilename, tile);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		// Determine how far the player has moved since the last terrain update
		int xMove = (int) (player.transform.position.x - startPos.x);
		int zMove = (int) (player.transform.position.z - startPos.z);

		// Tile information gets updated if player position is changed
		if (Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
		{
			float updateTime = Time.realtimeSinceStartup;

			// Force integer position and round down to nearest tilesize for determining tile spawn locations
			int playerX = (int) (Mathf.Floor(player.transform.position.x / planeSize) * planeSize);
			int playerZ = (int) (Mathf.Floor(player.transform.position.z / planeSize) * planeSize);

			for (int x = -halfTilesX; x < halfTilesX; x++)
			{
				for (int z = -halfTilesZ; z < halfTilesZ; z++)
				{
					// Create position based on player's current position
					Vector3 pos = new Vector3((x * planeSize + playerX), 0, (z * planeSize + playerZ));
					string tilename = "Tile_" + ((pos.x).ToString() + "_" + ((pos.z).ToString()));

					if (!tiles.ContainsKey(tilename))
					{
						GameObject t = (GameObject) Instantiate(plane, pos, Quaternion.identity);

						t.name = tilename;
						Tile tile = new Tile(t, updateTime);
						tiles.Add(tilename, tile);
					}
					else
					{
						(tiles[tilename] as Tile).creationTime = updateTime;
					}
				}
			}

			// Destroy old, unupdated tiles outside of player range while keeping the current/new ones
			Hashtable newTerrain = new Hashtable();
			foreach (Tile t in tiles.Values)
			{
				//if (t.creationTime != updateTime)
				if (t.creationTime != updateTime)
				{
					Destroy(t.tile);
				}
				else
				{
					newTerrain.Add(t.tile.name, t);
				}
			}

			// Copy new contents to working one
			tiles = newTerrain;
			startPos = player.transform.position;
		}
	}
}

class Tile
{
	public GameObject tile;
	public float creationTime;

	public Tile(GameObject t, float ct)
	{
		tile = t;
		creationTime = ct;
	}
}
