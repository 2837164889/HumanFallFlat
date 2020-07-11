using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanAPI
{
	public class Ambience : MonoBehaviour
	{
		[Serializable]
		public class AmbienceState
		{
			public AmbienceZoneState[] zones;
		}

		[Serializable]
		public class AmbienceZoneState
		{
			public string name;

			public AmbienceZoneSourceState[] sources;

			public float mainVerbLevel;

			public float musicLevel;

			public float ambienceLevel;

			public float effectsLevel;

			public float physicsLevel;

			public float characterLevel;

			public float ambienceFxLevel;
		}

		[Serializable]
		public class AmbienceZoneSourceState
		{
			public string source;

			public float volume;
		}

		public static Ambience instance;

		private List<AmbienceZone> activeZones = new List<AmbienceZone>();

		[NonSerialized]
		public AmbienceZone activeZone;

		public bool useTriggers;

		public AmbienceSource[] sources;

		private AmbienceZone[] zones;

		[NonSerialized]
		public AmbienceZone forcedZone;

		public void OnEnable()
		{
			instance = this;
			sources = GetComponentsInChildren<AmbienceSource>();
		}

		private void Start()
		{
			SoundManager componentInParent = GetComponentInParent<SoundManager>();
			if (componentInParent != null && componentInParent.storedState != null && componentInParent.storedState.ambience != null)
			{
				LoadJson(componentInParent.storedState.ambience);
			}
		}

		public void TransitionToZone(AmbienceZone zone, float duration)
		{
			activeZone = zone;
			for (int i = 0; i < sources.Length; i++)
			{
				float volume = 0f;
				for (int j = 0; j < zone.sources.Length; j++)
				{
					if (zone.sources[j] == sources[i])
					{
						volume = zone.volumes[j];
					}
				}
				sources[i].FadeVolume(volume, duration);
			}
			GameAudio.instance.SetAmbienceZoneMix(zone, duration);
		}

		public void EnterZone(AmbienceZone trigger)
		{
			if (!activeZones.Contains(trigger))
			{
				activeZones.Add(trigger);
				CalculateActiveZoneTrigger();
			}
		}

		public void LeaveZone(AmbienceZone trigger)
		{
			if (activeZones.Contains(trigger))
			{
				activeZones.Remove(trigger);
				CalculateActiveZoneTrigger();
			}
		}

		private void CalculateActiveZoneTrigger()
		{
			if (activeZones.Count == 0)
			{
				return;
			}
			AmbienceZone ambienceZone = activeZones[0];
			for (int i = 1; i < activeZones.Count; i++)
			{
				if (activeZones[i].priority < ambienceZone.priority)
				{
					ambienceZone = activeZones[i];
				}
			}
			if (!(activeZone == ambienceZone))
			{
				TransitionToZone(ambienceZone, ambienceZone.transitionDuration);
			}
		}

		private void CalculateActiveZone()
		{
			int num = int.MinValue;
			AmbienceZone ambienceZone = null;
			if (forcedZone != null)
			{
				ambienceZone = forcedZone;
			}
			else
			{
				Vector3 position = Listener.instance.transform.position;
				if (zones == null)
				{
					zones = GetComponentsInChildren<AmbienceZone>();
				}
				for (int i = 0; i < zones.Length; i++)
				{
					if (zones[i].GetComponent<Collider>().bounds.Contains(position) && zones[i].priority > num)
					{
						ambienceZone = zones[i];
						num = zones[i].priority;
					}
				}
			}
			if (!(ambienceZone == null) && !(activeZone == ambienceZone))
			{
				TransitionToZone(ambienceZone, ambienceZone.transitionDuration);
			}
		}

		public static string GetPath()
		{
			string name = SceneManager.GetActiveScene().name;
			return "Audio/" + name + "Ambience.txt";
		}

		public void Load()
		{
			try
			{
				string json = File.ReadAllText(GetPath());
				LoadJson(json);
			}
			catch
			{
			}
		}

		public void LoadJson(string json)
		{
			AmbienceState state = JsonUtility.FromJson<AmbienceState>(json);
			LoadJson(state);
		}

		private void LoadJson(AmbienceState state)
		{
			AmbienceSource[] componentsInChildren = GetComponentsInChildren<AmbienceSource>();
			AmbienceZone[] componentsInChildren2 = GetComponentsInChildren<AmbienceZone>();
			for (int i = 0; i < state.zones.Length; i++)
			{
				AmbienceZoneState ambienceZoneState = state.zones[i];
				foreach (AmbienceZone ambienceZone in componentsInChildren2)
				{
					if (!(ambienceZone.name == ambienceZoneState.name))
					{
						continue;
					}
					ambienceZone.mainVerbLevel = ambienceZoneState.mainVerbLevel;
					ambienceZone.musicLevel = ambienceZoneState.musicLevel;
					ambienceZone.ambienceLevel = ambienceZoneState.ambienceLevel;
					ambienceZone.effectsLevel = ambienceZoneState.effectsLevel;
					ambienceZone.physicsLevel = ambienceZoneState.physicsLevel;
					ambienceZone.characterLevel = ambienceZoneState.characterLevel;
					ambienceZone.ambienceFxLevel = ambienceZoneState.ambienceFxLevel;
					if (ambienceZone.sources == null || ambienceZone.sources.Length != ambienceZoneState.sources.Length)
					{
						ambienceZone.sources = new AmbienceSource[ambienceZoneState.sources.Length];
					}
					if (ambienceZone.volumes == null || ambienceZone.volumes.Length != ambienceZoneState.sources.Length)
					{
						ambienceZone.volumes = new float[ambienceZoneState.sources.Length];
					}
					int num = 0;
					for (int k = 0; k < ambienceZoneState.sources.Length; k++)
					{
						AmbienceZoneSourceState ambienceZoneSourceState = ambienceZoneState.sources[k];
						foreach (AmbienceSource ambienceSource in componentsInChildren)
						{
							if (ambienceZoneSourceState.source == ambienceSource.name)
							{
								ambienceZone.sources[num] = ambienceSource;
								ambienceZone.volumes[num] = ambienceZoneSourceState.volume;
								num++;
							}
						}
					}
					if (num != ambienceZone.sources.Length)
					{
						Array.Resize(ref ambienceZone.sources, num);
						Array.Resize(ref ambienceZone.volumes, num);
					}
				}
			}
		}

		public void ForceZone(AmbienceZone zone)
		{
			forcedZone = zone;
			CalculateActiveZone();
		}

		public void Save()
		{
			AmbienceSource[] componentsInChildren = GetComponentsInChildren<AmbienceSource>();
			AmbienceZone[] componentsInChildren2 = GetComponentsInChildren<AmbienceZone>();
			AmbienceState ambienceState = new AmbienceState();
			ambienceState.zones = new AmbienceZoneState[componentsInChildren2.Length];
			AmbienceState ambienceState2 = ambienceState;
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				AmbienceZone ambienceZone = componentsInChildren2[i];
				ambienceState2.zones[i] = new AmbienceZoneState
				{
					name = ambienceZone.name,
					sources = new AmbienceZoneSourceState[ambienceZone.volumes.Length],
					mainVerbLevel = ambienceZone.mainVerbLevel,
					musicLevel = ambienceZone.musicLevel,
					ambienceLevel = ambienceZone.ambienceLevel,
					effectsLevel = ambienceZone.effectsLevel,
					physicsLevel = ambienceZone.physicsLevel,
					characterLevel = ambienceZone.characterLevel,
					ambienceFxLevel = ambienceZone.ambienceFxLevel
				};
				for (int j = 0; j < ambienceZone.volumes.Length; j++)
				{
					ambienceState2.zones[i].sources[j] = new AmbienceZoneSourceState
					{
						source = ambienceZone.sources[j].name,
						volume = ambienceZone.volumes[j]
					};
				}
			}
			string contents = JsonUtility.ToJson(ambienceState2, prettyPrint: true);
			string path = GetPath();
			File.WriteAllText(path, contents);
		}

		private void Update()
		{
			CalculateActiveZone();
		}
	}
}
