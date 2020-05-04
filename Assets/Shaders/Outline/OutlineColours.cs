using UnityEngine;

[CreateAssetMenu(fileName = "Colours", menuName = "ScriptableObjects/OutlineColours", order = 1)]
public class OutlineColours : ScriptableObject
{
	[HideInInspector] public readonly Color Empty = Color.black;
	public Color Pushable = Color.green;
	public Color Pickupable = Color.yellow;
	public Color Canvas = Color.blue;
	public Color Interactable = Color.red;

#pragma warning disable 0184
	public Color this [MonoBehaviour I]
	{
		get
		{
			if (I is null)
				return Empty;

			var type = I.GetType();
			switch (type)
			{
				case var _ when type is CanvasObject:
					return Canvas;
				case var _ when type is Pushable:
					return Pushable;
				case var _ when type is Pickupable:
					return Pickupable;
				case var _ when(type is InteractableObject || type.IsSubclassOf(typeof(InteractableObject))):
					return Interactable;
			}
			return Empty;
		}
	}
#pragma warning restore 0184
}
