using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

/// <summary>
/// Generate tiles with trees and occasional door around the player
/// </summary>
public class TileGeneration : MonoBehaviour
{
	public GameObject player;
	public GameObject door;

	List<GameObject> myTrees = new List<GameObject>();
	List<GameObject> myDoors = new List<GameObject>();

	[SerializeField] private int spawnDist = 50; // Spawn door after 50 steps
	[SerializeField] private int stepsTaken = 149;

	[SerializeField] NoiseMapGeneration noiseMapGeneration = default; // Get script
	[SerializeField] private MeshRenderer tileRenderer = default; // Show height map of each vertex
	[SerializeField] private MeshFilter meshFilter = default; // Access mesh vertices
	[SerializeField] private MeshCollider meshCollider = default;
	[SerializeField] private float mapScale = default; // Scale height map

	// Terrain properties
	[SerializeField] private TerrainType[] terrainTypes = default;
	[SerializeField] private float heightMultiplier = default;
	[SerializeField] private AnimationCurve heightCurve = default;
	[SerializeField] private Waves[] waves = default; // Perlin noise detail

	GenerateInfinite gen;
	[HideInInspector] public Vector2Int gridPos;

	void Start()
	{
		gen = transform.GetComponentInParent<GenerateInfinite>();
		GenerateTile();
	}

	Vector2 Gaussian(float? x = null, float? y = null)
	{
		float u1 = x ?? 1f - Random.value, u2 = y ?? 1f - Random.value;
		float preLog = Mathf.Sqrt(-2f * Mathf.Log(u1));
		return new Vector2(preLog * Mathf.Sin(2f * Mathf.PI * u2), preLog * Mathf.Cos(2f * Mathf.PI * u2));
	}

	void GenerateTile()
	{
		// Calculate tile depth and width
		Vector3[] meshVertices = meshFilter.mesh.vertices.OrderBy(v => v.sqrMagnitude).ToArray();
		int tileDepth = (int) Mathf.Sqrt(meshVertices.Length);
		int tileWidth = tileDepth;

		// Calculate the offsets based on the tile position
		Vector2 offset = -new Vector2(transform.position.x, transform.position.z);

		// Generate heightmap 
		float[, ] heightMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, mapScale, offset.x, offset.y, waves);

		Texture2D tileTexture = BuildTexture(heightMap);
		tileRenderer.material.mainTexture = tileTexture;

		// Get trees and door from pool and place based on mesh height
		for (int v = 0; v < meshVertices.Length; v++)
		{
			// if(meshVertices[v].y > 0.8 && Mathf.PerlinNoise((meshVertices[v].x+5)/10, (meshVertices[v].z+5)/10)*10 > 5.2)
			if (meshVertices[v].y > 1.0)
			{
				//   Debug.Log("perlin noise: " + Mathf.PerlinNoise((meshVertices[v].x + 5) / 10, (meshVertices[v].z + 5) / 10) * 10);
				GameObject newTree = TreePool.GetTree();
				float treeScale = Random.Range(0.0f, 0.1f);
				if (newTree != null && Random.Range(0.0f, 1.0f) < .5f)
				// if (newTree != null && Random.Range(0.0f, 1.0f) < (1.2f - Mathf.Min(1, meshVertices[v].magnitude / SnowstormBehaviour.walkDistance)) * .8f)
				{
					Vector3 treePos = new Vector3(meshVertices[v].x + transform.position.x,
						meshVertices[v].y + 2.5f,
						meshVertices[v].z + transform.position.z);
					newTree.transform.position = treePos;
					newTree.transform.localScale += new Vector3(treeScale, treeScale, treeScale);
					newTree.SetActive(true);
					myTrees.Add(newTree);
				}
			}
		}

