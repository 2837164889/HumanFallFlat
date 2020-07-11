using UnityEngine;

public class DontDestroy : MonoBehaviour
{
	public void OnEnable()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
