using Multiplayer;
using System.Collections;
using UnityEngine;

public class Hint : MonoBehaviour
{
	public float delay;

	private HintRemote remote;

	private static Hint activeHint;

	private Coroutine timerCoroutine;

	internal bool wasActivated;

	public bool disabled;

	private void Awake()
	{
		remote = GetComponentInChildren<HintRemote>();
		if (!NetGame.isClient)
		{
			remote.GetComponent<NetBody>().SetVisible(visible: false);
		}
	}

	public void TriggerHint()
	{
		if (!wasActivated && !NetGame.isClient)
		{
			if (activeHint != null)
			{
				activeHint.StopHint();
			}
			activeHint = this;
			timerCoroutine = StartCoroutine(HintCoroutine());
		}
	}

	private IEnumerator HintCoroutine()
	{
		wasActivated = true;
		float timer = delay;
		while (timer > 0f)
		{
			if (Game.instance.state == GameState.PlayingLevel)
			{
				timer -= Time.deltaTime;
			}
			else if (Game.instance.state != GameState.Paused)
			{
				yield break;
			}
			yield return null;
		}
		if (StillValid())
		{
			remote.GetComponent<NetBody>().SetVisible(visible: true);
			remote.GetComponent<NetBody>().Respawn();
			timerCoroutine = null;
		}
	}

	public void StopHint()
	{
		wasActivated = true;
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
		}
		timerCoroutine = null;
	}

	protected virtual bool StillValid()
	{
		return true;
	}
}
