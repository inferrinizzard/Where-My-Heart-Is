using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWindowManager : InteractableObject
{
    public bool useTwin;
    public MagicWindowManager twin;

    public List<ClipableObject> toClip;
    public GameObject bounds;
    public float offset;
    public Vector3 rotationOffset;

    private bool held;

    // Start is called before the first frame update
    void Start()
    {
        held = false;
        ApplyAll();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ApplyAll()
    {
        if(useTwin)
        {
            twin.ApplyAll();
        }
        foreach(ClipableObject target in toClip)
        {
            if(target.gameObject.activeSelf)
            {
                GetComponent<CSG.Operations>().Union(target.gameObject, bounds, target.result);
            }
        }
    }

    public override void Interact(PlayerManager player)
    {
        if(held)
        {
            transform.parent = null;
            ApplyAll();
            held = false;
        }
        else
        {
            transform.parent = player.transform;
            transform.position = player.transform.position + player.transform.forward * offset;
            transform.rotation = Quaternion.Euler(player.transform.rotation.eulerAngles + rotationOffset);
            held = true;
        }
    }
}
