using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class Dithering
	{
		private int m_NoiseTextureIndex;

		internal void Render(PostProcessRenderContext context)
		{
			Texture2D[] blueNoise = context.resources.blueNoise64;
			if (++m_NoiseTextureIndex >= blueNoise.Length)
			{
				m_NoiseTextureIndex = 0;
			}
			float value = Random.value;
			float value2 = Random.value;
			Texture2D texture2D = blueNoise[m_NoiseTextureIndex];
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.properties.SetTexture(ShaderIDs.DitheringTex, texture2D);
			uberSheet.properties.SetVector(ShaderIDs.Dithering_Coords, new Vector4((float)context.screenWidth / (float)texture2D.width, (float)context.screenHeight / (float)texture2D.height, value, value2));
		}
	}
}
