using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	public sealed class Fog
	{
		[Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
		public bool enabled = true;

		[Tooltip("Should the fog affect the skybox?")]
		public bool excludeSkybox = true;

		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		internal bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && (bool)context.resources.shaders.deferredFog && context.resources.shaders.deferredFog.isSupported && context.camera.actualRenderingPath == RenderingPath.DeferredShading;
		}

		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
			propertySheet.ClearKeywords();
			Color c = (!RuntimeUtilities.isLinearColorSpace) ? RenderSettings.fogColor : RenderSettings.fogColor.linear;
			propertySheet.properties.SetVector(ShaderIDs.FogColor, c);
			propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			CommandBuffer command = context.command;
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, excludeSkybox ? 1 : 0);
		}
	}
}
