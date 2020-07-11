using Multiplayer;
using UnityEngine;

public class CaveRender : MonoBehaviour
{
	private static float _defaultFogDensity;

	private static Color _defaultFogColor = Color.black;

	public static float fogDensityMultiplier = 1f;

	private Color creditsFogColor = new Color(142f / 255f, 154f / 255f, 32f / 51f);

	private Color defaultAmbientColor;

	public Color waterFogColor = Color.blue;

	public float waterFogDensity = 0.1f;

	private CaveLighting cave;

	private NetPlayer player;

	private Color fogColor;

	private float fogDensity;

	public static float defaultFogDensity => fogDensityMultiplier * ((!(Game.currentLevel != null)) ? _defaultFogDensity : Game.currentLevel.fogDensity);

	public static Color defaultFogColor => (!(Game.currentLevel != null)) ? _defaultFogColor : Game.currentLevel.fogColor;

	private void OnEnable()
	{
		player = GetComponentInParent<NetPlayer>();
		defaultAmbientColor = RenderSettings.ambientLight;
		if (defaultFogColor == Color.black)
		{
			_defaultFogColor = RenderSettings.fogColor;
			_defaultFogDensity = RenderSettings.fogDensity;
		}
	}

	private void OnPreRender()
	{
		FogMode fogMode = FogMode.ExponentialSquared;
		fogColor = Color.Lerp(defaultFogColor, creditsFogColor, MenuCameraEffects.instance.creditsAdjust);
		fogDensity = defaultFogDensity;
		if (player != null && player.cameraController.waterSensor != null && player.cameraController.waterSensor.waterBody != null)
		{
			Vector3 velocity;
			float num = player.cameraController.waterSensor.waterBody.SampleDepth(base.transform.position, out velocity) * 10f - 0.5f;
			fogColor = Color.Lerp(fogColor, waterFogColor, num);
			fogDensity = Mathf.Lerp(defaultFogDensity, waterFogDensity, num);
			if (num > 0f)
			{
				RenderSettings.fogMode = FogMode.Exponential;
			}
		}
		if (player != null)
		{
			cave = CaveLighting.GetCaveForPlayer(player);
			if (cave != null)
			{
				fogDensity *= Mathf.Lerp(1f, 0.1f, cave.GetPlaseForPlayer(player));
			}
		}
		if (RenderSettings.fogMode != fogMode)
		{
			RenderSettings.fogMode = fogMode;
		}
		if (RenderSettings.fogColor != fogColor)
		{
			RenderSettings.fogColor = fogColor;
		}
		if (RenderSettings.fogDensity != fogDensity)
		{
			RenderSettings.fogDensity = fogDensity;
		}
		for (int i = 0; i < Human.all.Count; i++)
		{
			if (Human.all[i].player.nametag != null)
			{
				Human.all[i].player.nametag.Align(base.transform);
			}
		}
	}
}
