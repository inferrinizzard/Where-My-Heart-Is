using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class Birdcage : InteractableObject
{
    [FMODUnity.EventRef]
    public string CageEvent;

    public GameObject blackout;
    public GameObject distress;

    public override void Interact()
    {
        GameObject b = Instantiate(blackout);
        b.transform.parent = Player.Instance.transform;
        b.transform.localPosition = Vector3.forward * 0.5f;
        b.transform.localRotation = Quaternion.identity;
        Player.Instance.interactPrompt.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot(CageEvent);
        AudioMaster.Instance.StopAll();
        distress.GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        Player.Instance.playerCanMove = false;
    }
}
