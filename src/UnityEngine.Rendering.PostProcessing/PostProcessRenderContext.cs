using System.Collections.Generic;
using UnityEngine.XR;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessRenderContext
	{
		private Camera m_Camera;

		public Vector2 projOffset;

		internal PropertySheet uberSheet;

		internal Texture autoExposureTexture;

		internal LogHistogram logHistogram;

		internal Texture logLut;

		internal AutoExposure autoExposure;

		internal int bloomBufferNameID;

		private RenderTextureDescriptor m_sourceDescriptor;

		public Camera camera
		{
			get
			{
				return m_Camera;
			}
			set
			{
				m_Camera = value;
				if (m_Camera.stereoEnabled)
				{
					RenderTextureDescriptor eyeTextureDesc = XRSettings.eyeTextureDesc;
					width = eyeTextureDesc.width;
					height = eyeTextureDesc.height;
					m_sourceDescriptor = eyeTextureDesc;
					if (m_Camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
					{
						xrActiveEye = 1;
					}
					screenWidth = XRSettings.eyeTextureWidth;
					screenHeight = XRSettings.eyeTextureHeight;
					stereoActive = true;
				}
				else
				{
					width = m_Camera.pixelWidth;
					height = m_Camera.pixelHeight;
					m_sourceDescriptor.width = width;
					m_sourceDescriptor.height = height;
					screenWidth = width;
					screenHeight = height;
					stereoActive = false;
				}
			}
		}

		public CommandBuffer command
		{
			get;
			set;
		}

		public RenderTargetIdentifier source
		{
			get;
			set;
		}

		public RenderTargetIdentifier destination
		{
			get;
			set;
		}

		public RenderTextureFormat sourceFormat
		{
			get;
			set;
		}

		public bool flip
		{
			get;
			set;
		}

		public PostProcessResources resources
		{
			get;
			internal set;
		}

		public PropertySheetFactory propertySheets
		{
			get;
			internal set;
		}

		public Dictionary<string, object> userData
		{
			get;
			private set;
		}

		public PostProcessDebugLayer debugLayer
		{
			get;
			internal set;
		}

		public int width
		{
			get;
			private set;
		}

		public int height
		{
			get;
			private set;
		}

		public bool stereoActive
		{
			get;
			private set;
		}

		public int xrActiveEye
		{
			get;
			private set;
		}

		public int screenWidth
		{
			get;
			private set;
		}

		public int screenHeight
		{
			get;
			private set;
		}

		public bool isSceneView
		{
			get;
			internal set;
		}

		public PostProcessLayer.Antialiasing antialiasing
		{
			get;
			internal set;
		}

		public TemporalAntialiasing temporalAntialiasing
		{
			get;
			internal set;
		}

		public void Reset()
		{
			m_Camera = null;
			width = 0;
			height = 0;
			m_sourceDescriptor = new RenderTextureDescriptor(0, 0);
			stereoActive = false;
			xrActiveEye = 0;
			screenWidth = 0;
			screenHeight = 0;
			command = null;
			source = 0;
			destination = 0;
			sourceFormat = RenderTextureFormat.ARGB32;
			flip = false;
			resources = null;
			propertySheets = null;
			debugLayer = null;
			isSceneView = false;
			antialiasing = PostProcessLayer.Antialiasing.None;
			temporalAntialiasing = null;
			uberSheet = null;
			autoExposureTexture = null;
			logLut = null;
			autoExposure = null;
			bloomBufferNameID = -1;
			if (userData == null)
			{
				userData = new Dictionary<string, object>();
			}
			userData.Clear();
		}

		public bool IsTemporalAntialiasingActive()
		{
			return antialiasing == PostProcessLayer.Antialiasing.TemporalAntialiasing && !isSceneView && temporalAntialiasing.IsSupported();
		}

		public bool IsDebugOverlayEnabled(DebugOverlay overlay)
		{
			return debugLayer.debugOverlay == overlay;
		}

		public void PushDebugOverlay(CommandBuffer cmd, RenderTargetIdentifier source, PropertySheet sheet, int pass)
		{
			debugLayer.PushDebugOverlay(cmd, source, sheet, pass);
		}

		private RenderTextureDescriptor GetDescriptor(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default)
		{
			RenderTextureDescriptor result = new RenderTextureDescriptor(m_sourceDescriptor.width, m_sourceDescriptor.height, m_sourceDescriptor.colorFormat, depthBufferBits);
			result.dimension = m_sourceDescriptor.dimension;
			result.volumeDepth = m_sourceDescriptor.volumeDepth;
			result.vrUsage = m_sourceDescriptor.vrUsage;
			result.msaaSamples = m_sourceDescriptor.msaaSamples;
			result.memoryless = m_sourceDescriptor.memoryless;
			result.useMipMap = m_sourceDescriptor.useMipMap;
			result.autoGenerateMips = m_sourceDescriptor.autoGenerateMips;
			result.enableRandomWrite = m_sourceDescriptor.enableRandomWrite;
			result.shadowSamplingMode = m_sourceDescriptor.shadowSamplingMode;
			if (colorFormat != RenderTextureFormat.Default)
			{
				result.colorFormat = colorFormat;
			}
			result.sRGB = (readWrite != RenderTextureReadWrite.Linear);
			return result;
		}

		public void GetScreenSpaceTemporaryRT(CommandBuffer cmd, int nameID, int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filter = FilterMode.Bilinear, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			cmd.GetTemporaryRT(nameID, descriptor, filter);
		}

		public RenderTexture GetScreenSpaceTemporaryRT(int depthBufferBits = 0, RenderTextureFormat colorFormat = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int widthOverride = 0, int heightOverride = 0)
		{
			RenderTextureDescriptor descriptor = GetDescriptor(depthBufferBits, colorFormat, readWrite);
			if (widthOverride > 0)
			{
				descriptor.width = widthOverride;
			}
			if (heightOverride > 0)
			{
				descriptor.height = heightOverride;
			}
			return RenderTexture.GetTemporary(descriptor);
		}
	}
}
