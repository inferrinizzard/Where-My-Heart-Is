using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles the inputting of text from a json file and displaying of text onto the screen. </summary>
public class DialogueSystem : MonoBehaviour
{
	/// <summary> File name of the JSON to parse. </summary>
	[SerializeField] private string fileName;
	/// <summary> The Unity UI text object for the text to be displayed through. </summary>
	[SerializeField] private Text uiText = default;

	/// <summary> A string to hold the input text from the JSON file. </summary>
	private string inputText;
	/// <summary> The array of the lines to be displayed. </summary>
	/// <remarks> Split via the "/" character from inputText. </remarks>
	private string[] textQueue;
	/// <summary> Whether the current line is being typed or not. </summary>
	private bool typing = false;
	/// <summary> The current index of the character being typed. </summary>
	private int charCount = 0;
	/// <summary> The current index of the line being typed. </summary>
	private int activeLineCount = 0;
	/// <summary> The speed that the text types at. </summary>
	private float textSpeed = 0.04f;
	/// <summary> An object reference to parse the JSON into. </summary>
	private JsonParsable json;

	private void Start()
	{
		ParseText();
	}

	void Update()
	{
		/*if (Input.GetKeyDown(KeyCode.Z) && activeLineCount == 0 && !typing) // For testing, start the dialogue process when the Z key is pressed.
		{
			StartCoroutine(WriteDialogue("Heres some string or something"));
		}*/
	}

	/// <summary> Parses the input JSON file into the text queue. </summary>
	public void ParseText()
	{
		fileName = Application.dataPath + "/Scripts/Dialogue/" + fileName; // This will need to change once a folder has been created for dialogue
		string jsonString = File.ReadAllText(fileName);
		json = JsonUtility.FromJson<JsonParsable>(jsonString);
		inputText = json.text;
		textQueue = inputText.Split('/');
	}

	/// <summary> Types out the current line of dialogue. </summary>
	/// <param name="delay"> The time in seconds to wait before typing out the line. </param>
	public IEnumerator WriteDialogue(float delay)
	{
		if (activeLineCount != 0)yield return new WaitForSeconds(delay);

		if (!typing && activeLineCount < textQueue.Length)
		{
			typing = true;
			InvokeRepeating("AdvanceText", 0f, textSpeed);
		}
		else
		{
			EndText(); // Clears text once there are no more lines to display.
		}
	}

    public IEnumerator WriteDialogue(string text)
    {
        /*textQueue = new string[1];
        textQueue[0] = text;*/
        textQueue = text.Split('/');
        activeLineCount = 0;
        charCount = 0;
        if (!typing && activeLineCount < textQueue.Length)
        {
            typing = true;
            InvokeRepeating("AdvanceText", 0f, textSpeed);
        }
        yield return null;
    }

	/// <summary> Types out the current line character by character, then resets variables for recursive call to WriteDialogue. </summary>
	public void AdvanceText()
	{
		if (charCount <= textQueue[activeLineCount].Length && typing)
		{
			uiText.text = textQueue[activeLineCount].Substring(0, charCount);
			charCount++;
		}
		else
		{
			CancelInvoke("AdvanceText");
			typing = false;
			charCount = 0;
			activeLineCount++;
            float delay = (1f / 30f) * textQueue[activeLineCount - 1].Length;
            delay = Mathf.Clamp(delay, 0.5f, 1f);
			StartCoroutine(WriteDialogue(delay));
		}
	}

	/// <summary> Sets the UI text to be empty. </summary>
	public void EndText()
	{
		uiText.text = "";
	}
}
