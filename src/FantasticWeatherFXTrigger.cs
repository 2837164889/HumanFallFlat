using UnityEngine;

public class FantasticWeatherFXTrigger : MonoBehaviour
{
	[SerializeField]
	private float emit;

	[SerializeField]
	private float noise;

	[SerializeField]
	private Vector3 windDirection;

	[SerializeField]
	private float windStrength;

	public float getEmit()
	{
		return emit;
	}

	public float getNoise()
	{
		return noise;
	}

	public Vector3 getWindDirection()
	{
		return windDirection;
	}

	public float getWindStrength()
	{
		return windStrength;
	}
}
