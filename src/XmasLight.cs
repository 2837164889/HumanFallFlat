using UnityEngine;
using UnityEngine.Rendering;

public class XmasLight : MonoBehaviour
{
	public int priority;

	private Light light;

	private int syncedShadows = -1;

	private void Awake()
	{
		light = GetComponent<Light>();
		int advancedVideoShadows = Options.advancedVideoShadows;
		int num = advancedVideoShadows - priority;
		if (priority > 0)
		{
			light.shadowResolution = (LightShadowResolution)Mathf.Max(0, (int)(QualitySettings.shadowResolution - 1));
		}
		if (num >= 2)
		{
			light.enabled = true;
			light.shadows = LightShadows.Soft;
		}
		else if (num >= 1)
		{
			light.enabled = true;
			light.shadows = LightShadows.Hard;
		}
		else if (num >= 0)
		{
			light.enabled = true;
			light.shadows = LightShadows.None;
		}
		else
		{
			light.enabled = false;
		}
	}
}
