using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class RagdollCustomization : MonoBehaviour
	{
		public Ragdoll ragdoll;

		public RagdollModel main;

		public RagdollModel head;

		public RagdollModel upper;

		public RagdollModel lower;

		public RagdollPresetMetadata preset;

		internal List<RigClipVolume> cachedClipVolumes = new List<RigClipVolume>();

		public bool allowHead => (main == null || main.allowHead) && (upper == null || upper.allowHead) && (lower == null || lower.allowHead);

		public bool allowUpper => (main == null || main.allowUpperBody) && (lower == null || lower.allowUpperBody) && (head == null || head.allowUpperBody);

		public bool allowLower => (main == null || main.allowLowerBody) && (upper == null || upper.allowLowerBody) && (head == null || head.allowLowerBody);

		private void OnEnable()
		{
			ragdoll = GetComponent<Ragdoll>();
		}

		private RagdollModel BindToSkin(WorkshopItemMetadata wsMeta)
		{
			if (wsMeta == null)
			{
				return null;
			}
			RagdollModelMetadata ragdollModelMetadata = wsMeta as RagdollModelMetadata;
			if (ragdollModelMetadata != null)
			{
				RagdollModel component;
				if (ragdollModelMetadata.modelPrefab == null)
				{
					AssetBundle assetBundle = WorkshopRepository.LoadBundle(ragdollModelMetadata);
					string[] allAssetNames = assetBundle.GetAllAssetNames();
					GameObject original = assetBundle.LoadAsset(ragdollModelMetadata.model) as GameObject;
					component = UnityEngine.Object.Instantiate(original).GetComponent<RagdollModel>();
					assetBundle.Unload(unloadAllLoadedObjects: false);
				}
				else
				{
					component = UnityEngine.Object.Instantiate(ragdollModelMetadata.modelPrefab.gameObject).GetComponent<RagdollModel>();
				}
				component.meta = ragdollModelMetadata;
				component.BindToRagdoll(ragdoll);
				component.texture.LoadFromPreset(preset);
				return component;
			}
			return null;
		}

		internal List<RigClipVolume> GetClipVolumes(bool forceClip)
		{
			List<RigClipVolume> list = new List<RigClipVolume>();
			if (main != null && (main.clipInCustomize || forceClip))
			{
				list.AddRange(main.GetComponentsInChildren<RigClipVolume>());
			}
			if (head != null && (head.clipInCustomize || forceClip || (main != null && main.clipInCustomize)))
			{
				list.AddRange(head.GetComponentsInChildren<RigClipVolume>());
			}
			if (upper != null && (upper.clipInCustomize || forceClip || (main != null && main.clipInCustomize)))
			{
				list.AddRange(upper.GetComponentsInChildren<RigClipVolume>());
			}
			if (lower != null && (lower.clipInCustomize || forceClip || (main != null && main.clipInCustomize)))
			{
				list.AddRange(lower.GetComponentsInChildren<RigClipVolume>());
			}
			return list;
		}

		public void ClearOutCachedClipVolumes()
		{
			cachedClipVolumes.Clear();
		}

		internal void ApplyPreset(RagdollPresetMetadata preset, bool forceRebuild = false)
		{
			if (!forceRebuild && this.preset != null && preset != null && this.preset.folder != preset.folder)
			{
				forceRebuild = true;
			}
			this.preset = preset;
			bool flag = false;
			if (!forceRebuild)
			{
				string a = (!(main != null) || main.meta == null || main.meta.folder == null) ? string.Empty : main.meta.folder;
				string b = (preset == null || preset.main == null || preset.main.modelPath == null) ? string.Empty : preset.main.modelPath;
				flag = (a != b);
			}
			bool flag2 = RebindMain(preset, forceRebuild);
			bool flag3 = RebindHead(preset, forceRebuild);
			bool flag4 = RebindUpper(preset, forceRebuild);
			bool flag5 = RebindLower(preset, forceRebuild);
			List<RigClipVolume> clipVolumes = GetClipVolumes(forceRebuild);
			if (!forceRebuild)
			{
				bool flag6 = flag || cachedClipVolumes.Count != clipVolumes.Count;
				if (!flag6)
				{
					for (int i = 0; i < cachedClipVolumes.Count; i++)
					{
						flag6 |= (cachedClipVolumes[i] != clipVolumes[i]);
					}
				}
				if (flag6)
				{
					forceRebuild = true;
					if (!flag2)
					{
						flag2 = RebindMain(preset, forceRebuild);
					}
				}
			}
			if (forceRebuild && main != null)
			{
				RagdollModelSkinnedMesh[] componentsInChildren = main.GetComponentsInChildren<RagdollModelSkinnedMesh>();
				RigClipVolume[] clipVolumes2 = clipVolumes.ToArray();
				cachedClipVolumes = clipVolumes;
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].Clip(clipVolumes2);
				}
			}
			else if (main == null)
			{
				cachedClipVolumes.Clear();
			}
		}

		private bool RebindMain(RagdollPresetMetadata preset, bool forceRebuild)
		{
			if (main != null && (forceRebuild || preset == null || preset.main == null || main.meta.folder != preset.main.modelPath))
			{
				main.Unbind(ragdoll);
				UnityEngine.Object.DestroyImmediate(main.gameObject);
				main = null;
			}
			if (main == null && preset != null && preset.main != null)
			{
				RagdollModelMetadata item = WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelFull).GetItem(preset.main.modelPath);
				if (item != null)
				{
					main = BindToSkin(item);
					return true;
				}
			}
			return false;
		}

		private bool RebindHead(RagdollPresetMetadata preset, bool forceRebuild)
		{
			if (head != null && (forceRebuild || !allowHead || preset == null || preset.head == null || preset.head.modelPath != head.meta.folder))
			{
				head.Unbind(ragdoll);
				UnityEngine.Object.DestroyImmediate(head.gameObject);
				head = null;
			}
			if (head == null && allowHead && preset != null && preset.head != null)
			{
				RagdollModelMetadata item = WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelHead).GetItem(preset.head.modelPath);
				if (item != null)
				{
					head = BindToSkin(item);
					return true;
				}
			}
			return false;
		}

		private bool RebindUpper(RagdollPresetMetadata preset, bool forceRebuild)
		{
			if (upper != null && (forceRebuild || !allowUpper || preset == null || preset.upperBody == null || preset.upperBody.modelPath != upper.meta.folder))
			{
				upper.Unbind(ragdoll);
				UnityEngine.Object.DestroyImmediate(upper.gameObject);
				upper = null;
			}
			if (upper == null && allowUpper && preset != null && preset.upperBody != null)
			{
				RagdollModelMetadata item = WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelUpperBody).GetItem(preset.upperBody.modelPath);
				if (item != null)
				{
					upper = BindToSkin(item);
					return true;
				}
			}
			return false;
		}

		private bool RebindLower(RagdollPresetMetadata preset, bool forceRebuild)
		{
			if (lower != null && (forceRebuild || !allowLower || preset == null || preset.lowerBody == null || preset.lowerBody.modelPath != lower.meta.folder))
			{
				lower.Unbind(ragdoll);
				UnityEngine.Object.DestroyImmediate(lower.gameObject);
				lower = null;
			}
			if (lower == null && allowLower && preset != null && preset.lowerBody != null)
			{
				RagdollModelMetadata item = WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelLowerBody).GetItem(preset.lowerBody.modelPath);
				if (item != null)
				{
					lower = BindToSkin(item);
					return true;
				}
			}
			return false;
		}

		public void RebindColors(bool bake, bool compress = false)
		{
			if (main != null)
			{
				RebindColors(main, bake, compress);
			}
			if (head != null)
			{
				RebindColors(head, bake, compress);
			}
			if (upper != null)
			{
				RebindColors(upper, bake, compress);
			}
			if (lower != null)
			{
				RebindColors(lower, bake, compress);
			}
		}

		public RagdollModel GetModel(WorkshopItemType part)
		{
			switch (part)
			{
			case WorkshopItemType.ModelFull:
				return main;
			case WorkshopItemType.ModelHead:
				return head;
			case WorkshopItemType.ModelUpperBody:
				return upper;
			case WorkshopItemType.ModelLowerBody:
				return lower;
			default:
				throw new InvalidOperationException();
			}
		}

		public void RebindColors(RagdollModel model, bool bake, bool compress)
		{
			model.texture.ApplyPresetColors(preset, bake, compress);
		}
	}
}
