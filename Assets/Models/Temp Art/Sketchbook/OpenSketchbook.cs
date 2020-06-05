using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSketchbook : MonoBehaviour
{
    public Animator anim;
    public Camera cam;
    public GameObject mainMenuUI;
    public GameObject pressAnyPrompt;

    float transitionTime = 0f;
    float cameraAnimationTime = 50.0f;

    bool opened = false;

    Vector3 lastPosition;
    Vector3 targetPosition;

    Quaternion lastRotation;
    Quaternion targetRotation;

    private void Start()
    {
        lastPosition = cam.transform.position;
        mainMenuUI.SetActive(false);
        pressAnyPrompt.SetActive(true);
        targetPosition = new Vector3(0.75f, 6f, 0f);
        targetRotation.eulerAngles = new Vector3(90f, 0f, -180f);
    }

    private void Update()
    {
        // Reset these for the lerp
        transitionTime += Time.deltaTime;
        lastPosition = cam.transform.position;
        lastRotation = cam.transform.rotation;

        if (Input.anyKeyDown && !opened) OpenBook();
    }

    private void LateUpdate()
    {
        if(opened)
        {
            cam.transform.position = Vector3.Lerp(lastPosition, targetPosition, transitionTime / cameraAnimationTime);
            cam.transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, transitionTime / cameraAnimationTime);
        }
    }

    private void OpenBook()
    {
        pressAnyPrompt.SetActive(false);
        transitionTime = 0f;
        anim.SetTrigger("Open");
        opened = true;
        StartCoroutine(FadeInUI());
    }

    private IEnumerator FadeInUI()
    {
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Fading in UI");
        mainMenuUI.SetActive(true);
    }
}
