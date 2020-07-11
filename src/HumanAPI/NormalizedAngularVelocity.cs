using UnityEngine;

namespace HumanAPI
{
	public class NormalizedAngularVelocity : Node
	{
		public NodeOutput value;

		[SerializeField]
		private float maxVelocity = 10f;

		private Rigidbody rb;

		private float normalizedVelocity;

		private float previousVelocity;

		protected override void OnEnable()
		{
			if (rb == null)
			{
				rb = GetComponent<Rigidbody>();
			}
			base.OnEnable();
		}

		private void FixedUpdate()
		{
			normalizedVelocity = Mathf.Clamp(rb.angularVelocity.magnitude / maxVelocity, 0f, 1f);
			if (!(Mathf.Abs(normalizedVelocity - previousVelocity) < 0.05f))
			{
				previousVelocity = normalizedVelocity;
				value.SetValue(normalizedVelocity);
			}
		}
	}
}
