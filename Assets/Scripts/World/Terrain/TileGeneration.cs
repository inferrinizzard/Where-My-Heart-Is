using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate tiles around the player
/// </summary>
public class TileGeneration : MonoBehaviour
{
    public GameObject player;
    public GameObject door;

    static int timesGenerated = 0;

    //Vector3 startPos = Vector3.zero;
    List<GameObject> myTrees = new List<GameObject>();
    List<GameObject> myDoors = new List<GameObject>();

    [SerializeField] NoiseMapGeneration noiseMapGeneration; // Get script
    [SerializeField] private MeshRenderer tileRenderer; // Show height map of each vertex
    [SerializeField] private MeshFilter meshFilter; // Access mesh vertices
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float mapScale;  // Scale height map

    // Terrain properties
    [SerializeField] private TerrainType[] terrainTypes;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private Waves[] waves;  // Perlin noise detail

    // Start is called before the first frame update
    void Start()
    {
        GenerateTile();
    }

    void GenerateTile()
    {
        // Calculate tile depth and width
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // Calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // Generate heightmap 
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, waves);

        Texture2D tileTexture = BuildTexture(heightMap);
        this.tileRenderer.material.mainTexture = tileTexture;

        // Get trees and door from pool and place based on mesh height
        for (int v = 0; v < meshVertices.Length; v++)
        {
            // if(meshVertices[v].y > 0.8 && Mathf.PerlinNoise((meshVertices[v].x+5)/10, (meshVertices[v].z+5)/10)*10 > 5.2)
            if (meshVertices[v].y > 1.0)
            {
                //   Debug.Log("perlin noise: " + Mathf.PerlinNoise((meshVertices[v].x + 5) / 10, (meshVertices[v].z + 5) / 10) * 10);
                GameObject newTree = TreePool.GetTree();
                if (newTree != null && Random.Range(0.0f, 1.0f) < 0.05)
                {
                    Vector3 treePos = new Vector3(meshVertices[v].x + this.gameObject.transform.position.x,
                                                    meshVertices[v].y + 2.3f,
                                                    meshVertices[v].z + this.gameObject.transform.position.z);
                    newTree.transform.position = treePos;
                    newTree.SetActive(true);
                    myTrees.Add(newTree);
                }

                // Spawn one door when it exists in the pool and respawns when player starts moving 
                GameObject newDoor = DoorPool.GetDoor();
                if (newDoor != null && timesGenerated > 200 && timesGenerated % 3 ==0 )
                {
                    Vector3 doorPos = (player.transform.forward*20) + new Vector3(player.transform.position.x,
                                                    meshVertices[v].y - 0.5f,
                                                    player.transform.position.z);
                    newDoor.transform.position = doorPos;
                    newDoor.transform.rotation = Quaternion.LookRotation(player.transform.forward);
                    newDoor.SetActive(true);
                    myDoors.Add(newDoor);
                   // Debug.Log("player door dist: " + Vector3.Distance(myDoors[0].transform.position, player.transform.position));
                }
                
            }
        }

        timesGenerated++;
       // Debug.Log("total tiles generated " + timesGenerated);

        // Update the tile mesh vertices according to the height map
        UpdateMeshVertices(heightMap);
    }


    // Clear trees and doors when tiles are destoryed
    void OnDestroy()
    {
        for (int i = 0; i < myTrees.Count; i++)
        {
            if (myTrees[i] != null)
                myTrees[i].SetActive(false);
        }

        for (int i = 0; i < myDoors.Count; i++)
        {
            //  Debug.Log("door clear condition: " + Mathf.Abs(Vector3.Distance(myDoors[i].transform.position, player.transform.position)));
            // if (myDoors[i] != null && Mathf.Abs(Vector3.Distance(myDoors[i].transform.position, player.transform.position)) > 25.0f)
            if (myDoors[i] != null)
            {
                myDoors[i].SetActive(false);
            }
        }

        myTrees.Clear();
        myDoors.Clear();

    }
    // Changes plane mesh vertices according to the height map
    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
                meshVertices[vertexIndex] = new Vector3(vertex.x, this.heightCurve.Evaluate(height) * this.heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        // update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

    // Return a Color array to create the Texture2D
    private Texture2D BuildTexture(float[,] heightMap)
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