using UnityEngine;

public static class BoneWeightUtils
{
	public static BoneWeight FindBestBones(float[] weights)
	{
		BoneWeight result = default(BoneWeight);
		result.boneIndex0 = FindBestBone(weights);
		result.weight0 = weights[result.boneIndex0];
		weights[result.boneIndex0] = 0f;
		result.boneIndex1 = FindBestBone(weights);
		result.weight1 = weights[result.boneIndex1];
		weights[result.boneIndex1] = 0f;
		result.boneIndex2 = FindBestBone(weights);
		result.weight2 = weights[result.boneIndex2];
		weights[result.boneIndex2] = 0f;
		result.boneIndex3 = FindBestBone(weights);
		result.weight3 = weights[result.boneIndex3];
		weights[result.boneIndex3] = 0f;
		float num = result.weight0 + result.weight1 + result.weight2 + result.weight3;
		if (num != 0f)
		{
			result.weight0 /= num;
			result.weight1 /= num;
			result.weight2 /= num;
			result.weight3 /= num;
		}
		return result;
	}

	private static int FindBestBone(float[] weights)
	{
		float num = 0f;
		int result = 0;
		for (int i = 0; i < weights.Length; i++)
		{
			if (num < weights[i])
			{
				num = weights[i];
				result = i;
			}
		}
		return result;
	}

	public static BoneWeight Lerp(BoneWeight a, BoneWeight b, float mix)
	{
		if (mix <= 0f)
		{
			return a;
		}
		if (mix >= 1f)
		{
			return b;
		}
		a.weight0 *= 1f - mix;
		a.weight1 *= 1f - mix;
		a.weight2 *= 1f - mix;
		a.weight3 *= 1f - mix;
		b.weight0 *= mix;
		b.weight1 *= mix;
		b.weight2 *= mix;
		b.weight3 *= mix;
		BoneWeight result = default(BoneWeight);
		int num = 0;
		int num2 = 0;
		if (a.GetWeight(num) > b.GetWeight(num2))
		{
			result.weight0 = a.GetWeight(num);
			result.boneIndex0 = a.GetBoneIndex(num);
			num++;
		}
		else
		{
			result.weight0 = b.GetWeight(num2);
			result.boneIndex0 = b.GetBoneIndex(num2);
			num2++;
		}
		if (a.GetWeight(num) > b.GetWeight(num2))
		{
			result.weight1 = a.GetWeight(num);
			result.boneIndex1 = a.GetBoneIndex(num);
			num++;
		}
		else
		{
			result.weight1 = b.GetWeight(num2);
			result.boneIndex1 = b.GetBoneIndex(num2);
			num2++;
		}
		if (a.GetWeight(num) > b.GetWeight(num2))
		{
			result.weight2 = a.GetWeight(num);
			result.boneIndex2 = a.GetBoneIndex(num);
			num++;
		}
		else
		{
			result.weight2 = b.GetWeight(num2);
			result.boneIndex2 = b.GetBoneIndex(num2);
			num2++;
		}
		if (a.GetWeight(num) > b.GetWeight(num2))
		{
			result.weight3 = a.GetWeight(num);
			result.boneIndex3 = a.GetBoneIndex(num);
			num++;
		}
		else
		{
			result.weight3 = b.GetWeight(num2);
			result.boneIndex3 = b.GetBoneIndex(num2);
			num2++;
		}
		float num3 = result.weight0 + result.weight1 + result.weight2 + result.weight3;
		if (num3 != 0f)
		{
			result.weight0 /= num3;
			result.weight1 /= num3;
			result.weight2 /= num3;
			result.weight3 /= num3;
		}
		return result;
	}

	public static int GetBoneIndex(this BoneWeight w, int i)
	{
		switch (i)
		{
		case 0:
			return w.boneIndex0;
		case 1:
			return w.boneIndex1;
		case 2:
			return w.boneIndex2;
		default:
			return w.boneIndex3;
		}
	}

	public static float GetWeight(this BoneWeight w, int i)
	{
		switch (i)
		{
		case 0:
			return w.weight0;
		case 1:
			return w.weight1;
		case 2:
			return w.weight2;
		default:
			return w.weight3;
		}
	}
}
