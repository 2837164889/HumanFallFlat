using UnityEngine;

public class AmbienceMix : MonoBehaviour
{
	public AmbienceSource[] sources;

	public float[] volumes;

	public void TransitionTo(float duration)
	{
		for (int i = 0; i < sources.Length; i++)
		{
			AmbienceManager.instance.RegisterSource(sources[i]);
			sources[i].FadeVolume(volumes[i], duration);
		}
	}
}
