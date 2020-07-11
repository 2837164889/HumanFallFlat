using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuTransition : MonoBehaviour
{
	public GameObject defaultElement;

	public GameObject lastFocusedElement;

	public bool selectLastFocused = true;

	private RectTransform rect;

	private CanvasGroup group;

	private float start;

	private float target;

	private float current;

	private float phase = 1f;

	private float speed = 1f;

	protected virtual void OnEnable()
	{
		rect = GetComponent<RectTransform>();
		group = GetComponent<CanvasGroup>();
	}

	public void Transition(float newTarget, float duration)
	{
		if (duration == 0f)
		{
			phase = 1f;
			current = newTarget;
			target = newTarget;
			Apply();
		}
		else
		{
			target = newTarget;
			start = current;
			phase = 0f;
			speed = 1f / duration;
		}
	}

	protected virtual void Update()
	{
		if (phase < 1f)
		{
			phase = Mathf.MoveTowards(phase, 1f, speed * Mathf.Min((!(speed > 0f)) ? 1f : 0.33333334f, Time.unscaledDeltaTime));
			current = Ease.easeInOutQuad(start, target, phase);
			Apply();
			if (phase == 1f && target == 0f)
			{
				MenuSystem.instance.MenuTransitionedIn(this);
			}
		}
		else if (target == 1f || target == -1f)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Apply()
	{
		if (group != null)
		{
			group.alpha = 1f - Mathf.Abs(current);
		}
		Vector3 localPosition = rect.localPosition;
		localPosition.z = current * 2000f;
		rect.localPosition = localPosition;
		rect.localRotation = Quaternion.Euler(0f, (0f - current) * 60f, 0f);
	}

	public virtual void OnLostFocus()
	{
	}

	public virtual void OnGotFocus()
	{
		EventSystem.current.SetSelectedGameObject(null);
		if (!selectLastFocused || lastFocusedElement == null || !lastFocusedElement.gameObject.activeInHierarchy)
		{
			lastFocusedElement = defaultElement;
		}
		EventSystem.current.SetSelectedGameObject(lastFocusedElement);
	}

	public virtual void OnBack()
	{
	}

	public virtual void ApplyMenuEffects()
	{
	}

	public void TransitionForward<T>(float fadeOutTime = 0.3f, float fadeInTime = 0.3f) where T : MenuTransition
	{
		MenuSystem.instance.TransitionForward<T>(this, fadeOutTime, fadeInTime);
	}

	public void TransitionBack<T>(float fadeOutTime = 0.3f, float fadeInTime = 0.3f) where T : MenuTransition
	{
		if (!(MenuSystem.instance.activeMenu is SelectPlayersMenu))
		{
			lastFocusedElement = null;
		}
		MenuSystem.instance.TransitionBack<T>(this, fadeOutTime, fadeInTime);
	}

	public void FadeOutForward()
	{
		MenuSystem.instance.FadeOutForward(this);
	}

	public void FadeOutBack()
	{
		MenuSystem.instance.FadeOutBack(this);
	}

	public void Link(Selectable above, Selectable below, bool makeExplicit = false)
	{
		Navigation navigation = above.navigation;
		navigation.selectOnDown = below;
		if (makeExplicit)
		{
			navigation.mode = Navigation.Mode.Explicit;
		}
		above.navigation = navigation;
		navigation = below.navigation;
		navigation.selectOnUp = above;
		if (makeExplicit)
		{
			navigation.mode = Navigation.Mode.Explicit;
		}
		below.navigation = navigation;
	}

	public virtual void OnTansitionedIn()
	{
	}
}
