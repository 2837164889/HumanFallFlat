using UnityEngine;

namespace HumanAPI
{
	public class SignalTranslation : Node
	{
		public NodeOutput value;

		public Rigidbody body;

		private Vector3 initialPosition;

		public float distance = 1f;

		public bool clampOutput = true;

		protected override void OnEnable()
		{
			base.OnEnable();
			if (body == null)
			{
				body = GetComponent<Rigidbody>();
			}
			if (body != null)
			{
				initialPosition = body.transform.position;
			}
			base.OnEnable();
		}

		private void FixedUpdate()
		{
			if (body != null)
			{
				float magnitude = (body.transform.position - initialPosition).magnitude;
				float num = magnitude / distance;
				if (clampOutput)
				{
					num = Mathf.Clamp(num, 0f, 1f);
				}
				value.SetValue(num);
			}
		}
	}
}
