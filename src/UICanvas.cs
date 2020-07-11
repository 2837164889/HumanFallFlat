using UnityEngine;

public class UICanvas : MonoBehaviour
{
	public static UICanvas instance;

	public static RectTransform rectTransform;

	public static Camera camera;

	private void OnEnable()
	{
		instance = this;
		rectTransform = (base.transform as RectTransform);
		camera = GetComponent<Canvas>().worldCamera;
	}

	public static Vector2 ScreenPointToLocal(Vector2 pos)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pos, camera, out Vector2 localPoint);
		return localPoint;
	}

	public static Vector2 ScreenPointToLocal(RectTransform rect, Vector2 pos)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pos, camera, out Vector2 localPoint);
		return localPoint;
	}
}
