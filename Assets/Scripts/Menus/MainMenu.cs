using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /// <summary> Local instance of main menu canvas objects. </summary>
    public GameObject mainMenuUI;
    /// <summary> Local instance of options menu canvas objects. </summary>
    public GameObject optionsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        CloseOptions();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseOptions();
    }

    public void OpenOptions()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    public void CloseOptions()
    {
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
    }
}
