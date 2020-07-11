using Multiplayer;
using UnityEngine;

public class CreditsFadeOutAndEnd : MonoBehaviour
{
	public delegate void AboutToQuitDelegate();

	private enum State
	{
		Off,
		Begin,
		Fading,
		WaitThenPause,
		ShowPauseEndlessly
	}

	private State state;

	private float timer;

	private const float FadeTime = 3f;

	private const float Leeway = 1f;

	private const float StallTime = 10f;

	private const float QuitTime = 3f;

	private AboutToQuitDelegate callback;

	private bool ShowingProgress;

	public static bool FadeInProgress
	{
		get;
		protected set;
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		Cancel();
	}

	public void OnDestroy()
	{
		Cancel();
	}

	public void Update()
	{
		if (state == State.Off)
		{
			base.enabled = false;
			return;
		}
		if (!IsFadeAllowed())
		{
			Cancel();
			return;
		}
		switch (state)
		{
		case State.Off:
			break;
		case State.Begin:
			MenuCameraEffects.SuperMasterFade(1f, 3f);
			FadeInProgress = true;
			timer = 3f + ((!NetGame.isLocal) ? 1f : 0.1f);
			state = State.Fading;
			break;
		case State.Fading:
			timer -= Time.deltaTime;
			if (!(timer < 0f))
			{
				break;
			}
			if (NetGame.isClient)
			{
				state = State.WaitThenPause;
				timer = 10f;
				ShowingProgress = true;
				SubtitleManager.instance.SetProgress(string.Empty);
				break;
			}
			state = State.WaitThenPause;
			ShowingProgress = false;
			timer = 3f;
			if (callback != null)
			{
				callback();
			}
			Game.instance.GameOver();
			break;
		case State.WaitThenPause:
			timer -= Time.deltaTime;
			if (timer < 0f)
			{
				if (ShowingProgress && SubtitleManager.instance.CheckProgressText(string.Empty))
				{
					SubtitleManager.instance.ClearProgress();
				}
				state = State.ShowPauseEndlessly;
			}
			break;
		case State.ShowPauseEndlessly:
			if (MenuSystem.instance.state == MenuSystemState.Inactive)
			{
				timer -= Time.deltaTime;
				MenuSystem.instance.ShowPauseMenu();
			}
			else
			{
				timer = 1f;
			}
			break;
		}
	}

	public static bool IsFadeAllowed()
	{
		AppSate appSate = App.state;
		if (appSate == AppSate.PlayLevel || appSate == AppSate.ServerPlayLevel || appSate == AppSate.ClientPlayLevel)
		{
			return true;
		}
		return false;
	}

	public static bool Trigger(GameObject go, AboutToQuitDelegate cb)
	{
		if (go == null)
		{
			return false;
		}
		CreditsFadeOutAndEnd creditsFadeOutAndEnd = go.GetComponent<CreditsFadeOutAndEnd>();
		if (creditsFadeOutAndEnd == null)
		{
			creditsFadeOutAndEnd = go.AddComponent<CreditsFadeOutAndEnd>();
		}
		return creditsFadeOutAndEnd.Trigger(cb);
	}

	public bool Trigger(AboutToQuitDelegate cb)
	{
		if (state != 0 || !IsFadeAllowed() || FadeInProgress)
		{
			return false;
		}
		callback = cb;
		state = State.Begin;
		base.enabled = true;
		Update();
		return true;
	}

	public void Cancel()
	{
		if (state != 0)
		{
			MenuCameraEffects.SuperMasterFade(0f, 1f);
			if (state == State.WaitThenPause && ShowingProgress && SubtitleManager.instance.CheckProgressText(string.Empty))
			{
				SubtitleManager.instance.ClearProgress();
			}
			state = State.Off;
			FadeInProgress = false;
			callback = null;
			base.enabled = false;
		}
	}
}
