using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(AmbientOcclusionRenderer), "Unity/Ambient Occlusion", true)]
	public sealed class AmbientOcclusion : PostProcessEffectSettings
	{
		[Tooltip("The ambient occlusion method to use. \"MSVO\" is higher quality and faster on desktop & console platforms but requires compute shader support.")]
		public AmbientOcclusionModeParameter mode = new AmbientOcclusionModeParameter
		{
			value = AmbientOcclusionMode.MultiScaleVolumetricObscurance
		};

		[Range(0f, 4f)]
		[Tooltip("Degree of darkness added by ambient occlusion.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		[ColorUsage(false)]
		[Tooltip("Custom color to use for the ambient occlusion.")]
		public ColorParameter color = new ColorParameter
		{
			value = Color.black
		};

		[Tooltip("Only affects ambient lighting. This mode is only available with the Deferred rendering path and HDR rendering. Objects rendered with the Forward rendering path won't get any ambient occlusion.")]
		public BoolParameter ambientOnly = new BoolParameter
		{
			value = true
		};

		[Range(-8f, 0f)]
		public FloatParameter noiseFilterTolerance = new FloatParameter
		{
			value = 0f
		};

		[Range(-8f, -1f)]
		public FloatParameter blurTolerance = new FloatParameter
		{
			value = -4.6f
		};

		[Range(-12f, -1f)]
		public FloatParameter upsampleTolerance = new FloatParameter
		{
			value = -12f
		};

		[Range(1f, 10f)]
		[Tooltip("Modifies thickness of occluders. This increases dark areas but also introduces dark halo around objects.")]
		public FloatParameter thicknessModifier = new FloatParameter
		{
			value = 1f
		};

		[Range(0f, 1f)]
		[Tooltip("")]
		public FloatParameter directLightingStrength = new FloatParameter
		{
			value = 0f
		};

		[Tooltip("Radius of sample points, which affects extent of darkened areas.")]
		public FloatParameter radius = new FloatParameter
		{
			value = 0.25f
		};

		[Tooltip("Number of sample points, which affects quality and performance. Lowest, Low & Medium passes are downsampled. High and Ultra are not and should only be used on high-end hardware.")]
		public AmbientOcclusionQualityParameter quality = new AmbientOcclusionQualityParameter
		{
			value = AmbientOcclusionQuality.Medium
		};

		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			bool flag = enabled.value && intensity.value > 0f;
			if (mode.value == AmbientOcclusionMode.ScalableAmbientObscurance)
			{
				flag &= !RuntimeUtilities.scriptableRenderPipelineActive;
				if (context != null)
				{
					flag &= ((bool)context.resources.shaders.scalableAO && context.resources.shaders.scalableAO.isSupported);
				}
			}
			else if (mode.value == AmbientOcclusionMode.MultiScaleVolumetricObscurance)
			{
				if (context != null)
				{
					flag &= ((bool)context.resources.shaders.multiScaleAO && context.resources.shaders.multiScaleAO.isSupported && (bool)context.resources.computeShaders.multiScaleAODownsample1 && (bool)context.resources.computeShaders.multiScaleAODownsample2 && (bool)context.resources.computeShaders.multiScaleAORender && (bool)context.resources.computeShaders.multiScaleAOUpsample);
				}
				flag &= (SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && RenderTextureFormat.RFloat.IsSupported() && RenderTextureFormat.RHalf.IsSupported() && RenderTextureFormat.R8.IsSupported());
			}
			return flag;
		}
	}
}
