using System;
using UnityEngine;

namespace HumanAPI
{
	public class AmbienceOverlay
	{
		private Ambience activeAmbience;

		private AmbienceSource[] sources;

		private AmbienceZone[] zones;

		private const int sliderWidth = 100;

		private const int sliderHeight = 20;

		private Vector2 scrollPos;

		private void Rebuild()
		{
			activeAmbience = UnityEngine.Object.FindObjectOfType<Ambience>();
			if (!(activeAmbience == null))
			{
				sources = activeAmbience.gameObject.GetComponentsInChildren<AmbienceSource>();
				zones = activeAmbience.gameObject.GetComponentsInChildren<AmbienceZone>();
			}
		}

		public void OnGUI()
		{
			if (activeAmbience == null)
			{
				Rebuild();
			}
			if (activeAmbience == null)
			{
				return;
			}
			if (GUILayout.Button("Save Ambience", GUILayout.Width(200f)))
			{
				activeAmbience.Save();
				SoundManager.level.Save(saveMaster: true);
			}
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width));
			GUILayout.BeginHorizontal();
			GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
			for (int i = 0; i < sources.Length; i++)
			{
				Rect rect = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
				if (sources[i].baseVolume * sources[i].rtVolume > 0f)
				{
					AudioUI.DrawRect(rect, new Color(0.5f, 0.25f, 0.25f, 1f));
				}
				else
				{
					AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
				}
				GUI.Label(rect, sources[i].name);
			}
			GUILayout.EndHorizontal();
			Rect rect2 = GUILayoutUtility.GetRect(600f, 0f);
			rect2.height = 20f;
			AudioUI.DrawRect(rect2, new Color(0.5f, 0.5f, 0.5f, 0.8f));
			GUILayout.BeginHorizontal(GUILayout.Height(rect2.height));
			GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
			for (int j = 0; j < sources.Length; j++)
			{
				Rect rect3 = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
				if (GUI.Button(rect3, sources[j].sampleLabel, AudioUI.buttonStyle))
				{
					SoundManager.SoundMaster soundMaster = SamplePicker.pickSampleSound = SoundManager.level.GetMaster(sources[j]);
					if (soundMaster.master.soundSample != null)
					{
						SamplePicker.pickCategoryName = soundMaster.master.soundSample.category;
					}
				}
			}
			GUILayout.Label(string.Empty, GUILayout.Width(100f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Verb", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Music", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Ambience", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Effects", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Physics", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("Character", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.Label("AmbienceFx", GUILayout.Width(75f), GUILayout.ExpandWidth(expand: false));
			GUILayout.EndHorizontal();
			for (int k = 0; k < zones.Length; k++)
			{
				rect2 = GUILayoutUtility.GetRect(600f, 0f);
				rect2.height = 20f;
				if (k % 2 == 0)
				{
					AudioUI.DrawRect(rect2, new Color(0.2f, 0.2f, 0.2f, 0.5f));
				}
				else
				{
					AudioUI.DrawRect(rect2, new Color(0f, 0f, 0f, 0.5f));
				}
				GUILayout.BeginHorizontal(GUILayout.Height(rect2.height));
				int num = (k + 2) * 20;
				Rect rect4 = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
				bool flag = activeAmbience.activeZone == zones[k];
				if (flag)
				{
					AudioUI.DrawRect(rect4, new Color(0.5f, 0.25f, 0.25f, 1f));
				}
				bool flag2 = flag & (activeAmbience.forcedZone != null);
				if (flag2 != GUI.Toggle(rect4, flag2, zones[k].name))
				{
					if (!flag2)
					{
						activeAmbience.ForceZone(zones[k]);
					}
					else
					{
						activeAmbience.ForceZone(null);
					}
				}
				for (int l = 0; l < sources.Length; l++)
				{
					rect4 = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
					DrawSlider(rect4, zones[k], sources[l]);
				}
				rect4 = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
				GUI.Label(rect4, zones[k].name);
				DrawMixer(zones[k], (AmbienceZone zone) => zone.mainVerbLevel, delegate(AmbienceZone zone, float v)
				{
					zone.mainVerbLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.musicLevel, delegate(AmbienceZone zone, float v)
				{
					zone.musicLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.ambienceLevel, delegate(AmbienceZone zone, float v)
				{
					zone.ambienceLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.effectsLevel, delegate(AmbienceZone zone, float v)
				{
					zone.effectsLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.physicsLevel, delegate(AmbienceZone zone, float v)
				{
					zone.physicsLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.characterLevel, delegate(AmbienceZone zone, float v)
				{
					zone.characterLevel = v;
				});
				DrawMixer(zones[k], (AmbienceZone zone) => zone.ambienceFxLevel, delegate(AmbienceZone zone, float v)
				{
					zone.ambienceFxLevel = v;
				});
				GUILayout.Label("x", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect4 = GUILayoutUtility.GetRect(100f, 20f, GUILayout.ExpandWidth(expand: false));
				float num2 = AudioUI.DrawHorizontalSlider(rect4, 500f, 10000f, 3000f, zones[k].transitionDuration * 1000f, AudioSliderType.Log2);
				if (num2 != zones[k].transitionDuration * 1000f)
				{
					zones[k].transitionDuration = num2 / 1000f;
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}

		private string FormatNumber(float num, int len = 5)
		{
			if (float.IsInfinity(num))
			{
				return "-Inf";
			}
			return num.ToString("0.0");
		}

		private void DrawMixer(AmbienceZone zone, Func<AmbienceZone, float> get, Action<AmbienceZone, float> set)
		{
			Rect rect = GUILayoutUtility.GetRect(75f, 20f, GUILayout.ExpandWidth(expand: false));
			float num = get(zone);
			float num2 = AudioUI.DrawHorizontalSlider(rect, -24f, 24f, 0f, get(zone), AudioSliderType.Level);
			if (num != num2)
			{
				set(zone, num2);
				if (activeAmbience.activeZone != null)
				{
					activeAmbience.TransitionToZone(activeAmbience.activeZone, 0.1f);
				}
			}
		}

		private void DrawSlider(Rect rect, AmbienceZone zone, AmbienceSource source)
		{
			float level = zone.GetLevel(source);
			float num = AudioUI.DrawHorizontalSlider(rect, 0f, 1f, 0f, level, AudioSliderType.Volume);
			if (level == num)
			{
				return;
			}
			if (zone.sources != null && zone.sources.Length > 0)
			{
				for (int i = 0; i < zone.sources.Length; i++)
				{
					if (zone.sources[i] == source)
					{
						if (num == 0f)
						{
							zone.sources[i] = zone.sources[zone.sources.Length - 1];
							zone.volumes[i] = zone.volumes[zone.sources.Length - 1];
							Array.Resize(ref zone.volumes, zone.sources.Length - 1);
							Array.Resize(ref zone.sources, zone.sources.Length - 1);
						}
						else
						{
							zone.volumes[i] = num;
						}
						if (activeAmbience.activeZone != null)
						{
							activeAmbience.TransitionToZone(activeAmbience.activeZone, 0.1f);
						}
						return;
					}
				}
			}
			if (zone.sources != null)
			{
				Array.Resize(ref zone.volumes, zone.sources.Length + 1);
				Array.Resize(ref zone.sources, zone.sources.Length + 1);
			}
			else
			{
				zone.volumes = new float[1];
				zone.sources = new AmbienceSource[1];
			}
			zone.sources[zone.sources.Length - 1] = source;
			zone.volumes[zone.sources.Length - 1] = num;
			if (activeAmbience.activeZone != null)
			{
				activeAmbience.TransitionToZone(activeAmbience.activeZone, 0.1f);
			}
		}
	}
}
