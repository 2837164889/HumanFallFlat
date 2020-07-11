namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class LogHistogram
	{
		public const int rangeMin = -9;

		public const int rangeMax = 9;

		private const int k_Bins = 128;

		private int m_ThreadX;

		private int m_ThreadY;

		public ComputeBuffer data
		{
			get;
			private set;
		}

		public void Generate(PostProcessRenderContext context)
		{
			if (data == null)
			{
				m_ThreadX = 16;
				m_ThreadY = ((!RuntimeUtilities.isAndroidOpenGL) ? 16 : 8);
				data = new ComputeBuffer(128, 4);
			}
			Vector4 histogramScaleOffsetRes = GetHistogramScaleOffsetRes(context);
			ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("LogHistogram");
			int kernelIndex = exposureHistogram.FindKernel("KEyeHistogramClear");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", data);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(128f / (float)m_ThreadX), 1, 1);
			kernelIndex = exposureHistogram.FindKernel("KEyeHistogram");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", data);
			command.SetComputeTextureParam(exposureHistogram, kernelIndex, "_Source", context.source);
			command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(histogramScaleOffsetRes.z / 2f / (float)m_ThreadX), Mathf.CeilToInt(histogramScaleOffsetRes.w / 2f / (float)m_ThreadY), 1);
			command.EndSample("LogHistogram");
		}

		public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
		{
			float num = 18f;
			float num2 = 1f / num;
			float y = 9f * num2;
			return new Vector4(num2, y, context.width, context.height);
		}

		public void Release()
		{
			if (data != null)
			{
				data.Release();
			}
			data = null;
		}
	}
}
