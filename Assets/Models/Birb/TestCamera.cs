using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestCamera : MonoBehaviour
{

	float rotationX, rotationY;
	[SerializeField] float mouseSensitivity = default;

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

}
