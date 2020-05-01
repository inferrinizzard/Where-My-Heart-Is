using System.Linq;

using UnityEngine;

[ExecuteInEditMode]
public class DeathPlane : MonoBehaviour
{
#if UNITY_EDITOR
	World world;
	void OnEnable() => world = FindObjectOfType<World>();
	void Update()
	{
		if (Application.isEditor && !Application.isPlaying)
		{
			Renderer[] rends = world.GetComponentsInChildren<Renderer>();
			if (rends.Length > 0)
				transform.position = new Vector3(0, rends?. /*Where(r => r.bounds.size.y > Mathf.Abs(transform.position.y)).*/ Select(r => r.bounds.min.y).OrderBy(r => r).Min(r => r) - 5 ?? -5, 0);
		}
		else
			Destroy(this);
	}
#endif
}
