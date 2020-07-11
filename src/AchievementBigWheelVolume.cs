using UnityEngine;

public class AchievementBigWheelVolume : MonoBehaviour
{
	public AchievementBigWheel achievementComponent;

	private void OnTriggerEnter(Collider other)
	{
		Human component = other.GetComponent<Human>();
		if (component != null)
		{
			achievementComponent.TriggerVolumeEntered(component);
		}
	}
}
