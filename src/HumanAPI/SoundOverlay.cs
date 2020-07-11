using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class SoundOverlay
	{
		private List<string> expandedGroups = new List<string>();

		public SoundManagerType type;

		private Vector2 soundScrollPos;

		private const int sliderWidth = 100;

		private const int sliderHeight = 20;

		private SoundManager.SoundState clipboard;

		public void OnGUI()
		{
			SoundManager soundManager = null;
			switch (type)
			{
			case SoundManagerType.Level:
				soundManager = SoundManager.level;
				break;
			case SoundManagerType.Menu:
				soundManager = SoundManager.menu;
				break;
			case SoundManagerType.Character:
				soundManager = SoundManager.character;
				break;
			default:
				soundManager = null;
				break;
			}
			if (!(soundManager == null))
			{
				if (GUILayout.Button("Save Sounds", GUILayout.Width(200f)))
				{
					soundManager.Save(saveMaster: true);
				}
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(expand: true));
				soundScrollPos = GUILayout.BeginScrollView(soundScrollPos, GUILayout.Width(Screen.width));
				ShowSounds(soundManager);
				GUILayout.EndScrollView();
				GUILayout.EndHorizontal();
			}
		}

		private void ShowSounds(SoundManager activeSounds)
		{
			if (activeSounds == null)
			{
				return;
			}
			for (int i = 0; i < activeSounds.groups.Count; i++)
			{
				SoundManager.SoundGroup soundGroup = activeSounds.groups[i];
				string text = soundGroup.name;
				if (string.IsNullOrEmpty(text))
				{
					text = "none";
				}
				bool flag = expandedGroups.Contains(text);
				if (flag != GUILayout.Toggle(flag, $"{text} ({soundGroup.sounds.Count})"))
				{
					if (!flag)
					{
						expandedGroups.Add(text);
					}
					else
					{
						expandedGroups.Remove(text);
					}
				}
				if (!flag)
				{
					continue;
				}
				for (int j = 0; j < soundGroup.sounds.Count; j++)
				{
					Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
					rect.height = 24f;
					if (j % 2 == 0)
					{
						AudioUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
					}
					else
					{
						AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
					}
					GUILayout.BeginHorizontal(GUILayout.Height(rect.height));
					SoundManager.SoundMaster soundMaster = soundGroup.sounds[j];
					string name = soundMaster.master.name;
					bool flag2 = SoundManager.hasSolo && !soundMaster.isMuted;
					if (flag2 != GUILayout.Toggle(flag2, name, GUILayout.Width(200f)))
					{
						SoundManager.SoloSound(soundMaster, !flag2);
					}
					if (GUILayout.Button(soundMaster.master.sampleLabel, GUILayout.Width(200f)))
					{
						SamplePicker.pickSampleSound = soundMaster;
						if (soundMaster.master.soundSample != null)
						{
							SamplePicker.pickCategoryName = soundMaster.master.soundSample.category;
						}
					}
					GUILayout.Label("lvl", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					Rect rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 1f, soundMaster.master.baseVolume, AudioSliderType.Volume);
					if (num != soundMaster.master.baseVolume)
					{
						soundMaster.SetBaseVolume(num);
					}
					GUILayout.Label("tune", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num2 = AudioUI.DrawHorizontalSlider(rect2, -2400f, 2400f, 0f, soundMaster.master.basePitch, AudioSliderType.Pitch);
					if (num2 != soundMaster.master.basePitch)
					{
						soundMaster.SetBasePitch(num2);
					}
					GUILayout.Label("dist", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num3 = AudioUI.DrawHorizontalSlider(rect2, 10f, 300f, 30f, soundMaster.master.maxDistance, AudioSliderType.Log2);
					if (num3 != soundMaster.master.maxDistance)
					{
						soundMaster.SetMaxDistance(num3);
						soundMaster.ApplyAttenuation();
					}
					GUILayout.Label("lp", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num4 = AudioUI.DrawHorizontalSlider(rect2, 1f, 30f, 2f, soundMaster.master.lpStart, AudioSliderType.Log2);
					if (num4 != soundMaster.master.lpStart)
					{
						soundMaster.SetLpStart(num4);
						soundMaster.ApplyAttenuation();
					}
					rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num5 = AudioUI.DrawHorizontalSlider(rect2, 0.1f, 1f, 0.5f, soundMaster.master.lpPower, AudioSliderType.Linear);
					if (num5 != soundMaster.master.lpPower)
					{
						soundMaster.SetLpPower(num5);
						soundMaster.ApplyAttenuation();
					}
					GUILayout.Label("att", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num6 = AudioUI.DrawHorizontalSlider(rect2, 1f, 30f, 1f, soundMaster.master.falloffStart, AudioSliderType.Log2);
					if (num6 != soundMaster.master.falloffStart)
					{
						soundMaster.SetFalloffStart(num6);
						soundMaster.ApplyAttenuation();
					}
					rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num7 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 0.5f, soundMaster.master.falloffPower, AudioSliderType.Linear);
					if (num7 != soundMaster.master.falloffPower)
					{
						soundMaster.SetFalloffPower(num7);
						soundMaster.ApplyAttenuation();
					}
					GUILayout.Label("spread", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num8 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 0.5f, soundMaster.master.spreadNear, AudioSliderType.Linear);
					if (num8 != soundMaster.master.spreadNear)
					{
						soundMaster.SetSpreadNear(num8);
						soundMaster.ApplyAttenuation();
					}
					rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num9 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 0f, soundMaster.master.spreadFar, AudioSliderType.Linear);
					if (num9 != soundMaster.master.spreadFar)
					{
						soundMaster.SetSpreadFar(num9);
						soundMaster.ApplyAttenuation();
					}
					GUILayout.Label("3d", AudioUI.style, GUILayout.ExpandWidth(expand: false));
					rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
					float num10 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 0.5f, soundMaster.master.spatialNear, AudioSliderType.Linear);
					if (num10 != soundMaster.master.spatialNear)
					{
						soundMaster.SetSpatialNear(num10);
						soundMaster.ApplyAttenuation();
					}
					if (GUILayout.Button("C", GUILayout.Width(32f)))
					{
						clipboard = SoundManager.Serialize(soundMaster);
					}
					if (GUILayout.Button("V", GUILayout.Width(32f)) && clipboard != null)
					{
						SoundManager.Deserialize(soundMaster, clipboard, pasteSample: false);
					}
					if (GUILayout.Button("S", GUILayout.Width(32f)) && clipboard != null)
					{
						SoundManager.Deserialize(soundMaster, clipboard, pasteSample: true);
					}
					bool flag3 = SoundManager.main.state.GetSoundState(soundMaster.master.fullName) != null;
					if (!flag3 && !soundMaster.master.useMaster && GUILayout.Button("Create Master", GUILayout.ExpandWidth(expand: false)))
					{
						soundMaster.SetUseMaster(useMaster: true);
					}
					else if ((flag3 || soundMaster.master.useMaster) && soundMaster.master.useMaster != GUILayout.Toggle(soundMaster.master.useMaster, "Master", GUILayout.ExpandWidth(expand: false)))
					{
						soundMaster.SetUseMaster(!soundMaster.master.useMaster);
						if (soundMaster.master.useMaster)
						{
							SoundManager.SoundState soundState = SoundManager.main.state.GetSoundState(soundMaster.master.fullName);
							if (soundState != null)
							{
								SoundManager.Deserialize(soundMaster, soundState, pasteSample: true);
							}
							SoundManager.GrainState grainState = SoundManager.main.state.GetGrainState(soundMaster.master.fullName);
							if (grainState != null)
							{
								SoundManager.Deserialize(soundMaster, grainState);
							}
						}
					}
					GUILayout.EndHorizontal();
				}
			}
		}
	}
}