		// Update the tile mesh vertices according to the height map
		UpdateMeshVertices(heightMap);
	}

	private void Update()
	{
		// if ((player.transform.position - transform.position).sqrMagnitude > 100 * 100)
		// 	gen.Remove(gridPos);

		// Spawn one door when it exists in the pool and respawns when player moves a certain distance
		GameObject newDoor = DoorPool.GetDoor();
		if (newDoor != null && (int) Vector3.Distance(Vector3.zero, player.transform.position) > stepsTaken && (int) Vector3.Distance(Vector3.zero, player.transform.position) % spawnDist == 0)
		{
			Vector3 doorPos = (player.transform.forward * 20) + new Vector3(player.transform.position.x,
				0.6f,
				player.transform.position.z);
			newDoor.transform.position = doorPos;
			newDoor.transform.rotation = Quaternion.LookRotation(player.transform.forward);
			newDoor.SetActive(true);
			myDoors.Add(newDoor);
			// Debug.Log("player door dist: " + Vector3.Distance(myDoors[0].transform.position, player.transform.position));
		}

		// Clear door when player position is far enough
		if ((int) Vector3.Distance(Vector3.zero, player.transform.position) % (spawnDist * 2) == 0)
		{
			for (int i = 0; i < myDoors.Count; i++)
			{
				if (myDoors[i] != null)
				{
					myDoors[i].SetActive(false);
				}
			}
			myDoors.Clear();
		}

		// Debug.Log((int) Vector3.Distance(Vector3.zero, player.transform.position));
	}
	// Clear trees and doors when tiles are destoryed
	void OnDestroy()
	{
		for (int i = 0; i < myTrees.Count; i++)
		{
			if (myTrees[i])
				myTrees[i].SetActive(false);
		}
		myTrees.Clear();
	}

	// Changes plane mesh vertices according to the height map
	private void UpdateMeshVertices(float[, ] heightMap)
	{
		int tileDepth = heightMap.GetLength(0);
		int tileWidth = heightMap.GetLength(1);

		Vector3[] meshVertices = meshFilter.mesh.vertices;

		// iterate through all the heightMap coordinates, updating the vertex index
		int vertexIndex = 0;
		for (int zIndex = 0; zIndex < tileDepth; zIndex++)
			for (int xIndex = 0; xIndex < tileWidth; xIndex++)
			{
				float height = heightMap[zIndex, xIndex];

				Vector3 vertex = meshVertices[vertexIndex];
				// change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
				meshVertices[vertexIndex] = new Vector3(vertex.x, heightCurve.Evaluate(height) * heightMultiplier, vertex.z);

				vertexIndex++;
			}

		// update the vertices in the mesh and update its properties
		meshFilter.mesh.vertices = meshVertices;
		meshFilter.mesh.RecalculateBounds();
		meshFilter.mesh.RecalculateNormals();
		// update the mesh collider
		meshCollider.sharedMesh = meshFilter.mesh;
	}

	// Return a Color array to create the Texture2D
	private Texture2D BuildTexture(float[, ] heightMap)
	{
		int tileDepth = heightMap.GetLength(0);
		int tileWidth = heightMap.GetLength(1);

		Color[] colorMap = new Color[tileDepth * tileWidth];
		for (int zIndex = 0; zIndex < tileDepth; zIndex++)
		{
			for (int xIndex = 0; xIndex < tileWidth; xIndex++)
			{
				// Transform the 2D map index into an Array index
				int colorIndex = zIndex * tileWidth + xIndex;
				float height = heightMap[zIndex, xIndex];

				// Terrain type according to height
				TerrainType terraintype = ChooseTerrainType(height);

				// Assign as color a shade of grey proportional to the height value
				//colorMap[colorIndex] = Color.Lerp(Color.black, Color.white, height);
				colorMap[colorIndex] = terraintype.color;

			}
		}

		// Create a new texture and set its pixel colors
		Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
		tileTexture.wrapMode = TextureWrapMode.Clamp;
		tileTexture.SetPixels(colorMap);
		tileTexture.Apply();

		return tileTexture;
	}

	TerrainType ChooseTerrainType(float height)
	{
		// For each terrain type, check if the height is lower than the one for the terrain type
		foreach (TerrainType terrainType in terrainTypes)
		{
			// Return the first terrain type whose height is higher than the generated one
			if (height < terrainType.height)
			{
				return terrainType;
			}
		}
		return terrainTypes[terrainTypes.Length - 1];
	}
}

[System.Serializable]
public class TerrainType
{
	public string name;
	public float height;
	public Color color;
}
