using UnityEngine;

namespace HumanAPI
{
	[AddComponentMenu("HumanRagdoll/Ragdoll Model", 10)]
	public class RagdollModel : MonoBehaviour
	{
		public enum Rigging
		{
			AutoRig,
			BoneMap
		}

		public WorkshopItemType ragdollPart;

		public Rigging rigType;

		public bool replacesCharacter;

		public bool clipInCustomize;

		public bool allowHead = true;

		public bool allowUpperBody = true;

		public bool allowLowerBody = true;

		public Texture2D baseTexture;

		public Texture2D maskTexture;

		public bool mask1;

		public bool mask2;

		public bool mask3;

		public Color color1;

		public Color color2;

		public Color color3;

		public Transform partHead;

		public Transform partChest;

		public Transform partWaist;

		public Transform partHips;

		public Transform partLeftArm;

		public Transform partLeftForearm;

		public Transform partLeftHand;

		public Transform partLeftThigh;

		public Transform partLeftLeg;

		public Transform partLeftFoot;

		public Transform partRightArm;

		public Transform partRightForearm;

		public Transform partRightHand;

		public Transform partRightThigh;

		public Transform partRightLeg;

		public Transform partRightFoot;

		internal RagdollModelMetadata meta;

		public RagdollTexture texture;

		public RagdollPaddedModel _padded;

		private Renderer[] texturedRenderers;

		public RagdollPaddedModel padded
		{
			get
			{
				if (_padded == null)
				{
					_padded = base.gameObject.AddComponent<RagdollPaddedModel>();
					_padded.CreatePaddedMesh(texturedRenderers);
				}
				return _padded;
			}
		}

		protected void OnDestroy()
		{
			if (maskTexture != null)
			{
				TextureTracker.instance.RemoveMapping(this, maskTexture);
				maskTexture = null;
			}
		}

		public void BindToRagdoll(Ragdoll ragdoll)
		{
			base.transform.SetParent(ragdoll.transform);
			texturedRenderers = GetComponentsInChildren<Renderer>();
			if (rigType == Rigging.AutoRig && GetComponentInChildren<RagdollModelSkinnedMesh>() == null)
			{
				MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled)
					{
						componentsInChildren[i].gameObject.AddComponent<RagdollModelSkinnedMesh>();
					}
				}
				SkinnedMeshRenderer[] componentsInChildren2 = GetComponentsInChildren<SkinnedMeshRenderer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if (componentsInChildren2[j].enabled)
					{
						componentsInChildren2[j].gameObject.AddComponent<RagdollModelSkinnedMesh>();
					}
				}
			}
			RagdollModelSkinnedMesh[] componentsInChildren3 = GetComponentsInChildren<RagdollModelSkinnedMesh>();
			if (componentsInChildren3.Length > 0)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					componentsInChildren3[k].Reskin2(ragdoll);
				}
			}
			BindBone(ragdoll.partHead, partHead);
			BindBone(ragdoll.partChest, partChest);
			BindBone(ragdoll.partWaist, partWaist);
			BindBone(ragdoll.partHips, partHips);
			BindBone(ragdoll.partLeftArm, partLeftArm);
			BindBone(ragdoll.partLeftForearm, partLeftForearm);
			BindBone(ragdoll.partLeftHand, partLeftHand);
			BindBone(ragdoll.partLeftThigh, partLeftThigh);
			BindBone(ragdoll.partLeftLeg, partLeftLeg);
			BindBone(ragdoll.partLeftFoot, partLeftFoot);
			BindBone(ragdoll.partRightArm, partRightArm);
			BindBone(ragdoll.partRightForearm, partRightForearm);
			BindBone(ragdoll.partRightHand, partRightHand);
			BindBone(ragdoll.partRightThigh, partRightThigh);
			BindBone(ragdoll.partRightLeg, partRightLeg);
			BindBone(ragdoll.partRightFoot, partRightFoot);
			for (int l = 0; l < componentsInChildren3.Length; l++)
			{
				for (int m = 0; m < texturedRenderers.Length; m++)
				{
					if (texturedRenderers[m] == componentsInChildren3[l].originalRenderer)
					{
						texturedRenderers[m] = componentsInChildren3[l].reskinnedRenderer;
					}
				}
			}
			texture = base.gameObject.AddComponent<RagdollTexture>();
			texture.BindToModel(this);
		}

		private void BindBone(HumanSegment segment, Transform bone)
		{
			if (bone != null)
			{
				Matrix4x4 matrix4x = segment.bindPose * bone.localToWorldMatrix;
				bone.SetParent(segment.transform, worldPositionStays: false);
				bone.localPosition = matrix4x.GetColumn(3);
				bone.localRotation = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			}
		}

		public void Unbind(Ragdoll ragdoll)
		{
			UnbindBone(ragdoll.partHead, partHead);
			UnbindBone(ragdoll.partChest, partChest);
			UnbindBone(ragdoll.partWaist, partWaist);
			UnbindBone(ragdoll.partHips, partHips);
			UnbindBone(ragdoll.partLeftArm, partLeftArm);
			UnbindBone(ragdoll.partLeftForearm, partLeftForearm);
			UnbindBone(ragdoll.partLeftHand, partLeftHand);
			UnbindBone(ragdoll.partLeftThigh, partLeftThigh);
			UnbindBone(ragdoll.partLeftLeg, partLeftLeg);
			UnbindBone(ragdoll.partLeftFoot, partLeftFoot);
			UnbindBone(ragdoll.partRightArm, partRightArm);
			UnbindBone(ragdoll.partRightForearm, partRightForearm);
			UnbindBone(ragdoll.partRightHand, partRightHand);
			UnbindBone(ragdoll.partRightThigh, partRightThigh);
			UnbindBone(ragdoll.partRightLeg, partRightLeg);
			UnbindBone(ragdoll.partRightFoot, partRightFoot);
		}

		private void UnbindBone(HumanSegment segment, Transform bone)
		{
			if (bone != null)
			{
				bone.SetParent(base.transform, worldPositionStays: false);
			}
		}

		public void SetTexture(Texture texture)
		{
			for (int i = 0; i < texturedRenderers.Length; i++)
			{
				texturedRenderers[i].material.mainTexture = texture;
			}
		}

		public void ShowMask(bool show)
		{
			for (int i = 0; i < texturedRenderers.Length; i++)
			{
				if (!(texturedRenderers[i].material.shader != Shaders.instance.showMaskShader) || texturedRenderers[i].material.GetFloat("_Mode") != 3f)
				{
					texturedRenderers[i].material.shader = ((!show) ? Shaders.instance.opaqueHumanShader : Shaders.instance.showMaskShader);
					texturedRenderers[i].material.SetTexture("_MaskTex", maskTexture);
				}
			}
		}

		public void SetMask(int mask)
		{
			for (int i = 0; i < texturedRenderers.Length; i++)
			{
				texturedRenderers[i].material.SetFloat("_Mask1", ((mask & 1) == 1) ? 1 : 0);
				texturedRenderers[i].material.SetFloat("_Mask2", ((mask & 2) == 2) ? 1 : 0);
				texturedRenderers[i].material.SetFloat("_Mask3", ((mask & 4) == 4) ? 1 : 0);
			}
		}
	}
}
