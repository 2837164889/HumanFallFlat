using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class MirrorReflection : LightFilter
	{
		public enum MirrorType
		{
			Reflect,
			Forward,
			Transparent
		}

		public enum ReflectFrom
		{
			Center,
			ContactPoint,
			ClosestPoint
		}

		public bool ignoreBackfacing = true;

		public MirrorType type;

		public ReflectFrom reflectFrom = ReflectFrom.ContactPoint;

		private Dictionary<LightBase, LightBase> reflectedLights = new Dictionary<LightBase, LightBase>();

		[SerializeField]
		private Vector3 _direction = Vector3.forward;

		public Vector3 Direction => base.transform.TransformDirection(_direction);

		public override int priority => 1;

		public override void ApplyFilter(LightHitInfo info)
		{
			Color color = info.source.color;
			float num = Vector3.Dot((info.contactPoint - info.source.transform.position).normalized, Direction.normalized);
			if (num <= 0f || !ignoreBackfacing)
			{
				CalculateReflection(info);
			}
			else
			{
				RemoveLight(info);
			}
		}

		private void CalculateReflection(LightHitInfo info)
		{
			Vector3 position = GetPosition(info.contactPoint);
			Vector3 direction = GetDirection(info);
			if (consume.debugLog)
			{
				Debug.Log("reflect");
			}
			LightBase lightBase;
			if (reflectedLights.ContainsKey(info.source))
			{
				lightBase = reflectedLights[info.source];
				if (!lightBase.isActiveAndEnabled)
				{
					RemoveLight(info);
					return;
				}
				lightBase.transform.position = position;
				lightBase.Direction = direction;
				lightBase.UpdateLight(consume);
			}
			else
			{
				if (info.source.bouncesLeft == 0)
				{
					return;
				}
				lightBase = CreateLight(position, direction);
				Collider collider = GetComponent<Collider>();
				if (collider == null)
				{
					collider = GetComponentInParent<Collider>();
				}
				lightBase.SetSize(collider.bounds);
				lightBase.bouncesLeft = info.source.bouncesLeft - 1;
				reflectedLights.Add(info.source, lightBase);
				info.outputs.Add(lightBase);
				consume.ignoreLights.Add(lightBase);
			}
			lightBase.color = info.source.color;
		}

		protected virtual LightBase CreateLight(Vector3 pos, Vector3 dir)
		{
			return LightPool.Create<LightBeam>(pos, dir, base.transform);
		}

		protected virtual Vector3 GetPosition(Vector3 contactPoint)
		{
			switch (reflectFrom)
			{
			case ReflectFrom.ContactPoint:
				return contactPoint;
			case ReflectFrom.ClosestPoint:
				return GetComponent<Collider>().ClosestPoint(contactPoint);
			case ReflectFrom.Center:
				return base.transform.position;
			default:
				return base.transform.position;
			}
		}

		protected virtual Vector3 GetDirection(LightHitInfo info)
		{
			switch (type)
			{
			case MirrorType.Forward:
				return Direction;
			case MirrorType.Reflect:
				return Vector3.Reflect(info.source.Direction, Direction);
			case MirrorType.Transparent:
				return info.source.Direction;
			default:
				return info.source.Direction;
			}
		}

		private void RemoveLight(LightHitInfo info)
		{
			if (reflectedLights.ContainsKey(info.source))
			{
				LightBeam item = (LightBeam)reflectedLights[info.source];
				info.outputs.Remove(item);
				RemoveLight(info.source);
			}
		}

		private void RemoveLight(LightBase source)
		{
			LightBeam lightBeam = (LightBeam)reflectedLights[source];
			consume.ignoreLights.Remove(lightBeam);
			lightBeam.DisableLight();
			reflectedLights.Remove(source);
		}

		protected override void OnLightExit(LightBase source)
		{
			if (reflectedLights.ContainsKey(source))
			{
				RemoveLight(source);
			}
		}
	}
}
