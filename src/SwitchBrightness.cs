using UnityEngine;

public class SwitchBrightness : MonoBehaviour
{
	public Shader shaderBrightness;

	private static float brightnessPower = 1f;

	private Material materialBrightness;

	private const int kMaxBrightness = 20;

	private const int kMidBrightness = 10;

	private const int kMinBrightness = 0;

	private const float kMaxBrightnessFloat = 20f;

	private const float kMidBrightnessPower = 1f;

	private const float kMinBrightnessCoef = 0.25f;

	private const float kMaxBrightnessCoef = 1.75f;

	public static void SetSwitchBrightness(int brightnessLevel)
	{
		if (brightnessLevel == 10 || brightnessLevel < 0 || brightnessLevel > 20)
		{
			brightnessPower = 1f;
			return;
		}
		brightnessPower = Mathf.Lerp(0.25f, 1.75f, (float)brightnessLevel / 20f);
		Debug.Log("SetSwitchBrightness: " + brightnessLevel + " " + brightnessPower);
	}

	public bool CheckResources()
	{
		if (materialBrightness == null)
		{
			materialBrightness = new Material(shaderBrightness);
			materialBrightness.hideFlags = HideFlags.DontSave;
		}
		return true;
	}
}
