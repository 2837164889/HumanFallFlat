using UnityEngine;

namespace HumanAPI
{
	public class GapJoint : JointBase
	{
		public JointBase jointA;

		public JointBase jointB;

		private bool initialized;

		public override void EnsureInitialized()
		{
			if (!initialized)
			{
				initialized = true;
				jointA.useLimits = (jointB.useLimits = useLimits);
				jointA.minValue = (jointB.minValue = minValue / 2f);
				jointA.maxValue = (jointB.maxValue = maxValue / 2f);
				jointA.maxSpeed = (jointB.maxSpeed = maxSpeed / 2f);
				jointA.maxAcceleration = (jointB.maxAcceleration = maxAcceleration / 2f);
				jointA.useSpring = (jointB.useSpring = useSpring);
				jointA.tensionDist = (jointB.tensionDist = tensionDist / 2f);
				jointA.maxForce = (jointB.maxForce = maxForce / 2f);
				jointA.EnsureInitialized();
				jointB.EnsureInitialized();
				_isKinematic = jointA.isKinematic;
				if (maxValue == 1.7f)
				{
					GameObject gameObject = new GameObject("JamDoorAchievement");
					gameObject.transform.SetParent(base.transform, worldPositionStays: false);
					gameObject.transform.localPosition = new Vector3(0f, 1f, -1f);
					gameObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
					gameObject.gameObject.AddComponent<JamDoorAchievement>().door = this;
					BoxCollider boxCollider = gameObject.gameObject.AddComponent<BoxCollider>();
					boxCollider.isTrigger = true;
					boxCollider.center = new Vector3(0.25f, 1f, 0f);
					boxCollider.size = new Vector3(1f, 2f, 2f);
					gameObject.layer = 15;
				}
			}
		}

		public override float GetTarget()
		{
			return jointA.GetTarget() + jointB.GetTarget();
		}

		public override float GetValue()
		{
			return jointA.GetValue() + jointB.GetValue();
		}

		public override void SetTarget(float value)
		{
			jointA.SetTarget(value / 2f);
			jointB.SetTarget(value / 2f);
		}

		public override void SetValue(float value)
		{
			jointA.SetValue(value / 2f);
			jointB.SetValue(value / 2f);
		}
	}
}
