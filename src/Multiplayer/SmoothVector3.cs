using UnityEngine;

namespace Multiplayer
{
	public class SmoothVector3
	{
		private Vector3[] buffer = new Vector3[10];

		private int smoothingStart;

		private int smoothingCount;

		private const float tolerance = 0.01f;

		public Vector3 Next(Vector3 target)
		{
			if (smoothingCount == buffer.Length)
			{
				smoothingStart = (smoothingStart + 1) % buffer.Length;
				smoothingCount--;
			}
			for (int num = smoothingCount - 1; num > 0; num--)
			{
				if ((target - buffer[(smoothingStart + num) % buffer.Length]).sqrMagnitude > 0.0001f)
				{
					smoothingStart = (smoothingStart + num + 1) % buffer.Length;
					smoothingCount -= num + 1;
					break;
				}
			}
			buffer[(smoothingStart + smoothingCount) % buffer.Length] = target;
			smoothingCount++;
			for (int i = 0; i < smoothingCount - 1; i++)
			{
				target = Vector3.Lerp(target, buffer[(smoothingStart + i) % buffer.Length], 1f / (float)(i + 1));
			}
			return target;
		}
	}
}
