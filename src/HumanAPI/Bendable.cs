using UnityEngine;

namespace HumanAPI
{
	public class Bendable : Rope, IReset
	{
		public float bendMultiplier = 1E-05f;

		public float treshold = 500f;

		private bool meshDirty = true;

		public bool isBent;

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebugBendable;

		public override void OnEnable()
		{
			if (showDebugBendable)
			{
				Debug.Log(base.name + " Enabled ");
			}
			base.OnEnable();
			for (int i = 0; i < bones.Length; i++)
			{
				bones[i].GetComponent<Rigidbody>().isKinematic = true;
				BendableSegment bendableSegment = bones[i].gameObject.AddComponent<BendableSegment>();
				bendableSegment.bandable = this;
				bendableSegment.index = i;
				bendableSegment.treshold = treshold;
			}
		}

		public void ReportBend(int index, Vector3 maxImpactVector)
		{
			if (showDebugBendable)
			{
				Debug.Log(base.name + " Report Bend ");
			}
			isDirty = true;
			for (int i = 0; i < bones.Length; i++)
			{
				float num = Mathf.Abs((float)bones.Length - (float)i * 2f) / (float)bones.Length;
				float num2 = 1f * (float)Mathf.Abs(index - i) / (float)bones.Length;
				num = 2f * num * num * num - 3f * num * num + 1f;
				num2 = 2f * num2 * num2 * num2 - 3f * num2 * num2 + 1f;
				if (!(num2 <= 0f) && !(num <= 0f))
				{
					Rigidbody component = bones[i].GetComponent<Rigidbody>();
					Vector3 vector = maxImpactVector * bendMultiplier * num * num2;
					vector = Vector3.ClampMagnitude(vector, radius / 4f);
					Vector3 a = component.position + vector;
					Vector3 a2 = component.position = originalPositions[i] + Vector3.ClampMagnitude(a - originalPositions[i], num * 0.5f);
					if ((a2 - originalPositions[i]).magnitude > 0.1f)
					{
						isBent = true;
					}
				}
			}
			SyncRotation();
		}

		private void SyncRotation()
		{
			if (showDebugBendable)
			{
				Debug.Log(base.name + " Sync Rotation ");
			}
			for (int i = 1; i < bones.Length - 1; i++)
			{
				Vector3 forward = bones[i + 1].position - bones[i - 1].position;
				Quaternion rotation = Quaternion.LookRotation(forward, bones[i - 1].up);
				Rigidbody component = bones[i].GetComponent<Rigidbody>();
				component.rotation = rotation;
			}
		}

		private void BendBone(int index, Vector3 maxImpactVector)
		{
			if (showDebugBendable)
			{
				Debug.Log(base.name + " Bend Bone ");
			}
			if (index >= 0 && index < bones.Length)
			{
				float num = 1f - Mathf.Abs((float)bones.Length - (float)index * 2f) / (float)bones.Length;
				num *= num;
				Rigidbody component = bones[index].GetComponent<Rigidbody>();
				component.MovePosition(component.position + maxImpactVector * bendMultiplier * num);
			}
		}

		public override void CheckDirty()
		{
			base.CheckDirty();
			if (meshDirty)
			{
				isDirty = true;
				meshDirty = false;
			}
		}

		public override void ResetState(int checkpoint, int subObjectives)
		{
			if (showDebugBendable)
			{
				Debug.Log(base.name + " Reset State ");
			}
			base.ResetState(checkpoint, subObjectives);
			isBent = false;
			isDirty = true;
			meshDirty = true;
		}
	}
}
