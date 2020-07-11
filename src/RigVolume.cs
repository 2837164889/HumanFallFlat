using UnityEngine;

public class RigVolume : MonoBehaviour
{
	public string boneName;

	public string mirrorBoneName;

	public int boneIndex = -1;

	public int mirrorBoneIndex = -1;

	public RigVolumeMesh inner;

	public RigVolumeMesh outer;

	public void Build(Transform[] bones)
	{
		boneIndex = -1;
		for (int i = 0; i < bones.Length; i++)
		{
			if (bones[i].name == boneName)
			{
				boneIndex = i;
				break;
			}
		}
		mirrorBoneIndex = -1;
		for (int j = 0; j < bones.Length; j++)
		{
			if (bones[j].name == mirrorBoneName)
			{
				mirrorBoneIndex = j;
				break;
			}
		}
		if (inner != null)
		{
			inner.Build(-1f);
		}
		if (outer != null)
		{
			outer.Build(1f);
		}
	}

	public float PaintWeights(Vector3 pos, float opacity, float[] weights, bool mirror)
	{
		if (mirror)
		{
			pos.x *= -1f;
		}
		int num = (!mirror) ? boneIndex : mirrorBoneIndex;
		float weight = GetWeight(pos);
		weights[num] = Mathf.Lerp(weights[num], 1f, weight * opacity);
		return weight;
	}

	private float GetWeight(Vector3 pos)
	{
		if (inner == null || outer == null)
		{
			return 1f;
		}
		float distOutside = inner.GetDistOutside(pos);
		float distInside = outer.GetDistInside(pos);
		if (distInside <= 0f)
		{
			return 0f;
		}
		if (distOutside <= 0f)
		{
			return 1f;
		}
		return distInside / (distOutside + distInside);
	}
}
