using System;
using UnityEngine;

namespace InControl
{
	public abstract class SingletonMonoBehavior<T, P> : MonoBehaviour where T : MonoBehaviour where P : MonoBehaviour
	{
		private static T instance;

		private static bool hasInstance;

		private static object lockObject = new object();

		public static T Instance => GetInstance();

		private static void CreateInstance()
		{
			GameObject gameObject = null;
			if (typeof(P) == typeof(MonoBehaviour))
			{
				gameObject = new GameObject();
				gameObject.name = typeof(T).Name;
			}
			else
			{
				P exists = UnityEngine.Object.FindObjectOfType<P>();
				if (!(UnityEngine.Object)exists)
				{
					Debug.LogError("Could not find object with required component " + typeof(P).Name);
					return;
				}
				gameObject = exists.gameObject;
			}
			Debug.Log("Creating instance of singleton component " + typeof(T).Name);
			instance = gameObject.AddComponent<T>();
			hasInstance = true;
		}

		private static T GetInstance()
		{
			lock (lockObject)
			{
				if (hasInstance)
				{
					return instance;
				}
				Type typeFromHandle = typeof(T);
				T[] array = UnityEngine.Object.FindObjectsOfType<T>();
				if (array.Length > 0)
				{
					instance = array[0];
					hasInstance = true;
					if (array.Length > 1)
					{
						Debug.LogWarning("Multiple instances of singleton " + typeFromHandle + " found; destroying all but the first.");
						for (int i = 1; i < array.Length; i++)
						{
							UnityEngine.Object.DestroyImmediate(array[i].gameObject);
						}
					}
					return instance;
				}
				SingletonPrefabAttribute singletonPrefabAttribute = Attribute.GetCustomAttribute(typeFromHandle, typeof(SingletonPrefabAttribute)) as SingletonPrefabAttribute;
				if (singletonPrefabAttribute == null)
				{
					CreateInstance();
				}
				else
				{
					string name = singletonPrefabAttribute.Name;
					GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(name));
					if (gameObject == null)
					{
						Debug.LogError("Could not find prefab " + name + " for singleton of type " + typeFromHandle + ".");
						CreateInstance();
					}
					else
					{
						gameObject.name = name;
						instance = gameObject.GetComponent<T>();
						if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
						{
							Debug.LogWarning("There wasn't a component of type \"" + typeFromHandle + "\" inside prefab \"" + name + "\"; creating one now.");
							instance = gameObject.AddComponent<T>();
							hasInstance = true;
						}
					}
				}
				return instance;
			}
		}

		protected bool EnforceSingleton()
		{
			lock (lockObject)
			{
				if (hasInstance)
				{
					T[] array = UnityEngine.Object.FindObjectsOfType<T>();
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i].GetInstanceID() != instance.GetInstanceID())
						{
							UnityEngine.Object.DestroyImmediate(array[i].gameObject);
						}
					}
				}
			}
			int instanceID = GetInstanceID();
			T val = Instance;
			return instanceID == val.GetInstanceID();
		}

		protected bool EnforceSingletonComponent()
		{
			lock (lockObject)
			{
				if (hasInstance && GetInstanceID() != instance.GetInstanceID())
				{
					UnityEngine.Object.DestroyImmediate(this);
					return false;
				}
			}
			return true;
		}

		private void OnDestroy()
		{
			hasInstance = false;
		}
	}
}
