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
	[SerializeField] GameObject plane;
	GameObject player;

	int planeSize = 10;
	int gridSize = 9;

	Vector3 startPos;
	TileHash tiles = new TileHash();

	void Start()
	{
		player = Player.Instance.gameObject;
		startPos = Vector3.zero;
		// ForSpiral(gridSize, gridSize, Vector2Int.zero, Generate);
		StartCoroutine(GenerateTiles());
	}

	void Generate(int x, int z)
	{
		Vector3 pos = new Vector3(x, 0, z) * planeSize;

		if (!tiles[x, z])
		{
			var tile = GameObject.Instantiate(plane, pos, Quaternion.identity, transform).GetComponent<TileGeneration>();
			tile.name = $"Tile [{x}, {z}]";
			tiles[x, z] = tile;
			tile.gridPos = new Vector2Int(x, z);
		}
	}

	IEnumerator GenerateTiles(float delay = 2)
	{
		ForSpiral(gridSize, gridSize, ToGridSpace(player.transform.position), Generate);
		yield return new WaitForSeconds(delay);
		StartCoroutine(GenerateTiles(delay));
	}

	public void Remove(Vector2Int index)
	{
		var tile = tiles[index];
		tiles.Remove(index);
		Destroy(tile.gameObject);
	}

	Vector2Int ToGridSpace(Vector3 pos) => Vector2Int.FloorToInt(new Vector2(pos.x, pos.z));

	void ForSpiral(int X, int Y, Vector2Int offset, System.Action<int, int> func)
	{
		int x, y, dx, dy;
		x = y = dx = 0;
		dy = -1;
		int t = System.Math.Max(X, Y);
		int maxI = t * t;
		for (int i = 0; i < maxI; i++)
		{
			if ((-X / 2 <= x) && (x <= X / 2) && (-Y / 2 <= y) && (y <= Y / 2))
				func.Invoke(x + offset.x, y + offset.y);
			if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
			{
				t = dx;
				dx = -dy;
				dy = t;
			}
			x += dx;
			y += dy;
		}
	}

	class TileHash
	{
		Dictionary<Vector2Int, TileGeneration> tiles;
		public TileHash() => tiles = new Dictionary<Vector2Int, TileGeneration>();
		public void Remove(Vector2Int index) => tiles.Remove(index);
		public TileGeneration this [Vector2Int index]
		{
			get => tiles.ContainsKey(index) ? tiles[index] : null;
			set
			{
				if (tiles.ContainsKey(index))
					tiles[index] = value;
				else tiles.Add(index, value);
			}
		}
		public TileGeneration this [int x, int z]
		{
			get
			{
				Vector2Int index = new Vector2Int(x, z);
				return tiles.ContainsKey(index) ? tiles[index] : null;
			}
			set
			{
				Vector2Int index = new Vector2Int(x, z);
				if (tiles.ContainsKey(index))
					tiles[index] = value;
				else tiles.Add(index, value);
			}
		}
	}
}
