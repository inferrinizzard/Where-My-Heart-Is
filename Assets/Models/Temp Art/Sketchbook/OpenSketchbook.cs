using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSketchbook : MonoBehaviour
{
    /// <summary> Sketchbook animator. </summary>
    public Animator anim;
    /// <summary> Reference to main camera. </summary>
    public Camera cam;
    /// <summary> Reference to main menu UI elements. </summary>
    public CanvasGroup mainMenuUI;
    /// <summary> Reference to 'press any key to start' UI element. </summary>
    public Graphic pressAnyPrompt;
    /// <summary> Camera lerping variable. </summary>
    float transitionTime = 0f;
    /// <summary> Animation time for the canvas lerp in frames(?). </summary>
    float cameraAnimationTime = 50.0f;
    /// <summary> Whether the book is open or not. </summary>
    bool opened = false;
    /// <summary> Whether the main menu should fade in or not. </summary>
    bool mainMenuFade = false;
    /// <summary> Last position of the camera for lerp. </summary>
    Vector3 lastPosition;
    /// <summary> Target position of the camera for lerp. </summary>
    Vector3 targetPosition;
    /// <summary> Last rotation of the camera for lerp. </summary>
    Quaternion lastRotation;
    /// <summary> Target rotation of the camera for lerp. </summary>
    Quaternion targetRotation;

    private void Start()
    {
        lastPosition = cam.transform.position;

        // Set UI elements
        mainMenuUI.gameObject.SetActive(false);
        pressAnyPrompt.gameObject.SetActive(true);
        mainMenuUI.alpha = 0.0f;

        // Set camera lerp position and rotation
        targetPosition = new Vector3(0.75f, 6f, 0f);
        targetRotation.eulerAngles = new Vector3(90f, 0f, -180f);
    }

    private void Update()
    {
        // Reset these for the lerp
        transitionTime += Time.deltaTime;
        lastPosition = cam.transform.position;
        lastRotation = cam.transform.rotation;

        // Open the book when the 'any' key is pressed, wherever that is on a keyboard...
        if (Input.anyKeyDown && !opened) OpenBook();

        // Fade in the main menu UI elements
        if (mainMenuFade && mainMenuUI.alpha < 1.0) mainMenuUI.alpha += Time.deltaTime;
    }

    private void LateUpdate()
    {
        // Camera lerp
        if(opened)
        {
            cam.transform.position = Vector3.Lerp(lastPosition, targetPosition, transitionTime / cameraAnimationTime);
            cam.transform.rotation = Quaternion.Lerp(lastRotation, targetRotation, transitionTime / cameraAnimationTime);
        }
    }

    /// <summary> Opens the book and fades the UI in. </summary>
    private void OpenBook()
    {
        pressAnyPrompt.gameObject.SetActive(false);
        transitionTime = 0f;
        anim.SetTrigger("Open");
        opened = true;
        StartCoroutine(FadeInUI());
    }

    /// <summary> Fades the UI elements of the main menu in. </summary>
    private IEnumerator FadeInUI()
    {
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Fading in UI");
        mainMenuUI.gameObject.SetActive(true);
        mainMenuFade = true;
    }
}
