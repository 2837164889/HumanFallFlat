using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightConsumeByColor : LightConsume
	{
		public NodeOutput r;

		public NodeOutput g;

		public NodeOutput b;

		public NodeOutput a;

		protected override void CheckOutput()
		{
			base.CheckOutput();
			Color litColor = base.LitColor;
			if (litColor.r != r.value)
			{
				r.SetValue(litColor.r);
			}
			if (litColor.g != g.value)
			{
				g.SetValue(litColor.g);
			}
			if (litColor.b != b.value)
			{
				b.SetValue(litColor.b);
			}
			if (litColor.a != a.value)
			{
				a.SetValue(litColor.a);
			}
		}
	}
}
