using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
	public class NetGraph : Graphic
	{
		private float[] values = new float[256];

		private int samplePos;

		private float range = 1f;

		public void PushValue(float value)
		{
			values[samplePos] = value;
			samplePos = (samplePos + 1) % values.Length;
			SetVerticesDirty();
		}

		public float GetMax()
		{
			float num = values[0];
			for (int i = 1; i < values.Length; i++)
			{
				if (num < values[i])
				{
					num = values[i];
				}
			}
			return num;
		}

		public void SetRange(float range)
		{
			this.range = range;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			Vector2 size = base.rectTransform.rect.size;
			Vector2 pivot = base.rectTransform.pivot;
			float x = (0f - pivot.x) * size.x;
			Vector2 pivot2 = base.rectTransform.pivot;
			Vector2 vector = new Vector2(x, (0f - pivot2.y) * size.y);
			UIVertex[] array = new UIVertex[4]
			{
				UIVertex.simpleVert,
				UIVertex.simpleVert,
				UIVertex.simpleVert,
				UIVertex.simpleVert
			};
			array[0].color = new Color32(0, 0, 0, 64);
			array[0].position = new Vector2(0f * size.x + vector.x, 0f * size.y + vector.y);
			array[1].color = new Color32(0, 0, 0, 64);
			array[1].position = new Vector2(0f * size.x + vector.x, 1f * size.y + vector.y);
			array[2].color = new Color32(0, 0, 0, 64);
			array[2].position = new Vector2(1f * size.x + vector.x, 1f * size.y + vector.y);
			array[3].color = new Color32(0, 0, 0, 64);
			array[3].position = new Vector2(1f * size.x + vector.x, 0f * size.y + vector.y);
			vh.AddUIVertexQuad(array);
			for (int i = 0; i < values.Length - 1; i++)
			{
				float num = values[(i + samplePos) % values.Length] / range;
				float num2 = values[(i + samplePos + 1) % values.Length] / range;
				float num3 = 1f * (float)i / (float)values.Length;
				float num4 = 1f * (float)(i + 1) / (float)values.Length;
				array[0].color = color;
				array[1].color = color;
				array[2].color = color;
				array[3].color = color;
				array[0].position = new Vector2(num3 * size.x + vector.x, 0f * size.y + vector.y);
				array[1].position = new Vector2(num3 * size.x + vector.x, num * size.y + vector.y);
				array[2].position = new Vector2(num4 * size.x + vector.x, num2 * size.y + vector.y);
				array[3].position = new Vector2(num4 * size.x + vector.x, 0f * size.y + vector.y);
				vh.AddUIVertexQuad(array);
			}
		}
	}
}
