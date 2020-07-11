using UnityEngine;

public class Curve_DisableIfNotXB1 : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}
}
