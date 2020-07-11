using UnityEngine;

namespace HumanAPI
{
	public class JointSensor : Node
	{
		[Tooltip("Raw value coming from the joint ")]
		public NodeOutput value;

		[Tooltip("Normalised Value coming from the joint ")]
		public NodeOutput valueNormalized;

		[Tooltip("Should we be using the normalised variable ")]
		public bool signedOutput;

		private JointBase joint;

		private void Awake()
		{
			joint = GetComponent<JointBase>();
		}

		private void FixedUpdate()
		{
			float num = joint.GetValue();
			value.SetValue(num);
			if (joint.minValue != joint.maxValue)
			{
				float num2 = Mathf.InverseLerp(joint.minValue, joint.maxValue, num);
				if (signedOutput)
				{
					num2 = num2 * 2f - 1f;
				}
				valueNormalized.SetValue(num2);
			}
		}
	}
}
