using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedRope : MonoBehaviour
{
	public Transform start;

	public Transform end;

	public bool fixStart;

	public bool fixEnd;

	public int meshSegments = 20;

	public int rigidSegments = 10;

	public float segmentMass = 20f;

	public int segmentsAround = 6;

	public float radius = 0.1f;

	public float lengthMultiplier = 1f;

	public PhysicMaterial ropeMaterial;

	private void OnEnable()
	{
		float num = (start.position - end.position).magnitude * lengthMultiplier;
		int num2 = meshSegments + 1;
		float num3 = num / (float)meshSegments;
		float num4 = num / (float)rigidSegments;
		Vector3[] array = new Vector3[num2 * segmentsAround];
		BoneWeight[] array2 = new BoneWeight[num2 * segmentsAround];
		int[] array3 = new int[meshSegments * segmentsAround * 6];
		int num5 = 0;
		for (int i = 0; i < num2; i++)
		{
			Vector3 a = new Vector3(0f, 0f, (float)i * num3);
			BoneWeight boneWeight = CalculateBoneWeights(i);
			for (int j = 0; j < segmentsAround; j++)
			{
				Vector3 b = new Vector2(radius, 0f).Rotate((float)Math.PI * 2f * (float)j / (float)segmentsAround);
				Vector3 vector = a + b;
				array2[num5] = boneWeight;
				array[num5++] = vector;
			}
		}
		num5 = 0;
		for (int k = 0; k < num2 - 1; k++)
		{
			for (int l = 0; l < segmentsAround; l++)
			{
				int num6 = k * segmentsAround;
				int num7 = num6 + segmentsAround;
				int num8 = (l + 1) % segmentsAround;
				int num9 = num6 + l;
				int num10 = num6 + num8;
				int num11 = num7 + l;
				int num12 = num7 + num8;
				array3[num5++] = num9;
				array3[num5++] = num10;
				array3[num5++] = num11;
				array3[num5++] = num11;
				array3[num5++] = num10;
				array3[num5++] = num12;
			}
		}
		Mesh mesh = new Mesh();
		mesh.name = "rope " + base.name;
		mesh.vertices = array;
		mesh.triangles = array3;
		mesh.boneWeights = array2;
		mesh.RecalculateNormals();
		Matrix4x4[] array4 = new Matrix4x4[rigidSegments];
		Transform[] array5 = new Transform[rigidSegments];
		for (int m = 0; m < rigidSegments; m++)
		{
			Vector3 position = new Vector3(0f, 0f, num4 / 2f + (float)m * num4);
			GameObject gameObject = base.gameObject;
			if (m > 0)
			{
				gameObject = new GameObject("bone" + m);
				gameObject.transform.SetParent(base.transform, worldPositionStays: true);
			}
			gameObject.transform.position = position;
			array5[m] = gameObject.transform;
			array4[m] = gameObject.transform.worldToLocalMatrix;
			gameObject.tag = "Target";
			gameObject.layer = 10;
			Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = segmentMass;
			CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
			capsuleCollider.direction = 2;
			capsuleCollider.radius = radius;
			capsuleCollider.height = num4;
			capsuleCollider.sharedMaterial = ropeMaterial;
			if (m != 0)
			{
				ConfigurableJoint configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
				configurableJoint.connectedBody = array5[m - 1].GetComponent<Rigidbody>();
				ConfigurableJointMotion configurableJointMotion2 = configurableJoint.zMotion = ConfigurableJointMotion.Locked;
				configurableJointMotion2 = (configurableJoint.xMotion = (configurableJoint.yMotion = configurableJointMotion2));
				configurableJointMotion2 = (configurableJoint.angularZMotion = ConfigurableJointMotion.Limited);
				configurableJointMotion2 = (configurableJoint.angularXMotion = (configurableJoint.angularYMotion = configurableJointMotion2));
				configurableJoint.angularXLimitSpring = new SoftJointLimitSpring
				{
					spring = 100f,
					damper = 10f
				};
				configurableJoint.angularYZLimitSpring = new SoftJointLimitSpring
				{
					spring = 100f,
					damper = 10f
				};
				configurableJoint.lowAngularXLimit = new SoftJointLimit
				{
					limit = -20f
				};
				configurableJoint.highAngularXLimit = new SoftJointLimit
				{
					limit = 20f
				};
				configurableJoint.angularYLimit = new SoftJointLimit
				{
					limit = 20f
				};
				configurableJoint.angularZLimit = new SoftJointLimit
				{
					limit = 20f
				};
				configurableJoint.axis = new Vector3(0f, 0f, 1f);
				configurableJoint.secondaryAxis = new Vector3(1f, 0f, 0f);
				configurableJoint.anchor = new Vector3(0f, 0f, (0f - num4) / 2f);
				configurableJoint.connectedAnchor = new Vector3(0f, 0f, num4 / 2f);
				configurableJoint.autoConfigureConnectedAnchor = false;
			}
		}
		mesh.bindposes = array4;
		SkinnedMeshRenderer component = GetComponent<SkinnedMeshRenderer>();
		component.bones = array5;
		component.sharedMesh = mesh;
		base.transform.position = Vector3.up;
		component.rootBone = array5[0];
		mesh.RecalculateBounds();
		component.localBounds = new Bounds(Vector3.zero, Vector3.one * num);
	}

	public BoneWeight CalculateBoneWeights(int ring)
	{
		float num = 1f * (float)ring / (float)meshSegments;
		int num2 = Mathf.FloorToInt(num * (float)rigidSegments - 0.5f);
		float num3 = num * (float)rigidSegments - 0.5f - (float)num2;
		float num4 = num3 * num3;
		float num5 = num4 * num3;
		float num6 = 2f * num5 - 3f * num4 + 1f;
		float num7 = num5 - 2f * num4 + num3;
		float num8 = -2f * num5 + 3f * num4;
		float num9 = num5 - num4;
		float num10 = (0f - num7) / 2f;
		float num11 = num6 - num9 / 2f;
		float num12 = num7 / 2f + num8;
		float num13 = num9 / 2f;
		int item = Mathf.Clamp(num2 - 1, 0, rigidSegments - 1);
		int item2 = Mathf.Clamp(num2, 0, rigidSegments - 1);
		int item3 = Mathf.Clamp(num2 + 1, 0, rigidSegments - 1);
		int item4 = Mathf.Clamp(num2 + 2, 0, rigidSegments - 1);
		num10 = (num13 = 0f);
		num11 = 1f - num3;
		num12 = 1f - num11;
		List<int> list = new List<int>();
		List<float> list2 = new List<float>();
		list.Add(item);
		list2.Add(num10);
		if (num11 > list2[0])
		{
			list.Insert(0, item2);
			list2.Insert(0, num11);
		}
		else
		{
			list.Add(item2);
			list2.Add(num11);
		}
		if (num12 > list2[0])
		{
			list.Insert(0, item3);
			list2.Insert(0, num12);
		}
		else if (num12 > list2[1])
		{
			list.Insert(1, item3);
			list2.Insert(1, num12);
		}
		else
		{
			list.Add(item3);
			list2.Add(num12);
		}
		if (num13 > list2[0])
		{
			list.Insert(0, item4);
			list2.Insert(0, num13);
		}
		else if (num13 > list2[1])
		{
			list.Insert(1, item4);
			list2.Insert(1, num13);
		}
		else if (num13 > list2[2])
		{
			list.Insert(2, item4);
			list2.Insert(2, num13);
		}
		else
		{
			list.Add(item4);
			list2.Add(num13);
		}
		BoneWeight result = default(BoneWeight);
		result.boneIndex0 = list[0];
		result.boneIndex1 = list[1];
		result.boneIndex2 = list[2];
		result.boneIndex3 = list[3];
		result.weight0 = list2[0];
		result.weight1 = list2[1];
		result.weight2 = list2[2];
		result.weight3 = list2[3];
		return result;
	}

	private void Update()
	{
	}
}
