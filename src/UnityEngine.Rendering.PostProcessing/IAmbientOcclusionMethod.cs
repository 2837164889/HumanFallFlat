namespace UnityEngine.Rendering.PostProcessing
{
	public interface IAmbientOcclusionMethod
	{
		DepthTextureMode GetCameraFlags();

		void RenderAfterOpaque(PostProcessRenderContext context);

		void RenderAmbientOnly(PostProcessRenderContext context);

		void CompositeAmbientOnly(PostProcessRenderContext context);

		void Release();
	}
}
