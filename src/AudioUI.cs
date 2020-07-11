using System;
using UnityEngine;

public static class AudioUI
{
	private static readonly Texture2D backgroundTexture = Texture2D.whiteTexture;

	private static readonly GUIStyle textureStyle = new GUIStyle
	{
		normal = new GUIStyleState
		{
			background = backgroundTexture
		}
	};

	public static GUIStyle style;

	public static GUIStyle buttonStyle;

	public static void DrawRect(Rect rect, Color color)
	{
		Color backgroundColor = GUI.backgroundColor;
		GUI.backgroundColor = color;
		GUI.Box(rect, GUIContent.none, textureStyle);
		GUI.backgroundColor = backgroundColor;
	}

	public static string FormatNumber(float num, int len = 5)
	{
		if (float.IsInfinity(num))
		{
			return "-Inf";
		}
		return num.ToString("0.0");
	}

	private static float DBtoFader(float db)
	{
		if (db < -24f)
		{
			return Mathf.Pow(2f, (db + 24f) / 24f) / 2f;
		}
		return (db + 48f) / 48f;
	}

	private static float FaderToDB(float fader)
	{
		if (fader < 0.5f)
		{
			return Mathf.Log(fader * 2f, 2f) * 24f - 24f;
		}
		return -48f + fader * 48f;
	}

	public static void EnsureStyle()
	{
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.GetStyle("label"));
			style.fontSize = 10;
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.MiddleCenter;
		}
		if (buttonStyle == null)
		{
			buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));
			buttonStyle.fontSize = 10;
		}
	}

	public static float DrawHorizontalSlider(Rect rect, float from, float to, float def, float value, AudioSliderType type)
	{
		EnsureStyle();
		GUI.color = new Color(0f, 0f, 0f, 0f);
		bool flag = Event.current.button == 1 && rect.Contains(Event.current.mousePosition);
		if (flag)
		{
			value = def;
		}
		float num;
		switch (type)
		{
		case AudioSliderType.Level:
			num = value;
			break;
		case AudioSliderType.Volume:
			num = DBtoFader(AudioUtils.ValueToDB(value));
			break;
		case AudioSliderType.Pitch:
			num = AudioUtils.RatioToCents(value);
			break;
		case AudioSliderType.Linear:
			num = value;
			break;
		case AudioSliderType.Log2:
			num = Mathf.Log(value, 2f);
			from = Mathf.Log(from, 2f);
			to = Mathf.Log(to, 2f);
			break;
		case AudioSliderType.Log10:
			num = Mathf.Log(value, 10f);
			from = Mathf.Log(from, 10f);
			to = Mathf.Log(to, 10f);
			break;
		default:
			throw new InvalidOperationException();
		}
		float num2 = (!flag) ? GUI.HorizontalSlider(rect, num, from, to) : num;
		GUI.color = Color.white;
		DrawRect(new Rect(rect.x + 4f, rect.y + 4f, rect.width - 6f, rect.height - 8f), new Color(0.1f, 0.1f, 0.1f, 1f));
		DrawRect(new Rect(rect.x + 5f, rect.y + 5f, (rect.width - 8f) * Mathf.InverseLerp(from, to, num2), rect.height - 10f), new Color(0.25f, 0.5f, 0.25f, 1f));
		float num3;
		switch (type)
		{
		case AudioSliderType.Level:
			num3 = num2;
			break;
		case AudioSliderType.Volume:
			num3 = FaderToDB(num2);
			num2 = ((num2 != num) ? AudioUtils.DBToValue(FaderToDB(num2)) : value);
			break;
		case AudioSliderType.Pitch:
			num3 = num2;
			num2 = ((num2 != num) ? AudioUtils.CentsToRatio(num2) : value);
			break;
		case AudioSliderType.Linear:
			num3 = num2;
			break;
		case AudioSliderType.Log2:
			num2 = ((num2 != num) ? Mathf.Pow(2f, num2) : value);
			num3 = num2;
			break;
		case AudioSliderType.Log10:
			num2 = ((num2 != num) ? Mathf.Pow(10f, num2) : value);
			num3 = num2;
			break;
		default:
			throw new InvalidOperationException();
		}
		GUI.Label(new Rect(rect.x, rect.y, rect.width, rect.height), FormatNumber(num3), style);
		return num2;
	}
}
