using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{
	Transform player, cam;

	void OnEnable()
	{
		player = FindObjectOfType<Player>().transform;
		cam = player.GetComponentInChildren<Camera>().transform;
	}
	void Update()
	{
		if (Application.isPlaying)
			return;

		transform.position = player.position;
		transform.eulerAngles = new Vector3(cam.eulerAngles.x, player.eulerAngles.y, 0);
	}
}
