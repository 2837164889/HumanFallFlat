using UnityEngine;

public class AngleSnapping : MonoBehaviour, IGrabbable
{
	public float strength = 1f;

	private bool isGrabbed;

	private void Update()
	{
		if (isGrabbed)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			float num = Mathf.Abs(eulerAngles.y % 90f);
			if ((num > 65f && num < 89f) || (num < 30f && num > 1f))
			{
				eulerAngles.y = Mathf.Round(eulerAngles.y / 90f) * 90f;
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(eulerAngles), Time.deltaTime * strength);
		}
	}

	public void OnGrab()
	{
		isGrabbed = true;
	}

	public void OnRelease()
	{
		isGrabbed = false;
	}
}
