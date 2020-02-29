using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : Singleton<AudioMaster>
{
    [FMODUnity.EventRef]
    public string AmbientEvent;

    private FMOD.Studio.EventInstance ambientInstance;

    // Start is called before the first frame update
    void Start()
    {
        ambientInstance = FMODUnity.RuntimeManager.CreateInstance(AmbientEvent);
        ambientInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
