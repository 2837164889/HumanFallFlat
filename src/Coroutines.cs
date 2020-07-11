using System.Collections;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
	private static Coroutines instance;

	private static void EnsureInstance()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("Coroutines");
			instance = gameObject.AddComponent<Coroutines>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public static Coroutine StartGlobalCoroutine(IEnumerator enumerator)
	{
		EnsureInstance();
		return instance.StartCoroutine(enumerator);
	}

	public static void StopGlobalCoroutine(Coroutine coroutine)
	{
		if (instance != null)
		{
			instance.StopCoroutine(coroutine);
		}
	}
}
