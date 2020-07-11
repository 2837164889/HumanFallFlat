using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(LensDistortionRenderer), "Unity/Lens Distortion", true)]
	public sealed class LensDistortion : PostProcessEffectSettings
	{
		[Range(-100f, 100f)]
		[Tooltip("Total distortion amount.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		[Range(0f, 1f)]
		[DisplayName("Y Multiplier")]
		[Tooltip("Intensity multiplier on X axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityX = new FloatParameter
		{
			value = 1f
		};

		[Range(0f, 1f)]
		[DisplayName("X Multiplier")]
		[Tooltip("Intensity multiplier on Y axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityY = new FloatParameter
		{
			value = 1f
		};

		[Space]
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (X axis).")]
		public FloatParameter centerX = new FloatParameter
		{
			value = 0f
		};

		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (Y axis).")]
		public FloatParameter centerY = new FloatParameter
		{
			value = 0f
		};

		[Space]
		[Range(0.01f, 5f)]
		[Tooltip("Global screen scaling.")]
		public FloatParameter scale = new FloatParameter
		{
			value = 1f
		};

		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value && !Mathf.Approximately(intensity, 0f) && ((float)intensityX > 0f || (float)intensityY > 0f) && !RuntimeUtilities.isVREnabled;
		}
	}
}
