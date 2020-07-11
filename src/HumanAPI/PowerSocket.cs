using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(NetIdentity))]
	public class PowerSocket : CircuitConnector
	{
		[Tooltip("Helps with attaching cables , shows snap range")]
		public float radius = 1f;

		[Tooltip("Power output by this game object")]
		public float power = 1f;

		[Tooltip("Alignment value used in the snap")]
		public float springAlign = 10f;

		[Tooltip("Spring value used in the Snap")]
		public float springSnap = 10f;

		private static List<PowerSocket> all = new List<PowerSocket>();

		[Tooltip("Use this in order to show the prints coming from the script")]
		public bool showDebug;

		[NonSerialized]
		public uint sceneId;

		private void OnEnable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Enable ");
			}
			sceneId = GetComponent<NetIdentity>().sceneId;
			all.Add(this);
		}

		private void OnDisable()
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Disable ");
			}
			all.Remove(this);
		}

		public static PowerSocket FindById(uint sceneId)
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

		public static PowerSocket Scan(Vector3 pos)
		{
			PowerSocket result = null;
			float num = 0f;
			for (int i = 0; i < all.Count; i++)
			{
				PowerSocket powerSocket = all[i];
				float num2 = 1f - (pos - powerSocket.transform.position).magnitude / powerSocket.radius;
				if (!(num2 < 0f))
				{
					num2 = Mathf.Pow(num2, powerSocket.power);
					if (num < num2)
					{
						num = num2;
						result = powerSocket;
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
			if (showDebug)
			{
				Debug.Log(base.name + " Align ");
			}
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
