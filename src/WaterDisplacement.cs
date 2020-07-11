using UnityEngine;

public class WaterDisplacement : MonoBehaviour
{
	private float scaleTarget = 1f;

	[SerializeField]
	private Transform waterLevel;

	[SerializeField]
	private float displacementMultiplier = 1f;

	private void Start()
	{
		Vector3 localScale = base.transform.localScale;
		scaleTarget = localScale.y;
	}

	private void Update()
	{
		Vector3 localScale = base.transform.localScale;
		float y = Mathf.Lerp(localScale.y, scaleTarget, 2.5f * Time.deltaTime);
		Transform transform = base.transform;
		Vector3 localScale2 = base.transform.localScale;
		float x = localScale2.x;
		Vector3 localScale3 = base.transform.localScale;
		transform.localScale = new Vector3(x, y, localScale3.z);
		Transform transform2 = waterLevel;
		Vector3 position = waterLevel.position;
		float x2 = position.x;
		Vector3 position2 = base.transform.position;
		float y2 = position2.y;
		Vector3 localScale4 = base.transform.localScale;
		float y3 = y2 + localScale4.y;
		Vector3 position3 = waterLevel.position;
		transform2.position = new Vector3(x2, y3, position3.z);
	}

	private void OnTriggerEnter(Collider collision)
	{
		Debug.Log("Water Triggered");
		if ((bool)collision.gameObject.GetComponent<VolumeFinder>())
		{
			float volume = collision.gameObject.GetComponent<VolumeFinder>().volume;
			AddVolume(volume);
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		Debug.Log("Water Triggered");
		if ((bool)collision.gameObject.GetComponent<VolumeFinder>())
		{
			float volume = collision.gameObject.GetComponent<VolumeFinder>().volume;
			AddVolume(0f - volume);
		}
	}

	private void AddVolume(float volume2Add)
	{
		float num = displacementMultiplier * volume2Add;
		Vector3 localScale = base.transform.localScale;
		float num2 = num / localScale.x;
		Vector3 localScale2 = base.transform.localScale;
		float num3 = num2 / localScale2.z;
		Vector3 localScale3 = base.transform.localScale;
		scaleTarget = num3 + localScale3.y;
		Debug.Log("Water rose to " + scaleTarget);
	}
}
