using UnityEngine;

namespace HumanAPI
{
	public class NormalizedLinearVelocity : Node
	{
		public NodeOutput value;

		[SerializeField]
		private float maxVelocity = 10f;

		[SerializeField]
		private bool needsCollision = true;

		private Rigidbody rb;

		private float normalizedVelocity;

		private float previousVelocity;

		private bool isColliding;

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
			normalizedVelocity = Mathf.Clamp(rb.velocity.magnitude / maxVelocity, 0f, 1f);
			if (!(Mathf.Abs(normalizedVelocity - previousVelocity) < 0.01f))
			{
				previousVelocity = normalizedVelocity;
				if (needsCollision && !isColliding)
				{
					value.SetValue(0f);
				}
				else
				{
					value.SetValue(normalizedVelocity);
				}
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.collider.gameObject.layer != 8 && collision.collider.gameObject.layer != 9)
			{
				isColliding = true;
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if (collision.collider.gameObject.layer != 8 && collision.collider.gameObject.layer != 9)
			{
				isColliding = true;
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			if (collision.collider.gameObject.layer != 8 && collision.collider.gameObject.layer != 9)
			{
				isColliding = false;
			}
		}
	}
}
