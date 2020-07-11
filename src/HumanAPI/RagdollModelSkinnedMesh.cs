using Multiplayer;
using System;
using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("HumanRagdoll/Ragdoll Skinned Mesh", 10)]
	public class RagdollModelSkinnedMesh : MonoBehaviour
	{
		public Renderer originalRenderer;

		public Renderer reskinnedRenderer;

		private Mesh meshToDestroy;

		internal void Reskin2(Ragdoll ragdoll)
		{
			Matrix4x4[] bindposes = new Matrix4x4[RagdollTemplate.instance.bindposes.Length];
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			for (int i = 0; i < bindposes.Length; i++)
			{
				bindposes[i] = RagdollTemplate.instance.bindposes[i] * localToWorldMatrix;
			}
			Mesh mesh = null;
			SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
			if ((bool)skinnedMeshRenderer)
			{
				mesh = (meshToDestroy = new Mesh());
				skinnedMeshRenderer.BakeMesh(mesh);
				originalRenderer = skinnedMeshRenderer;
				reskinnedRenderer = skinnedMeshRenderer;
			}
			else
			{
				MeshFilter component = GetComponent<MeshFilter>();
				MeshRenderer component2 = component.GetComponent<MeshRenderer>();
				if (component == null || component2 == null)
				{
					Debug.LogError("Trying to resking gameobject without mesh renderer", this);
					return;
				}
				component2.enabled = false;
				originalRenderer = component2;
				mesh = (meshToDestroy = new Mesh());
				mesh.vertices = component.sharedMesh.vertices;
				mesh.triangles = component.sharedMesh.triangles;
				mesh.uv = component.sharedMesh.uv;
				mesh.uv2 = component.sharedMesh.uv2;
				mesh.normals = component.sharedMesh.normals;
				GameObject gameObject = new GameObject(base.name + "Skinned");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				skinnedMeshRenderer.sharedMaterials = component2.sharedMaterials;
				reskinnedRenderer = skinnedMeshRenderer;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Vector3[] vertices = mesh.vertices;
			Matrix4x4 localToWorldMatrix2 = base.transform.localToWorldMatrix;
			for (int j = 0; j < vertices.Length; j++)
			{
				vertices[j] = localToWorldMatrix2.MultiplyPoint3x4(vertices[j]);
			}
			BoneWeight[] array = new BoneWeight[vertices.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = RagdollTemplate.instance.Map(vertices[k]);
			}
			Transform[] bones = ragdoll.bones;
			if (App.state != AppSate.Customize)
			{
				Override(ragdoll, vertices, array, ref bones, ref bindposes, localToWorldMatrix);
			}
			mesh.boneWeights = array;
			skinnedMeshRenderer.sharedMesh = mesh;
			mesh.bindposes = bindposes;
			skinnedMeshRenderer.bones = bones;
			skinnedMeshRenderer.rootBone = bones[0];
			mesh.RecalculateBounds();
		}

		private void Override(Ragdoll ragdoll, Vector3[] targetPositions, BoneWeight[] targetWeights, ref Transform[] bones, ref Matrix4x4[] bindposes, Matrix4x4 targetMeshToWorld)
		{
			SkinOverrideVolume[] componentsInChildren = GetComponentsInChildren<SkinOverrideVolume>();
			if (componentsInChildren.Length == 0)
			{
				return;
			}
			int num = bindposes.Length;
			Array.Resize(ref bindposes, bindposes.Length + componentsInChildren.Length);
			Array.Resize(ref bones, bones.Length + componentsInChildren.Length);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				SkinOverrideVolume skinOverrideVolume = componentsInChildren[i];
				bones[num + i] = skinOverrideVolume.transform;
				bindposes[num + i] = skinOverrideVolume.GetBindPose(ragdoll.transform) * targetMeshToWorld;
			}
			float[] array = new float[componentsInChildren.Length];
			for (int j = 0; j < targetPositions.Length; j++)
			{
				float num2 = 0f;
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					SkinOverrideVolume skinOverrideVolume2 = componentsInChildren[k];
					array[k] = skinOverrideVolume2.GetWeight(targetPositions[j]);
					num2 += array[k];
				}
				if (num2 != 0f)
				{
					BoneWeight b = BoneWeightUtils.FindBestBones(array);
					b.boneIndex0 += num;
					b.boneIndex1 += num;
					b.boneIndex2 += num;
					b.boneIndex3 += num;
					targetWeights[j] = BoneWeightUtils.Lerp(targetWeights[j], b, num2);
				}
			}
		}

		private void OnDestroy()
		{
			if (meshToDestroy != null)
			{
				UnityEngine.Object.Destroy(meshToDestroy);
			}
		}

		public void Clip(RigClipVolume[] clipVolumes)
		{
			if (clipVolumes.Length == 0)
			{
				return;
			}
			Mesh sharedMesh = (reskinnedRenderer as SkinnedMeshRenderer).sharedMesh;
			Vector3[] vertices = sharedMesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = reskinnedRenderer.transform.TransformPoint(vertices[i]);
			}
			bool[] array = new bool[vertices.Length];
			for (int j = 0; j < clipVolumes.Length; j++)
			{
				clipVolumes[j].Clip(array, vertices);
			}
			int[] triangles = sharedMesh.triangles;
			for (int k = 0; k < triangles.Length; k += 3)
			{
				if (array[triangles[k]] && array[triangles[k + 1]] && array[triangles[k + 2]])
				{
					triangles[k + 1] = (triangles[k + 2] = triangles[k]);
				}
			}
			sharedMesh.triangles = triangles;
			(reskinnedRenderer as SkinnedMeshRenderer).sharedMesh = sharedMesh;
		}
	}
}
