using UnityEngine;

namespace HumanAPI
{
	public class JointParamsOnGrab : MonoBehaviour, IGrabbable
	{
		public float damper;

		public float spring;

		public JointBase joint;

		private float originalDamper;

		private float originalSpring;

		private void OnEnable()
		{
			if (joint == null)
			{
				joint = GetComponent<JointBase>();
			}
			originalDamper = joint.damper;
			originalSpring = joint.spring;
		}

		public void OnGrab()
		{
			joint.damper = damper;
			joint.spring = spring;
			joint.useSpring = false;
		}

		public void OnRelease()
		{
			joint.damper = originalDamper;
			joint.spring = originalSpring;
			joint.useSpring = true;
			joint.target = joint.GetValue();
		}
	}
}
