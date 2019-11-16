using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicWindowManager : InteractableObject
{
    public List<ClipableObject> toClip;
    public GameObject bounds;

    private bool held;

    // Start is called before the first frame update
    void Start()
    {
        held = false;
        Apply();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Apply()
    {
        foreach(ClipableObject target in toClip)
        {
            if(target.gameObject.activeSelf)
            {
                GetComponent<ClipToBounds>().Clip(target.gameObject, bounds, target.result);
            }
        }

    }

    public override void Interact(PlayerManager player)
    {
        if(held)
        {
            transform.parent = null;
            Apply();
            held = false;
        }
        else
        {
            transform.parent = player.transform;
            held = true;
        }
    }
}
