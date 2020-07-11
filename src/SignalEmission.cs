using UnityEngine;

public class SignalEmission : SignalTweenBase
{
	public Color emissionColor = Color.white;

	public float emission = 1f;

	public override void OnValueChanged(float value)
	{
		base.OnValueChanged(value);
		Color color = Color.LerpUnclamped(Color.black, emissionColor * emission, value);
		GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
		DynamicGI.SetEmissive(GetComponent<MeshRenderer>(), color);
	}
}
