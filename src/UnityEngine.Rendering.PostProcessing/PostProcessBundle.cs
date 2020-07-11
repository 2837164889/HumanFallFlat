using System;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessBundle
	{
		private PostProcessEffectRenderer m_Renderer;

		public PostProcessAttribute attribute
		{
			get;
			private set;
		}

		public PostProcessEffectSettings settings
		{
			get;
			private set;
		}

		internal PostProcessEffectRenderer renderer
		{
			get
			{
				if (m_Renderer == null)
				{
					Type renderer = attribute.renderer;
					m_Renderer = (PostProcessEffectRenderer)Activator.CreateInstance(renderer);
					m_Renderer.SetSettings(settings);
					m_Renderer.Init();
				}
				return m_Renderer;
			}
		}

		internal PostProcessBundle(PostProcessEffectSettings settings)
		{
			this.settings = settings;
			attribute = settings.GetType().GetAttribute<PostProcessAttribute>();
		}

		internal void Release()
		{
			if (m_Renderer != null)
			{
				m_Renderer.Release();
			}
			RuntimeUtilities.Destroy(settings);
		}

		internal void ResetHistory()
		{
			if (m_Renderer != null)
			{
				m_Renderer.ResetHistory();
			}
		}

		internal T CastSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)settings;
		}

		internal T CastRenderer<T>() where T : PostProcessEffectRenderer
		{
			return (T)renderer;
		}
	}
}
