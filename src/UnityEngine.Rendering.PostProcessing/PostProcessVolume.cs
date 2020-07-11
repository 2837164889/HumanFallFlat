using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	[ExecuteInEditMode]
	[AddComponentMenu("Rendering/Post-process Volume", 1001)]
	public sealed class PostProcessVolume : MonoBehaviour
	{
		public PostProcessProfile sharedProfile;

		[Tooltip("A global volume is applied to the whole scene.")]
		public bool isGlobal;

		[Min(0f)]
		[Tooltip("Outer distance to start blending from. A value of 0 means no blending and the volume overrides will be applied immediatly upon entry.")]
		public float blendDistance;

		[Range(0f, 1f)]
		[Tooltip("Total weight of this volume in the scene. 0 means it won't do anything, 1 means full effect.")]
		public float weight = 1f;

		[Tooltip("Volume priority in the stack. Higher number means higher priority. Negative values are supported.")]
		public float priority;

		private int m_PreviousLayer;

		private float m_PreviousPriority;

		private List<Collider> m_TempColliders;

		private PostProcessProfile m_InternalProfile;

		public PostProcessProfile profile
		{
			get
			{
				if (m_InternalProfile == null)
				{
					m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
					if (sharedProfile != null)
					{
						foreach (PostProcessEffectSettings setting in sharedProfile.settings)
						{
							PostProcessEffectSettings item = Object.Instantiate(setting);
							m_InternalProfile.settings.Add(item);
						}
					}
				}
				return m_InternalProfile;
			}
			set
			{
				m_InternalProfile = value;
			}
		}

		internal PostProcessProfile profileRef => (!(m_InternalProfile == null)) ? m_InternalProfile : sharedProfile;

		public bool HasInstantiatedProfile()
		{
			return m_InternalProfile != null;
		}

		private void OnEnable()
		{
			PostProcessManager.instance.Register(this);
			m_PreviousLayer = base.gameObject.layer;
			m_TempColliders = new List<Collider>();
		}

		private void OnDisable()
		{
			PostProcessManager.instance.Unregister(this);
		}

		private void Update()
		{
			int layer = base.gameObject.layer;
			if (layer != m_PreviousLayer)
			{
				PostProcessManager.instance.UpdateVolumeLayer(this, m_PreviousLayer, layer);
				m_PreviousLayer = layer;
			}
			if (priority != m_PreviousPriority)
			{
				PostProcessManager.instance.SetLayerDirty(layer);
				m_PreviousPriority = priority;
			}
		}

		private void OnDrawGizmos()
		{
			List<Collider> tempColliders = m_TempColliders;
			GetComponents(tempColliders);
			if (!isGlobal && tempColliders != null)
			{
				Vector3 localScale = base.transform.localScale;
				Vector3 a = new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z);
				Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, localScale);
				foreach (Collider item in tempColliders)
				{
					if (item.enabled)
					{
						Type type = item.GetType();
						if (type == typeof(BoxCollider))
						{
							BoxCollider boxCollider = (BoxCollider)item;
							Gizmos.DrawCube(boxCollider.center, boxCollider.size);
							Gizmos.DrawWireCube(boxCollider.center, boxCollider.size + a * blendDistance * 4f);
						}
						else if (type == typeof(SphereCollider))
						{
							SphereCollider sphereCollider = (SphereCollider)item;
							Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
							Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius + a.x * blendDistance * 2f);
						}
						else if (type == typeof(MeshCollider))
						{
							MeshCollider meshCollider = (MeshCollider)item;
							if (!meshCollider.convex)
							{
								meshCollider.convex = true;
							}
							Gizmos.DrawMesh(meshCollider.sharedMesh);
							Gizmos.DrawWireMesh(meshCollider.sharedMesh, Vector3.zero, Quaternion.identity, Vector3.one + a * blendDistance * 4f);
						}
					}
				}
				tempColliders.Clear();
			}
		}
	}
}
