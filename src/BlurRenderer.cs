using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public sealed class BlurRenderer : PostProcessEffectRenderer<Blur>
{
	private int[] ids;

	public override void Init()
	{
		base.Init();
		ids = new int[(int)base.settings.blurIterations * 2 + 1];
		int num = 0;
		ids[num++] = Shader.PropertyToID("_RT");
		for (int i = 0; i < (int)base.settings.blurIterations; i++)
		{
			ids[num++] = Shader.PropertyToID("_RT1" + i);
			ids[num++] = Shader.PropertyToID("_RT2" + i);
		}
	}

	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Blur");
		PropertySheet propertySheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Blur"));
		float num = 1f / (1f * (float)(1 << (int)base.settings.downsample));
		propertySheet.properties.SetVector("_Parameter", new Vector4((float)base.settings.blurSize * num, (0f - (float)base.settings.blurSize) * num, 0f, 0f));
		int widthOverride = context.screenWidth >> (int)base.settings.downsample;
		int heightOverride = context.screenHeight >> (int)base.settings.downsample;
		int num2 = 0;
		context.GetScreenSpaceTemporaryRT(command, ids[num2], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, widthOverride, heightOverride);
		command.BlitFullscreenTriangle(context.source, ids[num2], propertySheet, 0);
		for (int i = 0; i < (int)base.settings.blurIterations; i++)
		{
			float num3 = (float)i * 1f;
			propertySheet.properties.SetVector("_Parameter", new Vector4((float)base.settings.blurSize * num + num3, (0f - (float)base.settings.blurSize) * num - num3, 0f, 0f));
			context.GetScreenSpaceTemporaryRT(command, ids[++num2], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, widthOverride, heightOverride);
			command.BlitFullscreenTriangle(ids[num2 - 1], ids[num2], propertySheet, 1);
			command.ReleaseTemporaryRT(ids[num2 - 1]);
			context.GetScreenSpaceTemporaryRT(command, ids[++num2], 0, context.sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, widthOverride, heightOverride);
			command.BlitFullscreenTriangle(ids[num2 - 1], ids[num2], propertySheet, 2);
			command.ReleaseTemporaryRT(ids[num2 - 1]);
		}
		command.BlitFullscreenTriangle(ids[num2], context.destination, propertySheet, 0);
		command.ReleaseTemporaryRT(ids[num2]);
		command.EndSample("Blur");
	}
}
