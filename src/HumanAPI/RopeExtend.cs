using UnityEngine;

namespace HumanAPI
{
	public class RopeExtend : Rope, IControllable, IReset
	{
		public float targetExtend = 0.5f;

		public float extend = 1.5f;

		public float compress = 0.5f;

		private float minBoneLen;

		private float maxBoneLen;

		private float originalTargetExtend;

		private ConfigurableJoint[] joints;

		private CapsuleCollider[] colliders;

		public override void OnEnable()
		{
			base.OnEnable();
			originalTargetExtend = targetExtend;
			minBoneLen = boneLen * compress;
			maxBoneLen = boneLen * extend;
			joints = new ConfigurableJoint[bones.Length + 1];
			colliders = new CapsuleCollider[bones.Length];
			for (int i = 0; i < bones.Length; i++)
			{
				joints[i] = bones[i].GetComponent<ConfigurableJoint>();
				colliders[i] = bones[i].GetComponent<CapsuleCollider>();
			}
			if (endBody != null)
			{
				ConfigurableJoint[] components = bones[bones.Length - 1].GetComponents<ConfigurableJoint>();
				joints[bones.Length] = components[1];
			}
		}

		private void FixedUpdate()
		{
			float num = Mathf.Lerp(minBoneLen, maxBoneLen, targetExtend);
			if (num == boneLen)
			{
				return;
			}
			boneLen = num;
			for (int i = 0; i < bones.Length; i++)
			{
				ConfigurableJoint configurableJoint = joints[i];
				CapsuleCollider capsuleCollider = colliders[i];
				capsuleCollider.height = boneLen + radius * 2f;
				if (i != bones.Length - 1)
				{
					configurableJoint.anchor = new Vector3(0f, 0f, (0f - boneLen) / 2f);
				}
				if (i != 0)
				{
					configurableJoint.connectedAnchor = new Vector3(0f, 0f, boneLen / 2f);
				}
			}
			if (endBody != null)
			{
				joints[bones.Length].anchor = new Vector3(0f, 0f, boneLen / 2f);
				endBody.WakeUp();
			}
		}

		public void SetControlValue(float v)
		{
			targetExtend = v;
		}

		public new void ResetState(int checkpoint, int subObjectives)
		{
			targetExtend = originalTargetExtend;
		}
	}
}
