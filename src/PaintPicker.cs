using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PaintPicker : MonoBehaviour
{
	[Serializable]
	public class ColorEvent : UnityEvent<Color>
	{
	}

	public Color color;

	public Graphic colorPreviewBox;

	private RectTransform rect;

	private RectTransform parentRect;

	private Camera mainCamera;

	private Texture2D readPixTex;

	private bool allowPicking;

	public ColorEvent onColorPreview;

	public ColorEvent onColorPick;

	public void Initialize()
	{
		rect = GetComponent<RectTransform>();
		parentRect = rect.parent.GetComponent<RectTransform>();
		mainCamera = CustomizationController.instance.mainCamera;
	}

	private IEnumerator PickColor()
	{
		yield return new WaitForEndOfFrame();
		int width = 1;
		int height = 1;
		if (readPixTex == null)
		{
			readPixTex = new Texture2D(width, height, TextureFormat.RGB24, mipmap: false);
		}
		Texture2D texture2D = readPixTex;
		Vector3 mousePosition = Input.mousePosition;
		float x = mousePosition.x;
		Vector3 mousePosition2 = Input.mousePosition;
		texture2D.ReadPixels(new Rect(x, mousePosition2.y, width, height), 0, 0);
		readPixTex.Apply();
		Color sampledColor = readPixTex.GetPixel(0, 0);
		onColorPick.Invoke(sampledColor);
	}

	public void Process(bool show, bool pick)
	{
		if (base.isActiveAndEnabled != show)
		{
			base.gameObject.SetActive(show);
		}
		if (allowPicking != pick)
		{
			allowPicking = pick;
			if (allowPicking)
			{
				MenuCameraEffects.SuspendEffects(suspend: true);
				mainCamera.SetReplacementShader(Shaders.instance.customizeUnlitShader, "RenderType");
				mainCamera.renderingPath = RenderingPath.Forward;
			}
			else
			{
				MenuCameraEffects.SuspendEffects(suspend: false);
				mainCamera.ResetReplacementShader();
				mainCamera.renderingPath = RenderingPath.UsePlayerSettings;
			}
		}
		if (show)
		{
			Vector2 anchoredPosition = UICanvas.ScreenPointToLocal(parentRect, Input.mousePosition);
			rect.anchoredPosition = anchoredPosition;
			if (allowPicking)
			{
				StartCoroutine(PickColor());
			}
			colorPreviewBox.color = color;
		}
	}
}
