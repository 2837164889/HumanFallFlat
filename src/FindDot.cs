using UnityEngine;
using UnityEngine.UI;

public class FindDot : MonoBehaviour
{
	public Image dot;

	private void Awake()
	{
		dot = GetComponent<Image>();
	}
}
