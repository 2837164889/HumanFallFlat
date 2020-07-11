using UnityEngine;
using UnityEngine.UI;

public class NewLevelStar : MonoBehaviour
{
	public Image outlineGlow;

	public bool autoHide = true;

	private bool newLevels;

	private void OnEnable()
	{
		if (autoHide)
		{
			newLevels = !GameSave.HasSeenLatestLevel();
			if (!newLevels)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
		float num = Mathf.Sin(Time.time * 3f);
		outlineGlow.color = new Color(1f, 1f, 1f, (num + 1f) / 2f * 0.75f);
	}
}
