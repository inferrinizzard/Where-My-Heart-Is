using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public JsonParsable json;
    public string path;

    public Text uiText;
    public string inputText;
    private string[] textQueue;

    private bool texting = false;
    private int count = 0;
    private int activeLineCount = 0;
    private float textSpeed = 0.1f;

    private void Start()
    {
        path = Application.dataPath + "/Scripts/Dialogue/" + path;
        Debug.Log(path);
        string jsonString = File.ReadAllText(path);
        Debug.Log(jsonString);
        json = JsonUtility.FromJson<JsonParsable>(jsonString);
        inputText = json.text;
        ParseText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && activeLineCount == 0)
        {
            StartCoroutine(WriteDialogue(0));
        }
    }

    public void ParseText()
    {
        textQueue = inputText.Split('/');
    }

    public IEnumerator WriteDialogue(float delay)
    {
        if(activeLineCount != 0) yield return new WaitForSeconds(delay);
        if (!texting && activeLineCount < textQueue.Length)
        {
            texting = true;
            InvokeRepeating("AdvanceText", 0f, textSpeed);
        }
        else
        {
            EndText();
        }
    }

    public void AdvanceText()
    {
        if (count <= textQueue[activeLineCount].Length && texting)
        {
            uiText.text = textQueue[activeLineCount].Substring(0, count);
            count++;
        }
        else
        {
            CancelInvoke("AdvanceText");
            texting = false;
            count = 0;
            activeLineCount++;
            StartCoroutine(WriteDialogue(1));
        }
    }

    public void EndText()
    {
        uiText.text = "";
    }
}
