using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "BridgeBehaviour", menuName = "Levels/Behaviours/BridgeBehaviour")]
public class BridgeBehaviour : LevelBehaviour
{
	public static bool forcePrompt = false;
	string heartPromptText = $"Press and Hold {Prompt.ParseKey(InputManager.altAimKey.ToString())} or Right Click to use Window";
	Prompt prompt;
	WindowMaskAnimation window;
	public void Init()
	{
		prompt = GameManager.Instance.prompt;
		window = Player.Instance.cam.GetComponent<WindowMaskAnimation>();
	}
	public void TutorialPrompt()
	{
		forcePrompt = true;
		prompt.SetText(heartPromptText);
		GameManager.Instance.CustomUpdate += MaterialisePrompt;
		// Debug.Log(GameManager.Instance.prompt.GetComponent<UnityEngine.UI.Text>().text);
	}

	void MaterialisePrompt()
	{
		if (window.openingWindow && !prompt.disabled)
			prompt.Disable();
		if (!window.openingWindow)
		{
			if (prompt.disabled)
				prompt.SetText(Player.Instance.State is Aiming ? "Press Left Click to Cut" : heartPromptText);
			else if (Player.Instance.State is null)
				prompt.SetText(heartPromptText);
		}
		if (Input.GetMouseButtonDown(0))
			GameManager.Instance.StartCoroutine(WaitAndReprompt());
		// prompt.SetText(heartPromptText);
	}

	IEnumerator WaitAndReprompt()
	{
		yield return new WaitForEndOfFrame();
		if (Player.Instance.State is Aiming)
			yield break;
		yield return new WaitForSeconds(0.5f);
		prompt.SetText(heartPromptText);
	}

	public void DisablePrompt()
	{
		forcePrompt = false;
		GameManager.Instance.prompt.Disable();
		GameManager.Instance.CustomUpdate -= MaterialisePrompt;
	}
}
