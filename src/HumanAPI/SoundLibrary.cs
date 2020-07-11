using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HumanAPI
{
	public class SoundLibrary : MonoBehaviour
	{
		[Serializable]
		public class SerializedSampleLibrary
		{
			[NonSerialized]
			public SoundLibrary library;

			public List<SerializedSample> samples = new List<SerializedSample>();

			[NonSerialized]
			public Dictionary<string, SerializedSample> map = new Dictionary<string, SerializedSample>();

			[NonSerialized]
			public List<SampleCategory> categories = new List<SampleCategory>();

			[NonSerialized]
			public Dictionary<string, SampleCategory> categoryMap = new Dictionary<string, SampleCategory>();

			public SerializedSample GetSample(string name)
			{
				map.TryGetValue(name, out SerializedSample value);
				return value;
			}

			public void AddSample(SerializedSample sample)
			{
				map[sample.name] = sample;
				samples.Add(sample);
				sample.library = this;
				if (!categoryMap.TryGetValue(sample.category, out SampleCategory value))
				{
					SampleCategory sampleCategory = new SampleCategory();
					sampleCategory.name = sample.category;
					value = sampleCategory;
					categoryMap[sample.category] = value;
					categories.Add(value);
				}
				value.samples.Add(sample);
			}

			public void ReloadSamples()
			{
				for (int i = 0; i < samples.Count; i++)
				{
					SerializedSample serializedSample = samples[i];
					serializedSample.CheckClipFileChange();
				}
			}

			public void MarkAllMissing()
			{
				for (int i = 0; i < samples.Count; i++)
				{
					SerializedSample serializedSample = samples[i];
					for (int j = 0; j < serializedSample.clips.Count; j++)
					{
						if (!serializedSample.clips[j].isEmbedded)
						{
							serializedSample.clips[j].missingFile = true;
						}
					}
				}
			}

			public void RemoveMissing()
			{
				map.Clear();
				Dictionary<string, SampleCategory> dictionary = categoryMap;
				categoryMap = new Dictionary<string, SampleCategory>();
				categories.Clear();
				for (int num = samples.Count - 1; num >= 0; num--)
				{
					SerializedSample serializedSample = samples[num];
					serializedSample.RemoveMissingCips();
					if (serializedSample.clips.Count == 0)
					{
						samples.RemoveAt(num);
					}
					else
					{
						map[serializedSample.name] = serializedSample;
						if (!categoryMap.TryGetValue(serializedSample.category, out SampleCategory value))
						{
							if (dictionary.TryGetValue(serializedSample.category, out value))
							{
								value.samples.Clear();
							}
							else
							{
								SampleCategory sampleCategory = new SampleCategory();
								sampleCategory.name = serializedSample.category;
								value = sampleCategory;
							}
							categoryMap[serializedSample.category] = value;
							categories.Add(value);
						}
						value.samples.Add(serializedSample);
					}
				}
			}
		}

		public class SampleCategory
		{
			public string name;

			public List<SerializedSample> samples = new List<SerializedSample>();
		}

		public interface IClip
		{
			string id
			{
				get;
				set;
			}
		}

		public interface IClipContainer : IClip
		{
			void SetChild(string localId, IClip container);

			SerializedClip GetClip(string currentId, char choice, SampleContainerChildType loopType);
		}

		public class RandomContainer : IClipContainer, IClip
		{
			private Dictionary<string, IClip> children = new Dictionary<string, IClip>();

			private List<IClip> list = new List<IClip>();

			public string id
			{
				get;
				set;
			}

			public void SetChild(string localId, IClip container)
			{
				if (children.TryGetValue(localId, out IClip value))
				{
					if (value == container)
					{
						return;
					}
					list.Remove(value);
				}
				children[localId] = container;
				list.Add(container);
			}

			public SerializedClip GetClip(string currentId, char choice, SampleContainerChildType loopType)
			{
				IClip clip = null;
				if (string.IsNullOrEmpty(currentId) || !currentId.StartsWith(id))
				{
					clip = list[UnityEngine.Random.Range(0, list.Count)];
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (currentId.StartsWith(list[i].id))
						{
							clip = list[i];
							break;
						}
					}
				}
				if (clip is SerializedClip)
				{
					return clip as SerializedClip;
				}
				return (clip as IClipContainer).GetClip(currentId, choice, loopType);
			}

			public override string ToString()
			{
				return $"Random {id}";
			}
		}

		public class LoopContainer : IClipContainer, IClip
		{
			private IClip start;

			private IClip loop;

			private IClip stop;

			public string id
			{
				get;
				set;
			}

			public void SetChild(string localId, IClip container)
			{
				switch (SerializedSample.GetChildType(localId))
				{
				case SampleContainerChildType.Start:
					start = container;
					break;
				case SampleContainerChildType.Stop:
					stop = container;
					break;
				default:
					loop = container;
					break;
				}
			}

			public SerializedClip GetClip(string currentId, char choice, SampleContainerChildType loopType)
			{
				IClip clip;
				switch (loopType)
				{
				case SampleContainerChildType.Start:
					clip = start;
					break;
				case SampleContainerChildType.Stop:
					clip = stop;
					break;
				default:
					clip = loop;
					break;
				}
				if (clip == null)
				{
					return null;
				}
				if (clip is SerializedClip)
				{
					return clip as SerializedClip;
				}
				return (clip as IClipContainer).GetClip(currentId, choice, loopType);
			}

			public override string ToString()
			{
				return $"Loop {id}";
			}
		}

		public class SwitchContainer : IClipContainer, IClip
		{
			private Dictionary<char, IClip> choices = new Dictionary<char, IClip>();

			public string id
			{
				get;
				set;
			}

			public void SetChild(string localId, IClip container)
			{
				choices[char.ToUpper(localId[0])] = container;
			}

			public SerializedClip GetClip(string currentId, char choice, SampleContainerChildType loopType)
			{
				IClip value = null;
				while (value == null && choice >= 'A')
				{
					choices.TryGetValue(choice--, out value);
				}
				if (value == null)
				{
					return null;
				}
				if (value is SerializedClip)
				{
					return value as SerializedClip;
				}
				return (value as IClipContainer).GetClip(currentId, choice, loopType);
			}

			public override string ToString()
			{
				return $"Switch {id}";
			}
		}

		public enum SampleContainerChildType
		{
			None,
			Random,
			Choice,
			Start,
			Loop,
			Stop
		}

		[Serializable]
		public class SerializedSample
		{
			[NonSerialized]
			public SerializedSampleLibrary library;

			public string name;

			public string category;

			public bool isSwitch;

			public bool isLoop;

			public float crossFade = 0.1f;

			public List<SerializedClip> clips;

			public float vB = 1f;

			public float vR;

			public float pB = 1f;

			public float pR;

			private Dictionary<string, IClipContainer> containers = new Dictionary<string, IClipContainer>();

			private IClip root;

			[NonSerialized]
			public bool loaded;

			[NonSerialized]
			public bool builtIn;

			private int loading;

			private bool terminateLoading;

			public float volume => (vR != 0f) ? (vB * AudioUtils.DBToValue(UnityEngine.Random.Range(0f - vR, vR))) : vB;

			public float pitch => (pR != 0f) ? (pB * AudioUtils.CentsToRatio(UnityEngine.Random.Range(0f - pR, pR))) : pB;

			public bool hasSubscribers => this.onLoaded != null;

			private event Action onLoaded;

			public void Merge(SerializedSample serialized)
			{
				if (category != serialized.category)
				{
					Debug.Log("Category change not supported");
				}
				isSwitch = serialized.isSwitch;
				isLoop = serialized.isLoop;
				crossFade = serialized.crossFade;
				vB = serialized.vB;
				vR = serialized.vR;
				pB = serialized.pB;
				pR = serialized.pR;
				for (int i = 0; i < serialized.clips.Count; i++)
				{
					AddClip(serialized.clips[i].filename, serialized.clips[i].id, serialized.clips[i]);
				}
			}

			public void RemoveMissingCips()
			{
				isSwitch = (isLoop = false);
				root = null;
				containers.Clear();
				List<SerializedClip> list = clips;
				clips = new List<SerializedClip>();
				for (int i = 0; i < list.Count; i++)
				{
					if (!list[i].missingFile)
					{
						AddClip(list[i].filename, list[i].id, list[i]);
					}
				}
			}

			private void ParseId(string childId, out string containerId, out string localId)
			{
				int num = childId.LastIndexOf('-');
				if (num >= 0)
				{
					containerId = childId.Substring(0, num);
					localId = childId.Substring(num + 1);
				}
				else
				{
					containerId = string.Empty;
					localId = childId;
				}
			}

			private IClipContainer GetContainer(string containerId)
			{
				containers.TryGetValue(containerId, out IClipContainer value);
				return value;
			}

			public SerializedClip AddClip(string path, string id, SerializedClip copyFrom = null, AudioClip loadedClip = null)
			{
				if (containers == null)
				{
					containers = new Dictionary<string, IClipContainer>();
				}
				if (clips == null)
				{
					clips = new List<SerializedClip>();
				}
				SerializedClip serializedClip = null;
				for (int i = 0; i < clips.Count; i++)
				{
					if (clips[i].id == id)
					{
						serializedClip = clips[i];
						break;
					}
				}
				if (serializedClip == null)
				{
					SerializedClip serializedClip2 = new SerializedClip();
					serializedClip2.id = id;
					serializedClip2.filename = path;
					serializedClip = serializedClip2;
					clips.Add(serializedClip);
				}
				serializedClip.filename = path;
				if (loadedClip != null)
				{
					serializedClip.clip = loadedClip;
					serializedClip.isEmbedded = true;
					serializedClip.clipDirty = false;
					serializedClip.missingFile = false;
				}
				if (copyFrom != null)
				{
					serializedClip.Merge(copyFrom);
				}
				serializedClip.sample = this;
				serializedClip.CheckClipFileChange();
				IClipContainer clipContainer = EnsureContainerFor(id);
				if (clipContainer != null)
				{
					ParseId(id, out string _, out string localId);
					clipContainer.SetChild(localId, serializedClip);
				}
				else
				{
					root = serializedClip;
				}
				return serializedClip;
			}

			private IClipContainer EnsureContainerFor(string childId)
			{
				ParseId(childId, out string containerId, out string localId);
				if (string.IsNullOrEmpty(containerId))
				{
					return null;
				}
				IClipContainer clipContainer = GetContainer(containerId);
				if (clipContainer == null)
				{
					SampleContainerChildType childType = GetChildType(localId);
					clipContainer = CreateContainer(containerId, childType);
					containers[containerId] = clipContainer;
					IClipContainer clipContainer2 = EnsureContainerFor(containerId);
					if (clipContainer2 == null)
					{
						root = clipContainer;
					}
					else
					{
						ParseId(containerId, out string _, out string localId2);
						clipContainer2.SetChild(localId2, clipContainer);
					}
					if (clipContainer is LoopContainer)
					{
						isLoop = true;
					}
					if (clipContainer is SwitchContainer)
					{
						isSwitch = true;
					}
				}
				return clipContainer;
			}

			public static SampleContainerChildType GetChildType(string localChildId)
			{
				if (localChildId.Equals("start", StringComparison.InvariantCultureIgnoreCase) || localChildId.Equals("begin", StringComparison.InvariantCultureIgnoreCase))
				{
					return SampleContainerChildType.Start;
				}
				if (localChildId.Equals("loop", StringComparison.InvariantCultureIgnoreCase) || localChildId.Equals("run", StringComparison.InvariantCultureIgnoreCase))
				{
					return SampleContainerChildType.Loop;
				}
				if (localChildId.Equals("stop", StringComparison.InvariantCultureIgnoreCase) || localChildId.Equals("end", StringComparison.InvariantCultureIgnoreCase))
				{
					return SampleContainerChildType.Stop;
				}
				if (localChildId.Length == 1 && char.IsLetter(localChildId[0]))
				{
					return SampleContainerChildType.Choice;
				}
				if (int.TryParse(localChildId, out int _))
				{
					return SampleContainerChildType.Random;
				}
				return SampleContainerChildType.None;
			}

			private IClipContainer CreateContainer(string containerId, SampleContainerChildType childType)
			{
				switch (childType)
				{
				case SampleContainerChildType.Random:
				{
					RandomContainer randomContainer = new RandomContainer();
					randomContainer.id = containerId;
					return randomContainer;
				}
				case SampleContainerChildType.Choice:
				{
					SwitchContainer switchContainer = new SwitchContainer();
					switchContainer.id = containerId;
					return switchContainer;
				}
				case SampleContainerChildType.Start:
				case SampleContainerChildType.Loop:
				case SampleContainerChildType.Stop:
				{
					LoopContainer loopContainer = new LoopContainer();
					loopContainer.id = containerId;
					return loopContainer;
				}
				case SampleContainerChildType.None:
				{
					RandomContainer randomContainer = new RandomContainer();
					randomContainer.id = containerId;
					return randomContainer;
				}
				default:
					throw new InvalidOperationException();
				}
			}

			private SerializedClip GetClip(string id)
			{
				for (int i = 0; i < clips.Count; i++)
				{
					if (clips[i].id.Equals(id))
					{
						return clips[i];
					}
				}
				return null;
			}

			public SerializedClip GetClip(SerializedClip currentClip, char choice, SampleContainerChildType loopType)
			{
				if (root == null)
				{
					return null;
				}
				if (root is SerializedClip)
				{
					return root as SerializedClip;
				}
				return (root as IClipContainer).GetClip(currentClip?.id, choice, loopType);
			}

			public void Subscribe(Action callback)
			{
				if (this.onLoaded == null && !loaded)
				{
					Coroutines.StartGlobalCoroutine(LoadClips());
				}
				else
				{
					callback();
				}
				onLoaded += callback;
			}

			public void Unsubscribe(Action callback)
			{
				onLoaded -= callback;
				if (this.onLoaded != null)
				{
				}
			}

			public void CheckClipFileChange()
			{
				bool flag = false;
				for (int i = 0; i < clips.Count; i++)
				{
					SerializedClip serializedClip = clips[i];
					serializedClip.CheckClipFileChange();
					if (serializedClip.clipDirty)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Coroutines.StartGlobalCoroutine(LoadClips());
				}
			}

			public IEnumerator LoadClips()
			{
				terminateLoading = true;
				while (loading > 0)
				{
					yield return null;
				}
				terminateLoading = false;
				loading++;
				if (builtIn)
				{
					while (SoundSourcePool.instance == null && !terminateLoading)
					{
						yield return null;
					}
				}
				else
				{
					for (int i = 0; i < clips.Count; i++)
					{
						if (terminateLoading)
						{
							break;
						}
						SerializedClip clip = clips[i];
						if (clip.clip == null || clip.clipDirty)
						{
							DateTime timestamp = File.GetLastWriteTimeUtc(Path.Combine(library.library.wavPath, clip.filename));
							WWW www = new WWW("file://" + Path.GetFullPath(Path.Combine(library.library.wavPath, clip.filename)));
							while (!www.isDone)
							{
								yield return null;
							}
							clip.LoadClip(www.GetAudioClip(threeD: true, stream: false), timestamp);
						}
					}
				}
				loading--;
				if (!terminateLoading)
				{
					loaded = true;
					if (this.onLoaded != null)
					{
						this.onLoaded();
					}
				}
			}
		}

		[Serializable]
		public class SerializedClip : IClip
		{
			[NonSerialized]
			public SerializedSample sample;

			[NonSerialized]
			public bool missingFile;

			private DateTime fileTimestamp;

			[NonSerialized]
			public AudioClip clip;

			[NonSerialized]
			public bool clipDirty;

			[NonSerialized]
			public bool isEmbedded;

			[SerializeField]
			private string _id;

			public string filename;

			public float vB = 1f;

			public float vR;

			public float pB = 1f;

			public float pR;

			public string id
			{
				get
				{
					if (string.IsNullOrEmpty(_id))
					{
						_id = Path.GetFileNameWithoutExtension(filename);
					}
					return _id;
				}
				set
				{
					_id = value;
				}
			}

			public float volume => (vR != 0f) ? (vB * AudioUtils.DBToValue(UnityEngine.Random.Range(0f - vR, vR))) : vB;

			public float pitch => (pR != 0f) ? (pB * AudioUtils.CentsToRatio(UnityEngine.Random.Range(0f - pR, pR))) : pB;

			public void Merge(SerializedClip serialized)
			{
				clip = serialized.clip;
				fileTimestamp = serialized.fileTimestamp;
				vB = serialized.vB;
				vR = serialized.vR;
				pB = serialized.pB;
				pR = serialized.pR;
			}

			public void CheckClipFileChange()
			{
				if (!isEmbedded && !clipDirty && clip != null && fileTimestamp != File.GetLastWriteTimeUtc(Path.Combine(sample.library.library.wavPath, filename)))
				{
					clipDirty = true;
				}
			}

			public void LoadClip(AudioClip clip, DateTime timestamp)
			{
				fileTimestamp = timestamp;
				this.clip = clip;
				clip.name = id;
				clipDirty = false;
			}

			public override string ToString()
			{
				return $"Clip {id} {filename}";
			}
		}

		public string path = "Assets/Audio/Library";

		public bool isMain;

		public static SoundLibrary main;

		public static SoundLibrary level;

		[NonSerialized]
		public SerializedSampleLibrary library;

		public string jsonPath => Path.Combine(wavPath, "library.txt");

		public string wavPath => "Audio/" + path;

		private void OnEnable()
		{
		}

		public static SerializedSample GetSample(string name)
		{
			SerializedSample serializedSample = null;
			if (level != null)
			{
				serializedSample = level.library.GetSample(name);
			}
			if (serializedSample == null && main != null)
			{
				serializedSample = main.library.GetSample(name);
			}
			return serializedSample;
		}

		public void Load()
		{
			SerializedSampleLibrary serializedSampleLibrary = ReadJson(jsonPath);
			if (serializedSampleLibrary != null)
			{
				LoadJson(serializedSampleLibrary);
			}
			LoadFilesystem();
		}

		public static SerializedSampleLibrary ReadJson(string path)
		{
			try
			{
				string json = File.ReadAllText(path);
				SerializedSampleLibrary serializedSampleLibrary = JsonUtility.FromJson<SerializedSampleLibrary>(json);
				for (int i = 0; i < serializedSampleLibrary.samples.Count; i++)
				{
					serializedSampleLibrary.map[serializedSampleLibrary.samples[i].name] = serializedSampleLibrary.samples[i];
				}
				return serializedSampleLibrary;
			}
			catch
			{
				return null;
			}
		}

		public void LoadJson(SerializedSampleLibrary savedLibrary)
		{
			for (int i = 0; i < savedLibrary.samples.Count; i++)
			{
				SerializedSample serializedSample = savedLibrary.samples[i];
				SerializedSample serializedSample2 = library.GetSample(serializedSample.name);
				if (serializedSample2 == null)
				{
					SerializedSample serializedSample3 = new SerializedSample();
					serializedSample3.category = serializedSample.category;
					serializedSample3.name = serializedSample.name;
					serializedSample2 = serializedSample3;
					library.AddSample(serializedSample2);
				}
				serializedSample2.Merge(serializedSample);
			}
		}

		public void Save()
		{
			string contents = JsonUtility.ToJson(library, prettyPrint: true);
			File.WriteAllText(jsonPath, contents);
		}

		public void LoadFilesystem()
		{
			library.MarkAllMissing();
			string fullPath = Path.GetFullPath(wavPath);
			try
			{
				string[] directories = Directory.GetDirectories(fullPath);
				for (int i = 0; i < directories.Length; i++)
				{
					string[] files = Directory.GetFiles(directories[i], "*.wav");
					for (int j = 0; j < files.Length; j++)
					{
						ParseSamplePath(files[j], out string category, out string name, out string id);
						if (!name.StartsWith("._"))
						{
							SerializedSample serializedSample = library.GetSample(name);
							if (serializedSample == null)
							{
								SerializedSample serializedSample2 = new SerializedSample();
								serializedSample2.category = category;
								serializedSample2.name = name;
								serializedSample = serializedSample2;
								library.AddSample(serializedSample);
							}
							serializedSample.category = category;
							SerializedClip serializedClip = serializedSample.AddClip(GetRelativePathTo(fullPath, files[j]), id);
							serializedClip.missingFile = false;
						}
					}
				}
				library.RemoveMissing();
				library.ReloadSamples();
				SoundManager.main.ReapplySamples();
				SoundManager.level.ReapplySamples();
			}
			catch
			{
			}
		}

		public static string GetRelativePathTo(string fromPath, string toPath)
		{
			Uri uri = new Uri(fromPath + "/");
			Uri uri2 = new Uri(toPath);
			Uri uri3 = uri.MakeRelativeUri(uri2);
			return Uri.UnescapeDataString(uri3.ToString());
		}

		private void ParseSamplePath(string path, out string category, out string name, out string id)
		{
			category = Path.GetFileName(Path.GetDirectoryName(path));
			id = Path.GetFileNameWithoutExtension(path);
			int num = id.IndexOf('-');
			if (num < 0)
			{
				name = id;
			}
			else
			{
				name = id.Substring(0, num);
			}
		}
	}
}
