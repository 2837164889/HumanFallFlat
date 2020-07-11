using UnityEngine;

public class ShadowSettingsOverride : MonoBehaviour
{
	private void Update()
	{
		if (QualitySettings.shadows != ShadowQuality.All)
		{
			QualitySettings.shadows = ShadowQuality.All;
		}
	}
}
