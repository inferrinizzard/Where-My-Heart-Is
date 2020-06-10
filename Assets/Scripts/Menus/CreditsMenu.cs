using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class CreditsMenu : MonoBehaviour
{
    public GameObject regularCredits;
    public GameObject niceCredits;

    // Start is called before the first frame update
    void Start()
    {
        regularCredits.SetActive(true);
        niceCredits.SetActive(false);
    }

    public void SwitchCredits()
    {
        regularCredits.SetActive(!regularCredits.activeSelf);
        niceCredits.SetActive(!niceCredits.activeSelf);
    }
}
