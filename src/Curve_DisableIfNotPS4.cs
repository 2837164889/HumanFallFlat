using UnityEngine;

public class Curve_DisableIfNotPS4 : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}
}
