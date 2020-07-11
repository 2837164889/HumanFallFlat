using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	internal class TextureLerper
	{
		private static TextureLerper m_Instance;

		private CommandBuffer m_Command;

		private PropertySheetFactory m_PropertySheets;

		private PostProcessResources m_Resources;

		private List<RenderTexture> m_Recycled;

		private List<RenderTexture> m_Actives;

		internal static TextureLerper instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new TextureLerper();
				}
				return m_Instance;
			}
		}

		private TextureLerper()
		{
			m_Recycled = new List<RenderTexture>();
			m_Actives = new List<RenderTexture>();
		}

		internal void BeginFrame(PostProcessRenderContext context)
		{
			m_Command = context.command;
			m_PropertySheets = context.propertySheets;
			m_Resources = context.resources;
		}

		internal void EndFrame()
		{
			if (m_Recycled.Count > 0)
			{
				foreach (RenderTexture item in m_Recycled)
				{
					RuntimeUtilities.Destroy(item);
				}
				m_Recycled.Clear();
			}
			if (m_Actives.Count > 0)
			{
				foreach (RenderTexture active in m_Actives)
				{
					m_Recycled.Add(active);
				}
				m_Actives.Clear();
			}
		}

		private RenderTexture Get(RenderTextureFormat format, int w, int h, int d = 1, bool enableRandomWrite = false, bool force3D = false)
		{
			RenderTexture renderTexture = null;
			int count = m_Recycled.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				RenderTexture renderTexture2 = m_Recycled[i];
				if (renderTexture2.width == w && renderTexture2.height == h && renderTexture2.volumeDepth == d && renderTexture2.format == format && renderTexture2.enableRandomWrite == enableRandomWrite && (!force3D || renderTexture2.dimension == TextureDimension.Tex3D))
				{
					renderTexture = renderTexture2;
					break;
				}
			}
			if (renderTexture == null)
			{
				TextureDimension dimension = (d <= 1 && !force3D) ? TextureDimension.Tex2D : TextureDimension.Tex3D;
				RenderTexture renderTexture3 = new RenderTexture(w, h, 0, format);
				renderTexture3.dimension = dimension;
				renderTexture3.filterMode = FilterMode.Bilinear;
				renderTexture3.wrapMode = TextureWrapMode.Clamp;
				renderTexture3.anisoLevel = 0;
				renderTexture3.volumeDepth = d;
				renderTexture3.enableRandomWrite = enableRandomWrite;
				renderTexture = renderTexture3;
				renderTexture.Create();
			}
			else
			{
				m_Recycled.RemoveAt(i);
			}
			m_Actives.Add(renderTexture);
			return renderTexture;
		}

		internal Texture Lerp(Texture from, Texture to, float t)
		{
			if (from == to)
			{
				return from;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = (!(from is Texture3D)) ? ((RenderTexture)from).volumeDepth : ((Texture3D)from).depth;
				int a = Mathf.Max(from.width, from.height);
				a = Mathf.Max(a, num);
				renderTexture = Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, enableRandomWrite: true, force3D: true);
				ComputeShader texture3dLerp = m_Resources.computeShaders.texture3dLerp;
				int kernelIndex = texture3dLerp.FindKernel("KTexture3DLerp");
				m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4(from.width, from.height, num, t));
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_Output", renderTexture);
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_From", from);
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_To", to);
				int num2 = Mathf.CeilToInt((float)a / 8f);
				int threadGroupsZ = Mathf.CeilToInt((float)a / 8f);
				m_Command.DispatchCompute(texture3dLerp, kernelIndex, num2, num2, threadGroupsZ);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(to);
			renderTexture = Get(uncompressedRenderTextureFormat, to.width, to.height);
			PropertySheet propertySheet = m_PropertySheets.Get(m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetTexture(ShaderIDs.To, to);
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 0);
			return renderTexture;
		}

		internal Texture Lerp(Texture from, Color to, float t)
		{
			if ((double)t < 1E-05)
			{
				return from;
			}
			RenderTexture renderTexture;
			if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
			{
				int num = (!(from is Texture3D)) ? ((RenderTexture)from).volumeDepth : ((Texture3D)from).depth;
				int a = Mathf.Max(from.width, from.height);
				a = Mathf.Max(a, num);
				renderTexture = Get(RenderTextureFormat.ARGBHalf, from.width, from.height, num, enableRandomWrite: true, force3D: true);
				ComputeShader texture3dLerp = m_Resources.computeShaders.texture3dLerp;
				int kernelIndex = texture3dLerp.FindKernel("KTexture3DLerpToColor");
				m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4(from.width, from.height, num, t));
				m_Command.SetComputeVectorParam(texture3dLerp, "_TargetColor", new Vector4(to.r, to.g, to.b, to.a));
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_Output", renderTexture);
				m_Command.SetComputeTextureParam(texture3dLerp, kernelIndex, "_From", from);
				int num2 = Mathf.CeilToInt((float)a / 4f);
				m_Command.DispatchCompute(texture3dLerp, kernelIndex, num2, num2, num2);
				return renderTexture;
			}
			RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(from);
			renderTexture = Get(uncompressedRenderTextureFormat, from.width, from.height);
			PropertySheet propertySheet = m_PropertySheets.Get(m_Resources.shaders.texture2dLerp);
			propertySheet.properties.SetVector(ShaderIDs.TargetColor, new Vector4(to.r, to.g, to.b, to.a));
			propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
			m_Command.BlitFullscreenTriangle(from, renderTexture, propertySheet, 1);
			return renderTexture;
		}

		internal void Clear()
		{
			foreach (RenderTexture active in m_Actives)
			{
				RuntimeUtilities.Destroy(active);
			}
			foreach (RenderTexture item in m_Recycled)
			{
				RuntimeUtilities.Destroy(item);
			}
			m_Actives.Clear();
			m_Recycled.Clear();
		}
	}
}
