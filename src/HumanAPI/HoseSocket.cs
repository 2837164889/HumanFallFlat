using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class HoseSocket : MonoBehaviour
	{
		public float radius = 1f;

		public float power = 1f;

		public float springAlign = 10f;

		public float springSnap = 10f;

		private static List<HoseSocket> all = new List<HoseSocket>();

		[NonSerialized]
		public uint sceneId;

		private void OnEnable()
		{
			sceneId = GetComponent<NetIdentity>().sceneId;
			all.Add(this);
		}

		private void OnDisable()
		{
			all.Remove(this);
		}

		public static HoseSocket FindById(uint sceneId)
		{
			for (int i = 0; i < all.Count; i++)
			{
				if (all[i].sceneId == sceneId)
				{
					return all[i];
				}
			}
			return null;
		}

		public static HoseSocket Scan(Vector3 pos)
		{
			HoseSocket result = null;
			float num = 0f;
			for (int i = 0; i < all.Count; i++)
			{
				HoseSocket hoseSocket = all[i];
				float num2 = 1f - (pos - hoseSocket.transform.position).magnitude / hoseSocket.radius;
				if (!(num2 < 0f))
				{
					num2 = Mathf.Pow(num2, hoseSocket.power);
					if (num < num2)
					{
						num = num2;
						result = hoseSocket;
					}
				}
			}
			return result;
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(base.transform.position, radius);
		}

		public void Align(Rigidbody body, Vector3 pos, Vector3 dir)
		{
			float num = 1f - (pos - base.transform.position).magnitude / radius;
			if (!(num < 0f))
			{
				num = Mathf.Pow(num, power);
				body.AddForceAtPosition(((base.transform.position - pos).normalized * springSnap - body.GetPointVelocity(pos) * body.mass * 1f) * num, pos);
				HumanMotion2.AlignToVector(body, dir, base.transform.forward, springAlign);
			}
		}
	}
}
