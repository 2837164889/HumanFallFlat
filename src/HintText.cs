using HumanAPI;
using UnityEngine;

public class HintText : MonoBehaviour
{
	public Sound2 onSound;

	public Sound2 offSound;

	private RectTransform rect;

	private float transitionSpeed;

	private float transitionPhase;

	private bool inTransition;

	public bool isVisible;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (inTransition)
		{
			transitionPhase = Mathf.Clamp01(transitionPhase + Time.deltaTime * transitionSpeed);
			float x = Ease.easeInOutQuad(0f, 1f, transitionPhase);
			rect.localScale = new Vector3(x, 1f, 1f);
			if (transitionPhase == 0f)
			{
				inTransition = false;
				base.gameObject.SetActive(value: false);
			}
		}
	}

	internal void Show()
	{
		isVisible = true;
		base.gameObject.SetActive(value: true);
		rect.localScale = new Vector3(0f, 1f, 1f);
		inTransition = true;
		transitionSpeed = 2f;
		onSound.PlayOneShot();
	}

	internal void Hide()
	{
		isVisible = false;
		offSound.PlayOneShot();
		inTransition = true;
		transitionSpeed = -2f;
	}
}
