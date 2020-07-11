using UnityEngine;

public class SumoStart : MonoBehaviour
{
	private void Start()
	{
		Object[] array = Resources.LoadAll(string.Empty);
		Object[] array2 = array;
		foreach (Object @object in array2)
		{
			Debug.Log("object: " + @object.name + " : " + @object.GetType().ToString());
		}
	}
}
