using UnityEngine;
using UnityEngine.Rendering;

public class CaveShadowDisable : MonoBehaviour
{
	private bool shadowsEnabled = true;

	private MeshRenderer[] renderers;

	private void OnEnable()
	{
		renderers = GetComponentsInChildren<MeshRenderer>();
	}

	public void DisableShadow()
	{
		if (shadowsEnabled)
		{
			shadowsEnabled = false;
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].shadowCastingMode = ShadowCastingMode.Off;
			}
		}
	}

	public void EnableShadow()
	{
		if (!shadowsEnabled)
		{
			shadowsEnabled = true;
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].shadowCastingMode = ShadowCastingMode.On;
			}
		}
	}
}
