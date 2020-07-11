using InControl;
using UnityEngine;

namespace MultiplayerBasicExample
{
	public class Player : MonoBehaviour
	{
		private Renderer cachedRenderer;

		public InputDevice Device
		{
			get;
			set;
		}

		private void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		private void Update()
		{
			if (Device == null)
			{
				cachedRenderer.material.color = new Color(1f, 1f, 1f, 0.2f);
				return;
			}
			cachedRenderer.material.color = GetColorFromInput();
			base.transform.Rotate(Vector3.down, 500f * Time.deltaTime * Device.Direction.X, Space.World);
			base.transform.Rotate(Vector3.right, 500f * Time.deltaTime * Device.Direction.Y, Space.World);
		}

		private Color GetColorFromInput()
		{
			if ((bool)Device.Action1)
			{
				return Color.green;
			}
			if ((bool)Device.Action2)
			{
				return Color.red;
			}
			if ((bool)Device.Action3)
			{
				return Color.blue;
			}
			if ((bool)Device.Action4)
			{
				return Color.yellow;
			}
			return Color.white;
		}
	}
}
