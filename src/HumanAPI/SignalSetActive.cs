using UnityEngine;

namespace HumanAPI
{
	public class SignalSetActive : Node
	{
		[Tooltip("If not set, will affect this object")]
		public GameObject target;

		public NodeInput input;

		public bool invert;

		public bool applyOnStart;

		private void Start()
		{
			if (applyOnStart)
			{
				Process();
			}
		}

		public override void Process()
		{
			base.Process();
			bool flag = Mathf.Abs(input.value) >= 0.5f;
			if (invert)
			{
				flag = !flag;
			}
			if (target != null)
			{
				target.SetActive(flag);
			}
			else
			{
				base.gameObject.SetActive(flag);
			}
		}
	}
}
