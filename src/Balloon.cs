using HumanAPI;
using Multiplayer;
using UnityEngine;

public class Balloon : Node
{
	private Renderer rend;

	private Light light;

	private float startingLightIntensity;

	private Color startingEmision;

	public Color myColor = Color.white;

	public NodeInput r;

	public NodeInput g;

	public NodeInput b;

	public float explodeThreshold = 0.5f;

	public float respawnTime = 1f;

	public float explodeTime = 0.5f;

	private NetBody body;

	private void Awake()
	{
		rend = GetComponent<Renderer>();
		light = GetComponent<Light>();
		startingEmision = rend.material.GetColor("_EmissionColor");
	}

	public override void Process()
	{
		base.Process();
		ConsumeLight(new Color(r.value, g.value, b.value));
	}

	private void ConsumeLight(Color c)
	{
		Color dif = myColor - c;
		if (dif.r < 0f - explodeThreshold || dif.g < 0f - explodeThreshold || dif.b < 0f - explodeThreshold)
		{
			Explode();
		}
		else
		{
			UpdateLight(dif);
		}
	}

	private void UpdateLight(Color dif)
	{
		float num = dif.r + dif.g + dif.b;
		float num2 = myColor.r + myColor.g + myColor.b;
		float t = num / num2;
		rend.material.SetColor("_EmissionColor", Color.Lerp(Color.black, startingEmision, t));
		light.intensity = Mathf.Lerp(0f, startingLightIntensity, t);
	}

	private void Explode()
	{
		Invoke("Respawn", respawnTime);
		base.gameObject.SetActive(value: false);
	}

	private void Respawn()
	{
		base.gameObject.SetActive(value: true);
		body.Respawn();
	}
}
