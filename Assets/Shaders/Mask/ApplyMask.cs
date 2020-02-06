using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ApplyMask : MonoBehaviour
{
	[SerializeField] Camera realCam = default;
	Camera dreamCam;

	[SerializeField] Shader merge = default;
	Material screenMat;
	// Start is called before the first frame update
	void Start()
	{
		screenMat = new Material(merge);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
