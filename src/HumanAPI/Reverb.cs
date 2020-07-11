using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanAPI
{
	public class Reverb : MonoBehaviour
	{
		[Serializable]
		public class ReverbState
		{
			public ReverbZoneState[] zones;
		}

		[Serializable]
		public class ReverbZoneState
		{
			public string id;

			public float level;

			public float delay = 0.5f;

			public float diffusion = 0.5f;

			public float lowPass = 22000f;

			public float highPass = 10f;
		}

		public static Reverb instance;

		[NonSerialized]
		public ReverbZone[] zones;

		public void OnEnable()
		{
			instance = this;
			zones = GetComponentsInChildren<ReverbZone>();
		}

		private void Start()
		{
			SoundManager componentInParent = GetComponentInParent<SoundManager>();
			if (componentInParent != null && componentInParent.storedState != null && componentInParent.storedState.reverb != null)
			{
				LoadJson(componentInParent.storedState.reverb);
			}
		}

		public static string GetPath()
		{
			string name = SceneManager.GetActiveScene().name;
			return "Audio/" + name + "Reverb.txt";
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
			ReverbState state = JsonUtility.FromJson<ReverbState>(json);
			LoadJson(state);
		}

		private void LoadJson(ReverbState state)
		{
			for (int i = 0; i < state.zones.Length; i++)
			{
				ReverbZoneState reverbZoneState = state.zones[i];
				ReverbZone reverbZone = null;
				for (int j = 0; j < zones.Length; j++)
				{
					if (zones[j].name == reverbZoneState.id)
					{
						reverbZone = zones[j];
						break;
					}
				}
				if (!(reverbZone == null))
				{
					Deserialize(reverbZone, reverbZoneState);
				}
			}
		}

		public void Save()
		{
			ReverbState reverbState = new ReverbState();
			reverbState.zones = new ReverbZoneState[zones.Length];
			ReverbState reverbState2 = reverbState;
			for (int i = 0; i < zones.Length; i++)
			{
				reverbState2.zones[i] = Serialize(zones[i]);
			}
			string contents = JsonUtility.ToJson(reverbState2, prettyPrint: true);
			string path = GetPath();
			File.WriteAllText(path, contents);
		}

		public static ReverbZoneState Serialize(ReverbZone zone)
		{
			ReverbZoneState reverbZoneState = new ReverbZoneState();
			reverbZoneState.id = zone.name;
			reverbZoneState.level = zone.level;
			reverbZoneState.delay = zone.delay;
			reverbZoneState.diffusion = zone.diffusion;
			reverbZoneState.lowPass = zone.lowPass;
			reverbZoneState.highPass = zone.highPass;
			return reverbZoneState;
		}

		public static void Deserialize(ReverbZone zone, ReverbZoneState reverbState)
		{
			zone.level = reverbState.level;
			zone.delay = reverbState.delay;
			zone.diffusion = reverbState.diffusion;
			zone.lowPass = reverbState.lowPass;
			zone.highPass = reverbState.highPass;
		}
	}
}
