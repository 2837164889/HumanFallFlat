using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI.LightLevel
{
	public class LightPool : MonoBehaviour
	{
		public Dictionary<Type, GameObject> prefabs;

		private Dictionary<Type, Stack<LightBase>> pool;

		private static LightPool _instance;

		private static LightPool Instance
		{
			get
			{
				if (_instance == null)
				{
					new GameObject("Light pool", typeof(LightPool));
				}
				return _instance;
			}
		}

		private void Awake()
		{
			if (_instance == null)
			{
				_instance = this;
			}
			else if (_instance != this)
			{
				UnityEngine.Object.Destroy(this);
			}
			pool = new Dictionary<Type, Stack<LightBase>>();
			prefabs = new Dictionary<Type, GameObject>();
			prefabs.Add(typeof(LightBeam), (GameObject)Resources.Load("LightBeam_ray"));
			prefabs.Add(typeof(LaserBeam), (GameObject)Resources.Load("LaserBeam"));
			prefabs.Add(typeof(LightBeamConvex), (GameObject)Resources.Load("LightBeam_Convex"));
		}

		public static void DestroyLight<T>(T light) where T : LightBase
		{
			light.Reset();
			light.transform.parent = Instance.transform;
			if (!Instance.pool[typeof(T)].Contains(light))
			{
				Instance.pool[typeof(T)].Push(light);
			}
		}

		public static T Create<T>(Vector3 origin, Vector3 dir, Transform parent = null) where T : LightBase
		{
			return Instance.Inner_Create<T>(new Ray(origin, dir), parent);
		}

		public static T Create<T>(Ray ray, Transform parent = null) where T : LightBase
		{
			return Instance.Inner_Create<T>(ray, parent);
		}

		private T Inner_Create<T>(Ray ray, Transform parent) where T : LightBase
		{
			Type typeFromHandle = typeof(T);
			if (!pool.ContainsKey(typeFromHandle))
			{
				pool[typeFromHandle] = new Stack<LightBase>();
			}
			T result;
			if (pool[typeFromHandle].Count > 0)
			{
				result = (T)pool[typeFromHandle].Pop();
				result.transform.position = ray.origin;
			}
			else
			{
				result = UnityEngine.Object.Instantiate(prefabs[typeFromHandle], ray.origin, Quaternion.identity).GetComponent<T>();
			}
			result.Direction = ray.direction;
			result.EnableLight();
			return result;
		}
	}
}
