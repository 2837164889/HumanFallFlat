using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanAPI
{
	public class SoundManager : MonoBehaviour
	{
		public class SoundGroup
		{
			public string name;

			public List<SoundMaster> sounds = new List<SoundMaster>();
		}

		[Serializable]
		public class SoundManagerState
		{
			public SoundState[] sounds;

			public GrainState[] grains;

			[NonSerialized]
			public Dictionary<string, SoundState> soundStateMap;

			[NonSerialized]
			public Dictionary<string, GrainState> grainStateMap;

			public void Populate()
			{
				if (sounds == null)
				{
					sounds = new SoundState[0];
				}
				if (grains == null)
				{
					grains = new GrainState[0];
				}
				soundStateMap = new Dictionary<string, SoundState>();
				grainStateMap = new Dictionary<string, GrainState>();
				for (int i = 0; i < sounds.Length; i++)
				{
					soundStateMap[sounds[i].id] = sounds[i];
				}
				for (int j = 0; j < grains.Length; j++)
				{
					grainStateMap[grains[j].id] = grains[j];
				}
			}

			public SoundState GetSoundState(string name)
			{
				soundStateMap.TryGetValue(name, out SoundState value);
				return value;
			}

			public GrainState GetGrainState(string name)
			{
				grainStateMap.TryGetValue(name, out GrainState value);
				return value;
			}
		}

		[Serializable]
		public class SoundState
		{
			public string id;

			public string sample;

			public float volume;

			public float pitch;

			public bool useMaster;

			public float maxDistance = 30f;

			public float falloffStart = 1f;

			public float falloffPower = 0.5f;

			public float lpStart = 2f;

			public float lpPower = 0.5f;

			public float spreadNear = 0.5f;

			public float spreadFar;

			public float spatialNear = 0.5f;

			public float spatialFar = 1f;
		}

		[Serializable]
		public class GrainState
		{
			public string id;

			public float slowVolume;

			public float slowPitch;

			public float frequency;

			public float fastJitter;

			public float slowJitter;
		}

		public class SoundMaster
		{
			public SoundManager manager;

			public Sound2 master;

			public List<Sound2> linked = new List<Sound2>();

			public bool isMuted;

			public override string ToString()
			{
				return master.name;
			}

			public void SetSample(string sampleName)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].SetSample(manager.GetSample(sampleName));
				}
			}

			public void SetUseMaster(bool useMaster)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].useMaster = useMaster;
				}
			}

			public void SetBaseVolume(float volume)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].SetBaseVolume(volume);
				}
			}

			public void SetBasePitch(float pitch)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].SetBasePitch(pitch);
				}
			}

			public void SetMaxDistance(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].maxDistance = value;
				}
			}

			public void SetFalloffStart(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].falloffStart = value;
				}
			}

			public void SetFalloffPower(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].falloffPower = value;
				}
			}

			public void SetSpreadNear(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].spreadNear = value;
				}
			}

			public void SetSpreadFar(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].spreadFar = value;
				}
			}

			public void SetSpatialNear(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].spatialNear = value;
				}
			}

			public void SetSpatialFar(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].spatialFar = value;
				}
			}

			public void SetLpStart(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].lpStart = value;
				}
			}

			public void SetLpPower(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].lpPower = value;
				}
			}

			internal void ApplyAttenuation()
			{
				for (int i = 0; i < linked.Count; i++)
				{
					linked[i].ApplyAttenuation();
				}
			}

			public void SetGrainSlowVolume(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					Grain grain = linked[i] as Grain;
					if (!(grain == null))
					{
						grain.slowVolume = value;
					}
				}
			}

			public void SetGrainSlowTune(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					Grain grain = linked[i] as Grain;
					if (!(grain == null))
					{
						grain.slowPitch = value;
					}
				}
			}

			public void SetGrainFrequency(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					Grain grain = linked[i] as Grain;
					if (!(grain == null))
					{
						grain.frequencyAtMaxIntensity = value;
					}
				}
			}

			public void SetGrainSlowJitter(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					Grain grain = linked[i] as Grain;
					if (!(grain == null))
					{
						grain.slowJitter = value;
					}
				}
			}

			public void SetGrainFastJitter(float value)
			{
				for (int i = 0; i < linked.Count; i++)
				{
					Grain grain = linked[i] as Grain;
					if (!(grain == null))
					{
						grain.fastJitter = value;
					}
				}
			}

			public void Mute(bool mute)
			{
				if (isMuted != mute)
				{
					isMuted = mute;
					for (int i = 0; i < linked.Count; i++)
					{
						linked[i].Mute(mute);
					}
				}
			}
		}

		[NonSerialized]
		public List<SoundMaster> sounds;

		[NonSerialized]
		public List<SoundGroup> groups;

		public SoundManagerPrefab storedState;

		public Dictionary<string, SoundMaster> map;

		public SoundManagerType type;

		public static SoundManager main;

		public static SoundManager level;

		public static SoundManager menu;

		public static SoundManager character;

		[NonSerialized]
		public SoundManagerState state;

		public static bool hasSolo;

		public void OnEnable()
		{
			if (storedState != null)
			{
				storedState = UnityEngine.Object.Instantiate(storedState);
				storedState.transform.SetParent(base.transform, worldPositionStays: false);
			}
			switch (type)
			{
			case SoundManagerType.Level:
				level = this;
				break;
			case SoundManagerType.Main:
				main = this;
				break;
			case SoundManagerType.Menu:
				menu = this;
				break;
			case SoundManagerType.Character:
				character = this;
				break;
			}
			if (storedState != null)
			{
				storedState.Initialize();
			}
			LoadStoredSounds();
		}

		private void Start()
		{
		}

		private void LoadStoredSounds()
		{
			if (storedState == null)
			{
				return;
			}
			Sound2[] componentsInChildren = GetComponentsInChildren<Sound2>(includeInactive: true);
			foreach (Sound2 sound in componentsInChildren)
			{
				string fullName = sound.fullName;
				SoundState soundState = storedState.GetSoundState(fullName);
				GrainState grainState = storedState.GetGrainState(fullName);
				if (soundState != null)
				{
					Deserialize(sound, soundState, pasteSample: true);
				}
				if (grainState != null)
				{
					Deserialize(sound, grainState);
				}
			}
		}

		private void LoadOverlaySounds()
		{
			if (type != SoundManagerType.Main && main.state == null)
			{
				main.LoadOverlaySounds();
			}
			CollectSounds();
			state = ReadJson(GetPath(type));
			if (state == null)
			{
				state = new SoundManagerState();
				state.Populate();
			}
			if (type != SoundManagerType.Main)
			{
				for (int i = 0; i < sounds.Count; i++)
				{
					LoadOverlaySound(sounds[i]);
				}
			}
		}

		private void CollectSounds()
		{
			Sound2[] componentsInChildren = GetComponentsInChildren<Sound2>(includeInactive: true);
			sounds = new List<SoundMaster>();
			groups = new List<SoundGroup>();
			map = new Dictionary<string, SoundMaster>();
			foreach (Sound2 sound in componentsInChildren)
			{
				SoundGroup soundGroup = null;
				for (int j = 0; j < groups.Count; j++)
				{
					if (groups[j].name == sound.group)
					{
						soundGroup = groups[j];
						break;
					}
				}
				if (soundGroup == null)
				{
					SoundGroup soundGroup2 = new SoundGroup();
					soundGroup2.name = sound.group;
					soundGroup = soundGroup2;
					groups.Add(soundGroup);
				}
				string key = sound.group + ":" + sound.name;
				if (!map.TryGetValue(key, out SoundMaster value))
				{
					SoundMaster soundMaster = new SoundMaster();
					soundMaster.master = sound;
					soundMaster.manager = this;
					value = soundMaster;
					sounds.Add(value);
					map[key] = value;
					soundGroup.sounds.Add(value);
				}
				value.linked.Add(sound);
			}
		}

		public static SoundManagerState ReadJson(string path)
		{
			try
			{
				string json = File.ReadAllText(path);
				SoundManagerState soundManagerState = JsonUtility.FromJson<SoundManagerState>(json);
				soundManagerState.Populate();
				return soundManagerState;
			}
			catch
			{
				return null;
			}
		}

		private void LoadOverlaySound(SoundMaster sound)
		{
			string fullName = sound.master.fullName;
			SoundState soundState = state.GetSoundState(fullName);
			GrainState grainState = state.GetGrainState(fullName);
			if (soundState == null || soundState.useMaster)
			{
				SoundState soundState2 = main.state.GetSoundState(fullName);
				if (soundState2 != null)
				{
					soundState = soundState2;
				}
				GrainState grainState2 = main.state.GetGrainState(fullName);
				if (grainState2 != null)
				{
					grainState = grainState2;
				}
			}
			if (soundState == null && storedState != null)
			{
				soundState = storedState.GetSoundState(fullName);
				grainState = storedState.GetGrainState(fullName);
			}
			if (soundState != null)
			{
				Deserialize(sound, soundState, pasteSample: true);
			}
			else
			{
				sound.SetSample(sound.master.sample);
			}
			if (grainState != null)
			{
				Deserialize(sound, grainState);
			}
		}

		public SoundMaster GetMaster(Sound2 sound)
		{
			string key = sound.group + ":" + sound.name;
			map.TryGetValue(key, out SoundMaster value);
			return value;
		}

		public void ReapplySamples()
		{
			for (int i = 0; i < sounds.Count; i++)
			{
				sounds[i].SetSample(sounds[i].master.sample);
			}
		}

		public void Save(bool saveMaster)
		{
			List<SoundState> list = new List<SoundState>();
			List<GrainState> list2 = new List<GrainState>();
			if (type == SoundManagerType.Main)
			{
				list = state.soundStateMap.Values.ToList();
				list2 = state.grainStateMap.Values.ToList();
			}
			else
			{
				for (int i = 0; i < sounds.Count; i++)
				{
					Sound2 master = sounds[i].master;
					Grain grain = master as Grain;
					if (master.useMaster && saveMaster)
					{
						main.state.soundStateMap[master.fullName] = Serialize(sounds[i]);
						if (grain != null)
						{
							main.state.grainStateMap[master.fullName] = Serialize(grain);
						}
					}
					else
					{
						list.Add(Serialize(sounds[i]));
						if (grain != null)
						{
							list2.Add(Serialize(grain));
						}
					}
				}
			}
			SoundManagerState soundManagerState = new SoundManagerState();
			soundManagerState.sounds = list.ToArray();
			soundManagerState.grains = list2.ToArray();
			SoundManagerState obj = soundManagerState;
			string contents = JsonUtility.ToJson(obj, prettyPrint: true);
			string path = GetPath(type);
			File.WriteAllText(path, contents);
			if (saveMaster)
			{
				main.Save(saveMaster: false);
			}
		}

		public static string GetPath(SoundManagerType type)
		{
			string str;
			switch (type)
			{
			case SoundManagerType.Main:
				str = "Main";
				break;
			case SoundManagerType.Menu:
				str = "Menu";
				break;
			case SoundManagerType.Character:
				str = "Character";
				break;
			default:
				str = SceneManager.GetActiveScene().name;
				break;
			}
			return "Audio/" + str + "Sounds.txt";
		}

		public static GrainState Serialize(Grain grain)
		{
			GrainState grainState = new GrainState();
			grainState.id = grain.group + ":" + grain.name;
			grainState.slowVolume = grain.slowVolume;
			grainState.slowPitch = grain.slowPitch;
			grainState.frequency = grain.frequencyAtMaxIntensity;
			grainState.fastJitter = grain.fastJitter;
			grainState.slowJitter = grain.slowJitter;
			return grainState;
		}

		public static void Deserialize(SoundMaster master, GrainState grainState)
		{
			master.SetGrainSlowVolume(grainState.slowVolume);
			master.SetGrainSlowTune(grainState.slowPitch);
			master.SetGrainFrequency(grainState.frequency);
			master.SetGrainFastJitter(grainState.fastJitter);
			master.SetGrainSlowJitter(grainState.slowJitter);
		}

		public void Deserialize(Sound2 sound, GrainState grainState)
		{
			Grain grain = sound as Grain;
			if (!(grain == null))
			{
				grain.slowVolume = grainState.slowVolume;
				grain.slowPitch = grainState.slowPitch;
				grain.frequencyAtMaxIntensity = grainState.frequency;
				grain.fastJitter = grainState.fastJitter;
				grain.slowJitter = grainState.slowJitter;
			}
		}

		public static SoundState Serialize(SoundMaster master)
		{
			Sound2 master2 = master.master;
			SoundState soundState = new SoundState();
			soundState.id = master2.group + ":" + master2.name;
			soundState.useMaster = master2.useMaster;
			soundState.sample = master2.sample;
			soundState.volume = master2.baseVolume;
			soundState.pitch = master2.basePitch;
			soundState.maxDistance = master2.maxDistance;
			soundState.falloffStart = master2.falloffStart;
			soundState.falloffPower = master2.falloffPower;
			soundState.lpStart = master2.lpStart;
			soundState.lpPower = master2.lpPower;
			soundState.spreadNear = master2.spreadNear;
			soundState.spreadFar = master2.spreadFar;
			soundState.spatialNear = master2.spatialNear;
			soundState.spatialFar = master2.spatialFar;
			return soundState;
		}

		public static void Deserialize(SoundMaster master, SoundState soundState, bool pasteSample)
		{
			if (pasteSample)
			{
				master.SetSample(soundState.sample);
			}
			master.SetUseMaster(soundState.useMaster);
			master.SetBasePitch(soundState.pitch);
			master.SetBaseVolume(soundState.volume);
			master.SetMaxDistance(soundState.maxDistance);
			master.SetFalloffStart(soundState.falloffStart);
			master.SetFalloffPower(soundState.falloffPower);
			master.SetLpStart(soundState.lpStart);
			master.SetLpPower(soundState.lpPower);
			master.SetSpreadNear(soundState.spreadNear);
			master.SetSpreadFar(soundState.spreadFar);
			master.SetSpatialNear(soundState.spatialNear);
			master.SetSpatialFar(soundState.spatialFar);
			master.ApplyAttenuation();
		}

		public void Deserialize(Sound2 sound, SoundState soundState, bool pasteSample)
		{
			if (pasteSample)
			{
				sound.SetSample(GetSample(soundState.sample));
			}
			sound.useMaster = soundState.useMaster;
			sound.basePitch = soundState.pitch;
			sound.baseVolume = soundState.volume;
			sound.maxDistance = soundState.maxDistance;
			sound.falloffStart = soundState.falloffStart;
			sound.falloffPower = soundState.falloffPower;
			sound.lpStart = soundState.lpStart;
			sound.lpPower = soundState.lpPower;
			sound.spreadNear = soundState.spreadNear;
			sound.spreadFar = soundState.spreadFar;
			sound.spatialNear = soundState.spatialNear;
			sound.spatialFar = soundState.spatialFar;
			sound.ApplyAttenuation();
		}

		public static void SoloSound(SoundMaster master, bool solo)
		{
			hasSolo = solo;
			if (solo)
			{
				for (int i = 0; i < main.sounds.Count; i++)
				{
					main.sounds[i].Mute(mute: true);
				}
				if (level != null)
				{
					for (int j = 0; j < level.sounds.Count; j++)
					{
						level.sounds[j].Mute(mute: true);
					}
				}
				if (menu != null)
				{
					for (int k = 0; k < menu.sounds.Count; k++)
					{
						menu.sounds[k].Mute(mute: true);
					}
				}
				if (character != null)
				{
					for (int l = 0; l < character.sounds.Count; l++)
					{
						character.sounds[l].Mute(mute: true);
					}
				}
				master.Mute(mute: false);
				return;
			}
			for (int m = 0; m < main.sounds.Count; m++)
			{
				main.sounds[m].Mute(mute: false);
			}
			if (level != null)
			{
				for (int n = 0; n < level.sounds.Count; n++)
				{
					level.sounds[n].Mute(mute: false);
				}
			}
			if (menu != null)
			{
				for (int num = 0; num < menu.sounds.Count; num++)
				{
					menu.sounds[num].Mute(mute: false);
				}
			}
			if (character != null)
			{
				for (int num2 = 0; num2 < character.sounds.Count; num2++)
				{
					character.sounds[num2].Mute(mute: false);
				}
			}
		}

		private SoundLibrary.SerializedSample GetSample(string sampleName)
		{
			SoundLibrary.SerializedSample serializedSample = SoundLibrary.GetSample(sampleName);
			if (serializedSample == null && storedState != null)
			{
				SoundLibrarySample sample = storedState.GetSample(sampleName);
				if (sample != null)
				{
					serializedSample = sample.GetSerialized();
				}
			}
			return serializedSample;
		}

		public void RefreshSampleParameters(SoundLibrary.SerializedSample sample)
		{
			for (int i = 0; i < sounds.Count; i++)
			{
				SoundMaster soundMaster = sounds[i];
				if (soundMaster.master.soundSample != null && soundMaster.master.soundSample.name == sample.name)
				{
					for (int j = 0; j < soundMaster.linked.Count; j++)
					{
						soundMaster.linked[j].RefreshSampleParameters();
					}
				}
			}
		}
	}
}
