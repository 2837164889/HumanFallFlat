using Multiplayer;
using UnityEngine;

namespace HumanAPI
{
	public class RespawningBodyOverride : MonoBehaviour
	{
		public NetBody netBody;

		public Checkpoint checkpoint;

		internal Matrix4x4 initialToNewLocationMatrtix;

		internal Quaternion initialToNewLocationRotation;

		private void Awake()
		{
			initialToNewLocationMatrtix = base.transform.localToWorldMatrix * netBody.transform.worldToLocalMatrix;
			initialToNewLocationRotation = base.transform.rotation * Quaternion.Inverse(netBody.transform.rotation);
			NetBody[] componentsInChildren = netBody.GetComponentsInChildren<NetBody>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].AddOverride(this);
			}
		}
	}
}
