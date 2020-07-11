using UnityEngine;

public static class ColorExtensions
{
	private const float treshold = 0.004f;

	public static Color Desaturate(this Color color, float amount)
	{
		float num = (color.r + color.g + color.b) / 3f;
		return Color.Lerp(color, new Color(num, num, num), amount);
	}

	public static Color Brighten(this Color color, float amount)
	{
		return Color.Lerp(color, Color.white, amount);
	}

	public static Color Darken(this Color color, float amount)
	{
		return Color.Lerp(color, Color.black, amount);
	}

	public static bool Similar(this Color color, Color other)
	{
		Color color2 = color - other;
		return color2.r > -0.004f && color2.r < 0.004f && color2.g > -0.004f && color2.g < 0.004f && color2.b > -0.004f && color2.b < 0.004f;
	}

	public static float sqrMagnitude(this Color color)
	{
		return color.r * color.r + color.g * color.g + color.b * color.b;
	}
}
