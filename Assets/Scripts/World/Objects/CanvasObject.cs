using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasObject : InteractableObject
{
	[SerializeField] float threshold = 1f;
	bool pickingUp;

	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
		pickingUp = false;
	}

	private void Update()
	{
		if (pickingUp)
		{
			transform.position = Vector3.Lerp(transform.position, player.transform.position, 5f * Time.deltaTime);

			if (Vector3.Distance(transform.position, player.transform.position) < threshold)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public override void Interact()
	{
		pickingUp = true;
		GameManager.Instance.ChangeLevel(GameManager.Instance.levels[GameManager.Instance.sceneIndex + 1]);
	}
}
