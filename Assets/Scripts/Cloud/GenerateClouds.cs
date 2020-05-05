using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rot;
    private bool _isActive;

    public bool isActive
    {
        get
        {
            return _isActive;
        }
    }

    public int x;
    public int y;
    public float distFromCam;

    //Returns Matrix4x4 for the cloud
    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rot, scale);
        }
    }


    public CloudData(Vector3 pos, Vector3 scale, Quaternion rot, int x, int y, float distFromCam)
    {
        this.pos = pos;
        this.scale = scale;
        this.rot = rot;
        SetActive(true);
        this.x = x;
        this.y = y;
        this.distFromCam = distFromCam;
    }

    //Sets our private isActive state since other classes can't set it directly
    public void SetActive(bool desState)
    {
        _isActive = desState;
    }
}

public class GenerateClouds : MonoBehaviour
{
    //Meshes
    public Mesh cloudMesh;
    public Material cloudMat;

    //Cloud Data
    public float cloudSize = 8;
    public float maxScale = 3;

    //Noise Generation
    public float timeScale = 1;
    public float texScale = 1;

    //Cloud Scaling Info
    public float minNoiseSize = 0.5f;
    public float sizeScale = 0.25f;

    //Culling Data
    public Camera cam;
    public int maxDist;

    //Number of batches
    public int batchesToCreate;

    private Vector3 prevCamPos;
    private float offsetX = 1;
    private float offsetY = 1;
    private List<List<CloudData>> batches = new List<List<CloudData>>();
    private List<List<CloudData>> batchesToUpdate = new List<List<CloudData>>();

    private void Start()
    {
        for (int batchesX = 0; batchesX < batchesToCreate; batchesX++)
        {
            for (int batchesY = 0; batchesY < batchesToCreate; batchesY++)
            {
                BuildCloudBatch(batchesX, batchesY);
            }
        }
    }

//to generate 100 clouds
    private void BuildCloudBatch(int xLoop, int yLoop)
    {
        //Mark a batch if it's within rage of our camera
        bool markBatch = false;
        //This is our current cloud batch that we're brewing
        List<CloudData> currBatch = new List<CloudData>();

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                //Add a cloud for each loop
                AddCloud(currBatch, x + xLoop * 10, y + yLoop * 10);
            }
        }

        //Check if the batch should be marked
        markBatch = CheckForActiveBatch(currBatch);

        //Add the newest batch to the batches list
        batches.Add(currBatch);

        //If the batch is marked add it to the batchesToUpdate list
        if (markBatch) batchesToUpdate.Add(currBatch);
    }

    //This method checks to see if the current batch has a cloud that is witin our cameras range
    //Return true if a cloud is within range
    //Return false if no clouds are within range
    private bool CheckForActiveBatch(List<CloudData> batch)
    {
        foreach (var cloud in batch)
        {
            cloud.distFromCam = Vector3.Distance(cloud.pos, cam.transform.position);
            if (cloud.distFromCam < maxDist) return true;
        }
        return false;
    }

    //This method created our clouds as a CloudData object
    private void AddCloud(List<CloudData> currBatch, int x, int y)
    {
        //First we set our new clouds position
        Vector3 position = new Vector3(transform.position.x + x * cloudSize, transform.position.y, transform.position.z + y * cloudSize);

        //We set our new clouds distance to the camera so we can use it later
        float disToCam = Vector3.Distance(new Vector3(x, transform.position.y, y), cam.transform.position);

        //Finally we add our new CloudData cloud to the current batch
        currBatch.Add(new CloudData(position, Vector3.zero, Quaternion.identity, x, y, disToCam));
    }

    //We need to generate our noise
    //We update our offsets to the noise 'rolls' through the cloud objects
    private void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
    }

    //This method updates our noise/clouds
    //First we check to see if the camera has moved
    //If it hasn't we update batches
    //If it has moved we need to reset the prevCamPos along with updating our batch list before updating our batches
    //TODO: Set allowed movement range to camera so the player can move a small amount without causing a full batch list reset
    private void MakeNoise()
    {
        if (cam.transform.position == prevCamPos)
        {
            UpdateBatches();
        }
        else
        {
            prevCamPos = cam.transform.position;
            UpdateBatchList();
            UpdateBatches();
        }
        RenderBatches();
        prevCamPos = cam.transform.position;
    }

    //This updates for the clouds
    private void UpdateBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            foreach (var cloud in batch)
            {
                //Get noise size based on clouds pos, noise texture scale, and our offset amount
                float size = Mathf.PerlinNoise(cloud.x * texScale + offsetX, cloud.y * texScale + offsetY);

                //If our cloud has a size that's above our visible cloud threashold we need to show it
                if (size > minNoiseSize)
                {
                    //Get the current scale of the cloud
                    float localScaleX = cloud.scale.x;

                    //Activate any clouds
                    if (!cloud.isActive)
                    {
                        cloud.SetActive(true);
                        cloud.scale = Vector3.zero;
                    }
                    //If not max size, scale up
                    if (localScaleX < maxScale)
                    {
                        ScaleCloud(cloud, 1);

                        // max size
                        if (cloud.scale.x > maxScale)
                        {
                            cloud.scale = new Vector3(maxScale, maxScale, maxScale);
                        }
                    }
                }

                else if (size < minNoiseSize)
                {
                    float localScaleX = cloud.scale.x;
                    ScaleCloud(cloud, -1);

                    //When the cloud small just hide them
                    if (localScaleX <= 0.1)
                    {
                        cloud.SetActive(false);
                        cloud.scale = Vector3.zero;
                    }
                }
            }
        }
    }

    //This method sets our cloud to a new size
    private void ScaleCloud(CloudData cloud, int direciton)
    {
        cloud.scale += new Vector3(sizeScale * Time.deltaTime * direciton, sizeScale * Time.deltaTime * direciton, sizeScale * Time.deltaTime * direciton);
    }

    //This method clears our batchesToUpdate list because we only want visible batches within this list
    private void UpdateBatchList()
    {
        //Clears our list
        batchesToUpdate.Clear();

        //Loop through all the generated batches
        foreach (var batch in batches)
        {
            //If a single cloud is within range we need to add the batch to the update list
            if (CheckForActiveBatch(batch))
            {
                batchesToUpdate.Add(batch);
            }
        }
    }

    //This method loops through all the batches to update and draws their meshes to the screen
    private void RenderBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            Graphics.DrawMeshInstanced(cloudMesh, 0, cloudMat, batch.Select((a) => a.matrix).ToList());
        }
    }
}