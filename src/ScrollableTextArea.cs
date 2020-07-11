using TMPro;
using UnityEngine;

public class ScrollableTextArea : MonoBehaviour
{
	public TextMeshProUGUI textArea;

	private float scrollPos;

	private float totalHeight;

	private float viewPortHeight;

	private Vector3 startPos;

	private float tweenFrom;

	private float tweenTo;

	private float tweenPhase = 1f;

	private float tweenDuration = 0.3f;

	private void OnEnable()
	{
		textArea = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (textArea == null)
		{
			return;
		}
		if (totalHeight != textArea.preferredHeight)
		{
			totalHeight = textArea.preferredHeight;
			viewPortHeight = textArea.rectTransform.rect.height;
			startPos = textArea.rectTransform.localPosition;
		}
		if (!(totalHeight > viewPortHeight))
		{
			return;
		}
		Vector2 mouseScrollDelta = Input.mouseScrollDelta;
		float num = mouseScrollDelta.y * 100f;
		num += (Options.controllerBindings.Move.Y + Options.keyboardBindings.Move.Y) * Time.unscaledDeltaTime * 500f;
		if (num != 0f)
		{
			float value = tweenTo - num;
			value = Mathf.Clamp(value, 0f, totalHeight - viewPortHeight);
			if (value != tweenTo)
			{
				tweenTo = value;
				tweenFrom = scrollPos;
				tweenPhase = 0f;
			}
		}
		if (tweenPhase != 1f)
		{
			tweenPhase += Time.unscaledDeltaTime / tweenDuration;
			tweenPhase = Mathf.Clamp01(tweenPhase);
			float t = Ease.easeOutQuad(0f, 1f, tweenPhase);
			scrollPos = Mathf.Lerp(tweenFrom, tweenTo, t);
			textArea.rectTransform.localPosition = startPos + new Vector3(0f, scrollPos, 0f);
		}
	}
}
