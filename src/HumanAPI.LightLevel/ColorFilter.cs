using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class ColorFilter : LightFilter
	{
		public Color color;

		public override int priority => 0;

		public override void ApplyFilter(LightHitInfo info)
		{
			Color color = default(Color);
			Color color2 = info.source.color;
			color.r = Mathf.Min(color2.r, this.color.r);
			Color color3 = info.source.color;
			color.g = Mathf.Min(color3.g, this.color.g);
			Color color4 = info.source.color;
			color.b = Mathf.Min(color4.b, this.color.b);
			Color color5 = color;
			if (consume.debugLog)
			{
				Debug.Log("Color");
			}
			foreach (LightBase output in info.outputs)
			{
				output.color = color5;
			}
		}
	}
}
