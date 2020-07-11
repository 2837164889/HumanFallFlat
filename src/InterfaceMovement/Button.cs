using UnityEngine;

namespace InterfaceMovement
{
	public class Button : MonoBehaviour
	{
		private Renderer cachedRenderer;

		public Button up;

		public Button down;

		public Button left;

		public Button right;

		private void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		private void Update()
		{
			bool flag = base.transform.parent.GetComponent<ButtonManager>().focusedButton == this;
			Color color = cachedRenderer.material.color;
			color.a = Mathf.MoveTowards(color.a, (!flag) ? 0.5f : 1f, Time.deltaTime * 3f);
			cachedRenderer.material.color = color;
		}
	}
}
