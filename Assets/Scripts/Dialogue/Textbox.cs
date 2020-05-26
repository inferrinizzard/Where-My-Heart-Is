using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Textbox : MonoBehaviour
{
	Text text;
	Image image;
	[SerializeField] float padding = 6;
	Vector3[] pos = new Vector3[4];

	void Start()
	{
		text = transform.parent.GetComponentInChildren<DialogueSystem>().GetComponent<Text>();
		image = GetComponent<Image>();

		(transform as RectTransform).sizeDelta = new Vector2(text.preferredWidth + padding * 2, text.preferredHeight + padding * 2);
		(text.transform as RectTransform).GetWorldCorners(pos); // repeat operation every new text;
		float textX = pos[0].x;
		(transform as RectTransform).GetWorldCorners(pos);
		(transform as RectTransform).anchoredPosition = new Vector2((textX - pos[0].x) - padding * 2, 0);
	}

	void Update()
	{
		(transform as RectTransform).sizeDelta = new Vector2(text.preferredWidth + padding * 2, text.preferredHeight + padding * 2);
	}
}
