using ProBuilder2.Common;
using UnityEngine;

namespace ProBuilder2.Examples
{
	public class HueCube : MonoBehaviour
	{
		private pb_Object pb;

		private void Start()
		{
			pb = pb_ShapeGenerator.CubeGenerator(Vector3.one);
			int num = pb.sharedIndices.Length;
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = HSVtoRGB((float)i / (float)num * 360f, 1f, 1f);
			}
			Color[] colors = pb.colors;
			for (int j = 0; j < pb.sharedIndices.Length; j++)
			{
				int[] array2 = pb.sharedIndices[j].array;
				foreach (int num2 in array2)
				{
					colors[num2] = array[j];
				}
			}
			pb.SetColors(colors);
			pb.Refresh();
		}

		private static Color HSVtoRGB(float h, float s, float v)
		{
			if (s == 0f)
			{
				return new Color(v, v, v, 1f);
			}
			h /= 60f;
			int num = (int)Mathf.Floor(h);
			float num2 = h - (float)num;
			float num3 = v * (1f - s);
			float num4 = v * (1f - s * num2);
			float num5 = v * (1f - s * (1f - num2));
			float r;
			float g;
			float b;
			switch (num)
			{
			case 0:
				r = v;
				g = num5;
				b = num3;
				break;
			case 1:
				r = num4;
				g = v;
				b = num3;
				break;
			case 2:
				r = num3;
				g = v;
				b = num5;
				break;
			case 3:
				r = num3;
				g = num4;
				b = v;
				break;
			case 4:
				r = num5;
				g = num3;
				b = v;
				break;
			default:
				r = v;
				g = num3;
				b = num4;
				break;
			}
			return new Color(r, g, b, 1f);
		}
	}
}
