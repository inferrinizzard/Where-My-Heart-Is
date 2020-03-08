using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
	[HideInInspector] public Text textComponent;

	void Start()
	{
		textComponent = GetComponent<Text>();
	}

	public Prompt Enable()
	{
		textComponent.enabled = true;
		return this;
	}

	public void SetText(string t) => textComponent.text = t;
	public void Disable() => textComponent.enabled = false;
}
