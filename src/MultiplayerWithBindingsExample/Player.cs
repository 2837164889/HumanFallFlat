using UnityEngine;

namespace MultiplayerWithBindingsExample
{
	public class Player : MonoBehaviour
	{
		private Renderer cachedRenderer;

		public PlayerActions Actions
		{
			get;
			set;
		}

		private void OnDisable()
		{
			if (Actions != null)
			{
				Actions.Destroy();
			}
		}

		private void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		private void Update()
		{
			if (Actions == null)
			{
				cachedRenderer.material.color = new Color(1f, 1f, 1f, 0.2f);
				return;
			}
			cachedRenderer.material.color = GetColorFromInput();
			base.transform.Rotate(Vector3.down, 500f * Time.deltaTime * Actions.Rotate.X, Space.World);
			base.transform.Rotate(Vector3.right, 500f * Time.deltaTime * Actions.Rotate.Y, Space.World);
		}

		private Color GetColorFromInput()
		{
			if ((bool)Actions.Green)
			{
				return Color.green;
			}
			if ((bool)Actions.Red)
			{
				return Color.red;
			}
			if ((bool)Actions.Blue)
			{
				return Color.blue;
			}
			if ((bool)Actions.Yellow)
			{
				return Color.yellow;
			}
			return Color.white;
		}
	}
}
