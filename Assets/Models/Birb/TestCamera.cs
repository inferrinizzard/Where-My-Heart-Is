using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestCamera : MonoBehaviour
{
	public Shader shader;
	Material mat;

	float rotationX, rotationY;
	[SerializeField] float mouseSensitivity = default;
	// Start is called before the first frame update
	void Start()
	{
		transform.LookAt(FindObjectOfType<BirdTrail>().transform.position);
		mat = new Material(shader);
	}

	// Update is called once per frame
	void Update()
	{
		var hor = Input.GetAxis("Horizontal");
		if (hor != 0)
			transform.Translate(Vector3.right * hor);

		var ver = Input.GetAxis("Vertical");
		if (ver != 0)
			transform.Translate(Vector3.forward * ver);

		rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
		rotationX += Input.GetAxis("Mouse Y") * mouseSensitivity;
		transform.localEulerAngles = new Vector3(-rotationX, rotationY, 0);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, mat);
	}
}
