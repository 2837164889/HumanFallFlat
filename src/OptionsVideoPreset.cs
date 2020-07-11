public struct OptionsVideoPreset
{
	public int preset;

	public int clouds;

	public int shadows;

	public int texture;

	public int ao;

	public int aa;

	public int vsync;

	public int hdr;

	public int bloom;

	public int dof;

	public int chroma;

	public int exposure;

	public static OptionsVideoPreset Create(int qualityLevel)
	{
		switch (qualityLevel)
		{
		case 1:
		{
			OptionsVideoPreset result5 = default(OptionsVideoPreset);
			result5.preset = 1;
			result5.clouds = 0;
			result5.shadows = 0;
			result5.texture = 0;
			result5.ao = 0;
			result5.aa = 0;
			result5.vsync = 0;
			result5.hdr = 0;
			result5.bloom = 0;
			result5.dof = 0;
			result5.chroma = 0;
			result5.exposure = 0;
			return result5;
		}
		case 2:
		{
			OptionsVideoPreset result4 = default(OptionsVideoPreset);
			result4.preset = 2;
			result4.clouds = 1;
			result4.shadows = 1;
			result4.texture = 1;
			result4.ao = 0;
			result4.aa = 0;
			result4.vsync = 0;
			result4.hdr = 0;
			result4.bloom = 0;
			result4.dof = 0;
			result4.chroma = 0;
			result4.exposure = 0;
			return result4;
		}
		case 3:
		{
			OptionsVideoPreset result3 = default(OptionsVideoPreset);
			result3.preset = 3;
			result3.clouds = 2;
			result3.shadows = 2;
			result3.texture = 2;
			result3.ao = 0;
			result3.aa = 0;
			result3.vsync = 1;
			result3.hdr = 1;
			result3.bloom = 0;
			result3.dof = 0;
			result3.chroma = 0;
			result3.exposure = 0;
			return result3;
		}
		case 4:
		{
			OptionsVideoPreset result2 = default(OptionsVideoPreset);
			result2.preset = 4;
			result2.clouds = 3;
			result2.shadows = 3;
			result2.texture = 2;
			result2.ao = 2;
			result2.aa = 2;
			result2.vsync = 1;
			result2.hdr = 1;
			result2.bloom = 1;
			result2.dof = 0;
			result2.chroma = 1;
			result2.exposure = 1;
			return result2;
		}
		case 5:
		{
			OptionsVideoPreset result = default(OptionsVideoPreset);
			result.preset = 5;
			result.clouds = 4;
			result.shadows = 4;
			result.texture = 2;
			result.ao = 1;
			result.aa = 3;
			result.vsync = 1;
			result.hdr = 1;
			result.bloom = 1;
			result.dof = 1;
			result.chroma = 1;
			result.exposure = 1;
			return result;
		}
		default:
			return default(OptionsVideoPreset);
		}
	}
}
