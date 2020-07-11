using HumanAPI;
using System.Collections.Generic;
using UnityEngine;

public class Sail : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	public float area;

	public Vector3 wind;

	public Rigidbody boom;

	public Rigidbody boat;

	public Transform forcePoint;

	public Sound2 sailSound;

	private List<float> smoothFill = new List<float>();

	private List<float> smoothOpen = new List<float>();

	private Vector3 force;

	private float phaseOpen;

	private float phaseFill;

	private float lastFrameTime;

	private void Update()
	{
		float f = Mathf.InverseLerp(0f, 2000f, force.magnitude);
		f = Mathf.Sqrt(f);
		float num = Vector3.Dot(boom.transform.right, force.normalized);
		num = Mathf.Clamp(num * 100f, -1f, 1f);
		float num2 = Mathf.Abs(Vector3.Dot(rhs: (wind - boom.GetPointVelocity(forcePoint.position)).normalized, lhs: boom.transform.right));
		float num3 = Mathf.Sqrt(1f - num2 * num2);
		float time = ReplayRecorder.time;
		float num4 = lastFrameTime - time;
		lastFrameTime = time;
		phaseOpen += num4 * f * f * 100f;
		phaseFill += num4 * f * 5f;
		float value = num2 * num * f;
		float value2 = num3 * num * f;
		value = Smoothing.SmoothValue(smoothFill, value);
		value2 = Smoothing.SmoothValue(smoothOpen, value2);
		SetSignedShape(0, 150f * value + Mathf.Sin(phaseFill) * Mathf.Lerp(5f, 0f, f * 10f));
		SetSignedShape(2, 150f * value2 + Mathf.Sin(phaseOpen) * Mathf.Lerp(10f, 5f, f));
		if (sailSound != null)
		{
			sailSound.SetPitch(Mathf.Lerp(0.4f, 1.2f, f));
			sailSound.SetVolume(Mathf.Lerp(0.2f, 1f, f));
		}
	}

	private void SetSignedShape(int baseIdx, float value)
	{
		skin.SetBlendShapeWeight(baseIdx, Mathf.Clamp(value, 0f, 100f));
		skin.SetBlendShapeWeight(baseIdx + 1, Mathf.Clamp(0f - value, 0f, 100f));
	}

	private void FixedUpdate()
	{
		if (!boom.IsSleeping())
		{
			Vector3 vector = wind - boom.GetPointVelocity(forcePoint.position);
			Vector3 right = boom.transform.right;
			float num = Vector3.Dot(vector.normalized, right);
			if (num < 0f)
			{
				num *= -1f;
				right *= -1f;
			}
			right = (right + boat.transform.up * 0.25f).normalized;
			force = area * right * num * vector.magnitude * vector.magnitude;
			boom.AddForceAtPosition(force, forcePoint.position);
		}
	}
}
