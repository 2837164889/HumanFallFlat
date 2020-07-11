using UnityEngine;

public class Spark : MonoBehaviour
{
	public Color coldColor = Color.red;

	public Color hotColor = Color.yellow;

	public Color emitColor = Color.yellow;

	public float emitIntensity = 2f;

	private Material material;

	private Light light;

	private float lifetime = 1f;

	private void Awake()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		material = component.material;
		component.sharedMaterial = material;
		light = GetComponent<Light>();
	}

	private void LateUpdate()
	{
		lifetime -= Time.deltaTime;
		if (lifetime <= 0.2f)
		{
			base.gameObject.SetActive(value: false);
			SparkPool.instance.Return(this);
			return;
		}
		float b = lifetime * emitIntensity;
		Color color = Color.Lerp(coldColor, hotColor, lifetime);
		material.SetColor("_EmissionColor", emitColor * b);
		material.color = color;
		light.color = emitColor * b;
	}

	internal void Ignite(Vector3 pos, Vector3 speed)
	{
		base.gameObject.SetActive(value: true);
		base.transform.position = pos;
		GetComponent<Rigidbody>().velocity = speed;
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 360f;
		lifetime = Random.Range(0.8f, 1.2f);
	}
}
