using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class FoldableSolarPanel : LightConsume
	{
		public Transform left;

		public Transform right;

		private float dotThreshold;

		protected override void CheckOutput()
		{
			float num = 0f;
			if (Vector3.Dot(left.forward, base.transform.forward) < dotThreshold)
			{
				num += 0.5f;
			}
			if (Vector3.Dot(right.forward, base.transform.forward) < dotThreshold)
			{
				num += 0.5f;
			}
			Color litColor = base.LitColor;
			float num2 = (litColor.r + litColor.g + litColor.b) / 3f;
			num2 *= num;
			if (num2 != Intensity.value)
			{
				Intensity.SetValue(num2);
			}
		}
	}
}
