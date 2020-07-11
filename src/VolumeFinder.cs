using UnityEngine;

public class VolumeFinder : MonoBehaviour
{
	private enum VolumeType
	{
		Custom,
		Box,
		Sphere
	}

	[SerializeField]
	public float volume;

	[SerializeField]
	private VolumeType shapeType;

	private void Start()
	{
		if (shapeType != 0)
		{
			CalculateVolume();
		}
	}

	public void CalculateVolume()
	{
		if (shapeType == VolumeType.Sphere)
		{
			Vector3 localScale = base.transform.localScale;
			volume = 4.1887903f * Mathf.Pow(localScale.x * 0.5f, 3f);
		}
		if (shapeType == VolumeType.Box)
		{
			Vector3 localScale2 = base.transform.localScale;
			float x = localScale2.x;
			Vector3 localScale3 = base.transform.localScale;
			float num = x * localScale3.y;
			Vector3 localScale4 = base.transform.localScale;
			volume = num * localScale4.z;
		}
	}
}
