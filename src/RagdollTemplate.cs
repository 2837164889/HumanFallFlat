using System;
using UnityEngine;

public class RagdollTemplate : MonoBehaviour, IDependency
{
	public static RagdollTemplate instance;

	public Ragdoll ragdoll;

	public RigVolume[] rigVolumes;

	[NonSerialized]
	public Matrix4x4[] bindposes;

	private float[] weights;

	public void Initialize()
	{
		instance = this;
		Transform[] bones = ragdoll.bones;
		weights = new float[bones.Length];
		for (int i = 0; i < rigVolumes.Length; i++)
		{
			rigVolumes[i].Build(bones);
		}
		bindposes = new Matrix4x4[bones.Length];
		for (int j = 0; j < bindposes.Length; j++)
		{
			bindposes[j] = bones[j].worldToLocalMatrix * ragdoll.transform.localToWorldMatrix;
		}
		Dependencies.OnInitialized(this);
	}

	public void PaintWeights(Vector3 pos, float[] weights)
	{
		float num = 1f;
		for (int num2 = rigVolumes.Length - 1; num2 >= 0; num2--)
		{
			num *= 1f - rigVolumes[num2].PaintWeights(pos, num, weights, mirror: false);
			if (rigVolumes[num2].mirrorBoneIndex >= 0)
			{
				num *= 1f - rigVolumes[num2].PaintWeights(pos, num, weights, mirror: true);
			}
		}
	}

	public BoneWeight Map(Vector3 pos)
	{
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = 0f;
		}
		PaintWeights(pos, weights);
		return BoneWeightUtils.FindBestBones(weights);
	}
}
