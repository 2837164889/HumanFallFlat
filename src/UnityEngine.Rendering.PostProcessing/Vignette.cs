using System;

namespace UnityEngine.Rendering.PostProcessing
{
	[Serializable]
	[PostProcess(typeof(VignetteRenderer), "Unity/Vignette", true)]
	public sealed class Vignette : PostProcessEffectSettings
	{
		[Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
		public VignetteModeParameter mode = new VignetteModeParameter
		{
			value = VignetteMode.Classic
		};

		[Tooltip("Vignette color. Use the alpha channel for transparency.")]
		public ColorParameter color = new ColorParameter
		{
			value = new Color(0f, 0f, 0f, 1f)
		};

		[Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
		public Vector2Parameter center = new Vector2Parameter
		{
			value = new Vector2(0.5f, 0.5f)
		};

		[Range(0f, 1f)]
		[Tooltip("Amount of vignetting on screen.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		[Range(0.01f, 1f)]
		[Tooltip("Smoothness of the vignette borders.")]
		public FloatParameter smoothness = new FloatParameter
		{
			value = 0.2f
		};

		[Range(0f, 1f)]
		[Tooltip("Lower values will make a square-ish vignette.")]
		public FloatParameter roundness = new FloatParameter
		{
			value = 1f
		};

		[Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
		public BoolParameter rounded = new BoolParameter
		{
			value = false
		};

		[Tooltip("A black and white mask to use as a vignette.")]
		public TextureParameter mask = new TextureParameter
		{
			value = null
		};

		[Range(0f, 1f)]
		[Tooltip("Mask opacity.")]
		public FloatParameter opacity = new FloatParameter
		{
			value = 1f
		};

		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return enabled.value && ((mode.value == VignetteMode.Classic && (intensity.value > 0f || opacity.value > 0f)) || (mode.value == VignetteMode.Masked && opacity.value > 0f && mask.value != null));
		}
	}
}
