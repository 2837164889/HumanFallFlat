namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class ChromaticAberrationRenderer : PostProcessEffectRenderer<ChromaticAberration>
	{
		private Texture2D m_InternalSpectralLut;

		public override void Render(PostProcessRenderContext context)
		{
			Texture texture = base.settings.spectralLut.value;
			if (texture == null)
			{
				if (m_InternalSpectralLut == null)
				{
					m_InternalSpectralLut = new Texture2D(3, 1, TextureFormat.RGB24, mipmap: false)
					{
						name = "Chromatic Aberration Spectrum Lookup",
						filterMode = FilterMode.Bilinear,
						wrapMode = TextureWrapMode.Clamp,
						anisoLevel = 0,
						hideFlags = HideFlags.DontSave
					};
					m_InternalSpectralLut.SetPixels(new Color[3]
					{
						new Color(1f, 0f, 0f),
						new Color(0f, 1f, 0f),
						new Color(0f, 0f, 1f)
					});
					m_InternalSpectralLut.Apply();
				}
				texture = m_InternalSpectralLut;
			}
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword((!base.settings.fastMode) ? "CHROMATIC_ABERRATION" : "CHROMATIC_ABERRATION_LOW");
			uberSheet.properties.SetFloat(ShaderIDs.ChromaticAberration_Amount, (float)base.settings.intensity * 0.05f);
			uberSheet.properties.SetTexture(ShaderIDs.ChromaticAberration_SpectralLut, texture);
		}

		public override void Release()
		{
			RuntimeUtilities.Destroy(m_InternalSpectralLut);
			m_InternalSpectralLut = null;
		}
	}
}
