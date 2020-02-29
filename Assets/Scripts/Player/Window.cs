using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/**
 * <summary>
 * Class that uses CSG.Operations on ClipableObjects to create the cut
 * </summary>
 * */
public class Window : MonoBehaviour
{
    public World world;
    public GameObject fieldOfView;
    MeshCollider fovMeshCollider; //assign
    public CSG.Model fieldOfViewModel;

    private CSG.Operations csgOperator;

    void Start()
    {
        csgOperator = GetComponent<CSG.Operations>();
        fieldOfViewModel = new CSG.Model(fieldOfView.GetComponent<MeshFilter>().mesh);
    }

    public void ApplyCut()
    {
        world.ResetCut();

        world.GetRealObjects().ToList().ForEach(clipable => { if (IntersectsBounds(clipable)) clipable.UnionWith(fieldOfView, csgOperator); });
        world.GetDreamObjects().ToList().ForEach(clipable => { if (IntersectsBounds(clipable)) clipable.Subtract(fieldOfView, csgOperator); });

        foreach (EntangledClipable entangled in world.GetEntangledObjects())
        {
            // clip the immediate children of entangled
            //if (IntersectsBounds(entangled.realVersion)) entangled.realVersion.UnionWith(fieldOfView, csgOperator);
            //if (IntersectsBounds(entangled.dreamVersion)) entangled.dreamVersion.Subtract(fieldOfView, csgOperator);

            // clip any children below them to the correct world
            entangled.realObject.GetComponentsInChildren<ClipableObject>().ToList().ForEach(
                clipable => { if (IntersectsBounds(clipable)) clipable.UnionWith(fieldOfView, csgOperator); });
            entangled.dreamObject.GetComponentsInChildren<ClipableObject>().ToList().ForEach(
                clipable => { if (IntersectsBounds(clipable)) clipable.Subtract(fieldOfView, csgOperator); });
        }
    }

    private bool IntersectsBounds(ClipableObject clipableObject)
    {
        // less expensive, less accurate intersection check
        //TODO: checking the bound intersections doesn't consider world space, just model space, so it's basically trash
        if (true || fieldOfView.GetComponent<MeshCollider>().bounds.Intersects(clipableObject.GetComponent<MeshFilter>().mesh.bounds))
        {
            // more expensive, more accurate intersection check
            if (clipableObject.IntersectsBound(fieldOfView.transform, fieldOfViewModel))
            {
                return true;
            }
        }

        return false;
    }
}
