using HumanAPI;
using Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ParticleSystem))]
public class FantasticWeatherFX : MonoBehaviour, IBeginLevel
{
	[SerializeField]
	private float speed = 5f;

	[SerializeField]
	private int frameSkip = 10;

	[SerializeField]
	private float minEmit;

	[SerializeField]
	private float maxEmit;

	[SerializeField]
	private float noise;

	[SerializeField]
	private Vector3 windDirection;

	[SerializeField]
	private float windStrength;

	[SerializeField]
	private NetPlayer player;

	private ParticleSystem pFX;

	private bool process;

	private bool inited;

	private bool hadWeather;

	private int frame;

	private void DisableWeather()
	{
		process = false;
		pFX.Stop();
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	private void EnableWeather()
	{
		process = true;
		pFX.Play();
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
	}

	private bool LevelHasWeather()
	{
		Level level = Object.FindObjectOfType<Level>();
		if (level != null)
		{
			return level.HasWeather;
		}
		return false;
	}

	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	private void Start()
	{
		pFX = GetComponent<ParticleSystem>();
		if (!player.isLocalPlayer || !LevelHasWeather())
		{
			DisableWeather();
			return;
		}
		hadWeather = true;
		EnableWeather();
		inited = true;
	}

	private void OnSceneUnloaded(Scene scene)
	{
		if (hadWeather)
		{
			DisableWeather();
		}
	}

	private void FixedUpdate()
	{
		if (!inited)
		{
			Start();
			inited = true;
		}
		if (process)
		{
			float b = 0f;
			if (frame == frameSkip)
			{
				frame = 0;
				b = ((!Physics.Raycast(base.transform.position + new Vector3(0f, 2f, 0f), Vector3.up, out RaycastHit _, float.PositiveInfinity)) ? maxEmit : minEmit);
			}
			else
			{
				frame++;
			}
			pFX.emissionRate = Mathf.Lerp(pFX.emission.rateOverTime.constantMax, b, speed * Time.deltaTime);
			ParticleSystem.NoiseModule noiseModule = pFX.noise;
			float num2 = noiseModule.strengthMultiplier = Mathf.Lerp(pFX.noise.strengthMultiplier, noise, speed * Time.deltaTime);
			ParticleSystem.ForceOverLifetimeModule forceOverLifetime = pFX.forceOverLifetime;
			Vector3 vector = Vector3.Normalize(windDirection) * windStrength;
			forceOverLifetime.x = vector.x;
			forceOverLifetime.y = vector.y;
			forceOverLifetime.z = vector.z;
			base.transform.rotation = Quaternion.identity;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.GetComponent<FantasticWeatherFXTrigger>())
		{
			FantasticWeatherFXTrigger component = other.GetComponent<FantasticWeatherFXTrigger>();
			maxEmit = component.getEmit();
			noise = component.getNoise();
			windStrength = component.getWindStrength();
			windDirection = component.getWindDirection();
		}
	}

	void IBeginLevel.BeginLevel()
	{
		Start();
	}
}
