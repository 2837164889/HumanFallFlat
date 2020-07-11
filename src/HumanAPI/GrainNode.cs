using UnityEngine;

namespace HumanAPI
{
	public class GrainNode : Node
	{
		[Tooltip("The value we should pass onto the grain script")]
		public NodeInput intensity;

		private Grain grain;

		private void Awake()
		{
			grain = GetComponent<Grain>();
		}

		public override void Process()
		{
			base.Process();
			grain.intensity = Mathf.Abs(intensity.value);
		}
	}
}
