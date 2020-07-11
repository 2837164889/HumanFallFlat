using System;
using UnityEngine;

public class SignalSine : SignalBase
{
	public float min;

	public float max = 1f;

	public float period = 1f;

	private float phase;

	private void Update()
	{
		SetValue(Mathf.Lerp(min, max, Mathf.Sin(phase) / 2f + 0.5f));
		phase += Time.deltaTime / period * (float)Math.PI * 2f;
	}
}
