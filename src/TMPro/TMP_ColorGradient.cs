using System;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TMP_ColorGradient : ScriptableObject
	{
		public Color topLeft;

		public Color topRight;

		public Color bottomLeft;

		public Color bottomRight;

		private static Color k_defaultColor = Color.white;

		public TMP_ColorGradient()
		{
			topLeft = k_defaultColor;
			topRight = k_defaultColor;
			bottomLeft = k_defaultColor;
			bottomRight = k_defaultColor;
		}

		public TMP_ColorGradient(Color color)
		{
			topLeft = color;
			topRight = color;
			bottomLeft = color;
			bottomRight = color;
		}

		public TMP_ColorGradient(Color color0, Color color1, Color color2, Color color3)
		{
			topLeft = color0;
			topRight = color1;
			bottomLeft = color2;
			bottomRight = color3;
		}
	}
}
