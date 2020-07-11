using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessManager
	{
		private static PostProcessManager s_Instance;

		private const int k_MaxLayerCount = 32;

		private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

		private readonly List<PostProcessVolume> m_Volumes;

		private readonly Dictionary<int, bool> m_SortNeeded;

		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		private readonly List<Collider> m_TempColliders;

		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

		public static PostProcessManager instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new PostProcessManager();
				}
				return s_Instance;
			}
		}

		private PostProcessManager()
		{
			m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
			m_Volumes = new List<PostProcessVolume>();
			m_SortNeeded = new Dictionary<int, bool>();
			m_BaseSettings = new List<PostProcessEffectSettings>();
			m_TempColliders = new List<Collider>(5);
			settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			ReloadBaseTypes();
		}

		private void CleanBaseTypes()
		{
			settingsTypes.Clear();
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				RuntimeUtilities.Destroy(baseSetting);
			}
			m_BaseSettings.Clear();
		}

		private void ReloadBaseTypes()
		{
			CleanBaseTypes();
			IEnumerable<Type> enumerable = from t in RuntimeUtilities.GetAllAssemblyTypes()
				where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), inherit: false) && !t.IsAbstract
				select t;
			foreach (Type item in enumerable)
			{
				settingsTypes.Add(item, item.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(item);
				postProcessEffectSettings.SetAllOverridesTo(state: true, excludeEnabled: false);
				m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (!flag) ? volumeTrigger.position : Vector3.zero;
			List<PostProcessVolume> list = GrabVolumes(value);
			foreach (PostProcessVolume item in list)
			{
				if ((!skipDisabled || item.enabled) && !(item.profileRef == null) && (!skipZeroWeight || !(item.weight <= 0f)))
				{
					if (item.isGlobal)
					{
						results.Add(item);
					}
					else if (!flag)
					{
						List<Collider> tempColliders = m_TempColliders;
						item.GetComponents(tempColliders);
						if (tempColliders.Count != 0)
						{
							float num = float.PositiveInfinity;
							foreach (Collider item2 in tempColliders)
							{
								if (item2.enabled)
								{
									Vector3 a = item2.ClosestPoint(vector);
									float sqrMagnitude = ((a - vector) / 2f).sqrMagnitude;
									if (sqrMagnitude < num)
									{
										num = sqrMagnitude;
									}
								}
							}
							tempColliders.Clear();
							float num2 = item.blendDistance * item.blendDistance;
							if (num <= num2)
							{
								results.Add(item);
							}
						}
					}
				}
			}
		}

		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return GetHighestPriorityVolume(layer.volumeLayer);
		}

		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			if (m_SortedVolumes.TryGetValue(mask, out List<PostProcessVolume> value))
			{
				foreach (PostProcessVolume item in value)
				{
					if (item.priority > num)
					{
						num = item.priority;
						result = item;
					}
				}
				return result;
			}
			return result;
		}

		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Quick Volume";
			gameObject.layer = layer;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			GameObject gameObject2 = gameObject;
			PostProcessVolume postProcessVolume = gameObject2.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings effect in settings)
			{
				profile.AddSettings(effect);
			}
			return postProcessVolume;
		}

		internal void SetLayerDirty(int layer)
		{
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				int key = sortedVolume.Key;
				if ((key & (1 << layer)) != 0)
				{
					m_SortNeeded[key] = true;
				}
			}
		}

		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Unregister(volume, prevLayer);
			Register(volume, newLayer);
		}

		private void Register(PostProcessVolume volume, int layer)
		{
			m_Volumes.Add(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				int key = sortedVolume.Key;
				if ((key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Add(volume);
				}
			}
			SetLayerDirty(layer);
		}

		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Register(volume, layer);
		}

		private void Unregister(PostProcessVolume volume, int layer)
		{
			m_Volumes.Remove(volume);
			foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
			{
				int key = sortedVolume.Key;
				if ((key & (1 << layer)) != 0)
				{
					sortedVolume.Value.Remove(volume);
				}
			}
		}

		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Unregister(volume, layer);
		}

		private void ReplaceData(PostProcessLayer postProcessLayer)
		{
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				PostProcessEffectSettings settings = postProcessLayer.GetBundle(baseSetting.GetType()).settings;
				int count = baseSetting.parameters.Count;
				for (int i = 0; i < count; i++)
				{
					settings.parameters[i].SetValue(baseSetting.parameters[i]);
				}
			}
		}

		internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
		{
			ReplaceData(postProcessLayer);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = (!flag) ? volumeTrigger.position : Vector3.zero;
			List<PostProcessVolume> list = GrabVolumes(value);
			foreach (PostProcessVolume item in list)
			{
				if (item.enabled && !(item.profileRef == null) && !(item.weight <= 0f))
				{
					List<PostProcessEffectSettings> settings = item.profileRef.settings;
					if (item.isGlobal)
					{
						postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(item.weight));
					}
					else if (!flag)
					{
						List<Collider> tempColliders = m_TempColliders;
						item.GetComponents(tempColliders);
						if (tempColliders.Count != 0)
						{
							float num = float.PositiveInfinity;
							foreach (Collider item2 in tempColliders)
							{
								if (item2.enabled)
								{
									Vector3 a = item2.ClosestPoint(vector);
									float sqrMagnitude = ((a - vector) / 2f).sqrMagnitude;
									if (sqrMagnitude < num)
									{
										num = sqrMagnitude;
									}
								}
							}
							tempColliders.Clear();
							float num2 = item.blendDistance * item.blendDistance;
							if (!(num > num2))
							{
								float num3 = 1f;
								if (num2 > 0f)
								{
									num3 = 1f - num / num2;
								}
								postProcessLayer.OverrideSettings(settings, num3 * Mathf.Clamp01(item.weight));
							}
						}
					}
				}
			}
		}

		private List<PostProcessVolume> GrabVolumes(LayerMask mask)
		{
			if (!m_SortedVolumes.TryGetValue(mask, out List<PostProcessVolume> value))
			{
				value = new List<PostProcessVolume>();
				foreach (PostProcessVolume volume in m_Volumes)
				{
					if (((int)mask & (1 << volume.gameObject.layer)) != 0)
					{
						value.Add(volume);
						m_SortNeeded[mask] = true;
					}
				}
				m_SortedVolumes.Add(mask, value);
			}
			if (m_SortNeeded.TryGetValue(mask, out bool value2) && value2)
			{
				m_SortNeeded[mask] = false;
				SortByPriority(value);
			}
			return value;
		}

		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}

		private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
		{
			return true;
		}
	}
}
