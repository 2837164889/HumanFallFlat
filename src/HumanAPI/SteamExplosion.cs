using UnityEngine;

namespace HumanAPI
{
	public class SteamExplosion : Node, IReset
	{
		[Tooltip("Input: Source signal")]
		public NodeInput input;

		[Tooltip("output: Signal when boiler has exploded ")]
		public NodeOutput explodedSignal;

		[SerializeField]
		private bool exploded;

		[SerializeField]
		private Rigidbody rb;

		[SerializeField]
		private Vector3 force;

		[SerializeField]
		private float threshold;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		private void Start()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Start ");
			}
			rb = GetComponent<Rigidbody>();
		}

		public override void Process()
		{
			base.Process();
			if (input.value >= threshold && !exploded)
			{
				if (showDebug)
				{
					Debug.Log(base.name + " EXPLOSION ");
				}
				exploded = true;
				rb.isKinematic = false;
				rb.AddForce(force);
				explodedSignal.SetValue(1f);
			}
		}

		void IReset.ResetState(int checkpoint, int subObjectives)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Reset State ");
			}
			exploded = false;
			rb.isKinematic = true;
			explodedSignal.SetValue(0f);
		}
	}
}
