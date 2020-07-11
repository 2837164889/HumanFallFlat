using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BlurRenderer), PostProcessEvent.AfterStack, "Custom/Blur", true)]
public sealed class Blur : PostProcessEffectSettings
{
	[Range(0f, 2f)]
	[Tooltip("Downsample")]
	public IntParameter downsample = new IntParameter
	{
		value = 1
	};

	[Range(0f, 10f)]
	[Tooltip("Blur Size")]
	public FloatParameter blurSize = new FloatParameter
	{
		value = 3f
	};

	[Range(1f, 4f)]
	[Tooltip("Blur iterations")]
	public IntParameter blurIterations = new IntParameter
	{
		value = 2
	};
}
