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
	[SerializeField] GameObject plane = default;
	GameObject player;

	public static int planeSize = 10;
	int gridSize = 9;

	Vector3 startPos;
	TileHash tiles = new TileHash();
	List<Vector2Int> deleteQ = new List<Vector2Int>();
	public DoorController door;

	void Start()
	{
		player = Player.Instance.gameObject;
		startPos = Vector3.zero;
		// ForSpiral(gridSize, gridSize, Vector2Int.zero, Generate);
		// StartCoroutine(GenerateTiles());
		GenerateTiles(Vector2Int.zero);
	}

	void Generate(int x, int z)
	{
		Vector3 pos = new Vector3(x, 0, z) * planeSize;
		Vector2Int gridPos = new Vector2Int(x, z);

		if (!tiles[gridPos])
		{
			var tile = GameObject.Instantiate(plane, pos, Quaternion.identity, transform).GetComponent<TileGeneration>();
			tile.name = $"Tile [{x}, {z}]";
			tiles[gridPos] = tile;
			tile.gridPos = gridPos;
		}
		deleteQ.Remove(gridPos);
	}

	public void GenerateTiles(Vector2Int offset)
	{
		deleteQ = tiles.Indices();
		ForSpiral(gridSize, gridSize, offset, Generate);
		foreach (var pos in deleteQ)
			tiles.Remove(pos);
	}

	public void DoorPos(float sqrMag)
	{
		if (sqrMag > (Snowstorm.walkDistance * Snowstorm.walkDistance * .8f) && !door.spawned)
			door.Spawn();
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
		public List<Vector2Int> Indices()
		{
			var list = new List<Vector2Int>();
			foreach (var kvp in tiles)
				list.Add(kvp.Key);
			return list;
		}
		public void Remove(Vector2Int index)
		{
			var destroy = tiles[index];
			tiles.Remove(index);
			Destroy(destroy.gameObject);
		}
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
	}
}
