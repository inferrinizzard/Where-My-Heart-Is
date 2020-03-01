﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : InteractableObject
{
	public float threshold;

	private bool pickingUp;
	private Vector3 spatialTarget;
	private Vector3 rotationalTarget;

	[SerializeField] string sceneTarget = "";

	protected override void Start()
	{
		base.Start();
		pickingUp = false;
	}

	private void Update()
	{
		if (pickingUp)
		{
			transform.position = Vector3.Lerp(transform.position, spatialTarget, 5f * Time.deltaTime);

			if (Vector3.Distance(transform.position, spatialTarget) < threshold)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public override void Interact()
	{
        Debug.Log("here");
		spatialTarget = player.transform.position;
		rotationalTarget = Quaternion.LookRotation(player.transform.forward, Vector3.up).eulerAngles;
		pickingUp = true;
		if (sceneTarget != "")
			GameManager.Instance.ChangeLevel(sceneTarget);
	}

}
