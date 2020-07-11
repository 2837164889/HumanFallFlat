namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
	{
		private enum Pass
		{
			VelocitySetup,
			TileMax1,
			TileMax2,
			TileMaxV,
			NeighborMax,
			Reconstruction
		}

		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			if (m_ResetHistory)
			{
				command.BlitFullscreenTriangle(context.source, context.destination);
				m_ResetHistory = false;
				return;
			}
			RenderTextureFormat format = RenderTextureFormat.RGHalf;
			RenderTextureFormat format2 = RenderTextureFormat.ARGB2101010.IsSupported() ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32;
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
			command.BeginSample("MotionBlur");
			int num = (int)(5f * (float)context.height / 100f);
			int num2 = ((num - 1) / 8 + 1) * 8;
			float value = (float)base.settings.shutterAngle / 360f;
			propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, num);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
			int velocityTex = ShaderIDs.VelocityTex;
			command.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, format2, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, velocityTex, propertySheet, 0);
			int tile2RT = ShaderIDs.Tile2RT;
			command.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(velocityTex, tile2RT, propertySheet, 1);
			int tile4RT = ShaderIDs.Tile4RT;
			command.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile2RT, tile4RT, propertySheet, 2);
			command.ReleaseTemporaryRT(tile2RT);
			int tile8RT = ShaderIDs.Tile8RT;
			command.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile4RT, tile8RT, propertySheet, 2);
			command.ReleaseTemporaryRT(tile4RT);
			Vector2 v = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
			propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, v);
			propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (int)((float)num2 / 8f));
			int tileVRT = ShaderIDs.TileVRT;
			command.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile8RT, tileVRT, propertySheet, 3);
			command.ReleaseTemporaryRT(tile8RT);
			int neighborMaxTex = ShaderIDs.NeighborMaxTex;
			int width = context.width / num2;
			int height = context.height / num2;
			command.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tileVRT, neighborMaxTex, propertySheet, 4);
			command.ReleaseTemporaryRT(tileVRT);
			propertySheet.properties.SetFloat(ShaderIDs.LoopCount, Mathf.Clamp((int)base.settings.sampleCount / 2, 1, 64));
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5);
			command.ReleaseTemporaryRT(velocityTex);
			command.ReleaseTemporaryRT(neighborMaxTex);
			command.EndSample("MotionBlur");
		}
	}
}
