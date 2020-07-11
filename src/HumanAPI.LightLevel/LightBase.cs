using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public abstract class LightBase : Node
	{
		private readonly Color DefaultColor = Color.white;

		private readonly float DefaultIntensity = 1f;

		private readonly int maxBounces = 5;

		public float intensity;

		public int bouncesLeft = 5;

		[SerializeField]
		private Vector3 direction = Vector3.forward;

		public Dictionary<Transform, Vector3> hitColliders = new Dictionary<Transform, Vector3>();

		private Vector3 lastPos;

		private Quaternion lastRot;

		protected List<LightConsume> consumesHit = new List<LightConsume>();

		public virtual Color color
		{
			get;
			set;
		}

		public virtual Vector3 Direction
		{
			get
			{
				return base.transform.TransformDirection(direction);
			}
			set
			{
				base.transform.forward = value;
			}
		}

		private void FixedUpdate()
		{
			if (lastPos != base.transform.position || lastRot != base.transform.rotation)
			{
				lastRot = base.transform.rotation;
				lastPos = base.transform.position;
				UpdateLight();
			}
			foreach (KeyValuePair<Transform, Vector3> hitCollider in hitColliders)
			{
				if (hitCollider.Key.transform.position != hitCollider.Value)
				{
					hitColliders[hitCollider.Key] = hitCollider.Key.position;
					UpdateLight();
					break;
				}
			}
		}

		protected void ClearConsumes()
		{
			List<LightConsume> list = new List<LightConsume>(consumesHit);
			if (consumesHit.Count > 0)
			{
				foreach (LightConsume item in list)
				{
					item.RemoveLightSource(this);
				}
			}
			consumesHit.Clear();
		}

		public void AddConsume(LightConsume c)
		{
			if (!consumesHit.Contains(c))
			{
				consumesHit.Add(c);
			}
		}

		public void RemoveConsume(LightConsume c)
		{
			if (consumesHit.Contains(c))
			{
				consumesHit.Remove(c);
			}
		}

		public virtual void Reset()
		{
			color = DefaultColor;
			intensity = DefaultIntensity;
			bouncesLeft = maxBounces;
			ClearConsumes();
		}

		public virtual void EnableLight()
		{
			base.gameObject.SetActive(value: true);
		}

		public virtual void DisableLight()
		{
			Reset();
			base.gameObject.SetActive(value: false);
			ReturnToPool();
		}

		protected abstract void ReturnToPool();

		public virtual void SetSize(Bounds b)
		{
		}

		public virtual void UpdateLight(LightConsume from)
		{
			if (consumesHit.Count > 0)
			{
				foreach (LightConsume item in consumesHit)
				{
					if (from != item)
					{
						item.Recalculate(this);
					}
				}
			}
		}

		public virtual void UpdateLight()
		{
			if (consumesHit.Count > 0)
			{
				foreach (LightConsume item in consumesHit)
				{
					item.Recalculate(this);
				}
			}
		}

		public virtual Vector3 ClosestPoint(Vector3 point)
		{
			return GetComponentInChildren<Collider>().ClosestPoint(point);
		}
	}
}
