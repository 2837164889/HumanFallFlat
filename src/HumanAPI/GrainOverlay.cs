using UnityEngine;

namespace HumanAPI
{
	public class GrainOverlay
	{
		private SoundManager activeSounds;

		private const int sliderWidth = 100;

		private const int sliderHeight = 20;

		private Vector2 soundScrollPos;

		private float clipFrequency = 30f;

		private float clipVol = 1f;

		private float clipSlowVol = 1f;

		private float clipTune = 1f;

		private float clipSlowTune = 1f;

		private float clipSlowJitter = 5f;

		private float clipFastJitter;

		public void OnGUI()
		{
			activeSounds = (activeSounds = SoundManager.level);
			if (!(activeSounds == null))
			{
				if (GUILayout.Button("Save Sounds", GUILayout.Width(200f)))
				{
					activeSounds.Save(saveMaster: true);
				}
				ShowGrains();
			}
		}

		private void ShowGrains()
		{
			soundScrollPos = GUILayout.BeginScrollView(soundScrollPos, GUILayout.Width(Screen.width));
			int num = 0;
			for (int i = 0; i < activeSounds.sounds.Count; i++)
			{
				SoundManager.SoundMaster soundMaster = activeSounds.sounds[i];
				Grain grain = soundMaster.master as Grain;
				if (grain == null)
				{
					continue;
				}
				Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
				rect.height = 24f;
				if (num % 2 == 0)
				{
					AudioUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
				}
				else
				{
					AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
				}
				num++;
				GUILayout.BeginHorizontal(GUILayout.Height(rect.height));
				string name = soundMaster.master.name;
				GUILayout.Label(name, GUILayout.Width(200f));
				if (GUILayout.Button(soundMaster.master.sampleLabel, GUILayout.Width(200f)))
				{
					SamplePicker.pickSampleSound = soundMaster;
					if (soundMaster.master.soundSample != null)
					{
						SamplePicker.pickCategoryName = soundMaster.master.soundSample.category;
					}
				}
				GUILayout.Label("freq", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				Rect rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num2 = AudioUI.DrawHorizontalSlider(rect2, 0.5f, 50f, 30f, grain.frequencyAtMaxIntensity, AudioSliderType.Linear);
				if (num2 != grain.frequencyAtMaxIntensity)
				{
					soundMaster.SetGrainFrequency(num2);
				}
				GUILayout.Label("lvl", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num3 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 0f, soundMaster.master.baseVolume, AudioSliderType.Volume);
				if (num3 != soundMaster.master.baseVolume)
				{
					soundMaster.SetBaseVolume(num3);
				}
				GUILayout.Label("slow", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				num3 = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 1f, grain.slowVolume, AudioSliderType.Volume);
				if (num3 != grain.slowVolume)
				{
					soundMaster.SetGrainSlowVolume(num3);
				}
				GUILayout.Label("tune", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num4 = AudioUI.DrawHorizontalSlider(rect2, -2400f, 2400f, 1f, soundMaster.master.basePitch, AudioSliderType.Pitch);
				if (num4 != soundMaster.master.basePitch)
				{
					soundMaster.SetBasePitch(num4);
				}
				GUILayout.Label("slow", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				num4 = AudioUI.DrawHorizontalSlider(rect2, -2400f, 2400f, 1f, grain.slowPitch, AudioSliderType.Pitch);
				if (num4 != grain.slowPitch)
				{
					soundMaster.SetGrainSlowTune(num4);
				}
				GUILayout.Label("jitter", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num5 = AudioUI.DrawHorizontalSlider(rect2, 0f, 10f, 0f, grain.fastJitter, AudioSliderType.Linear);
				if (num5 != grain.fastJitter)
				{
					soundMaster.SetGrainFastJitter(num5);
				}
				GUILayout.Label("slow", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				num5 = AudioUI.DrawHorizontalSlider(rect2, 0f, 10f, 5f, grain.slowJitter, AudioSliderType.Linear);
				if (num5 != grain.slowJitter)
				{
					soundMaster.SetGrainSlowJitter(num5);
				}
				if (GUILayout.Button("C", GUILayout.Width(32f)))
				{
					clipFrequency = grain.frequencyAtMaxIntensity;
					clipVol = grain.baseVolume;
					clipSlowVol = grain.slowVolume;
					clipTune = grain.basePitch;
					clipSlowTune = grain.slowPitch;
					clipFastJitter = grain.fastJitter;
					clipSlowJitter = grain.slowJitter;
				}
				if (GUILayout.Button("V", GUILayout.Width(32f)))
				{
					soundMaster.SetGrainFrequency(clipFrequency);
					soundMaster.SetBaseVolume(clipVol);
					soundMaster.SetGrainSlowVolume(clipSlowVol);
					soundMaster.SetBasePitch(clipTune);
					soundMaster.SetGrainSlowTune(clipSlowTune);
					soundMaster.SetGrainFastJitter(clipFastJitter);
					soundMaster.SetGrainSlowJitter(clipSlowJitter);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
	}
}
