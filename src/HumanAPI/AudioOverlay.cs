using System;
using UnityEngine;

namespace HumanAPI
{
	public class AudioOverlay : MonoBehaviour
	{
		private enum Tab
		{
			Ambience,
			Reverb,
			MainLibrary,
			LevelLibrary,
			Sounds,
			MenuSounds,
			CharSounds,
			Grain
		}

		private Tab currentTab = Tab.Sounds;

		private AmbienceOverlay ambience = new AmbienceOverlay();

		private ReverbOverlay reverb = new ReverbOverlay();

		private LibraryOverlay library = new LibraryOverlay();

		private LibraryOverlay mainlibrary = new LibraryOverlay
		{
			showMain = true
		};

		private SoundOverlay sounds = new SoundOverlay();

		private GrainOverlay grain = new GrainOverlay();

		private SamplePicker picker = new SamplePicker();

		public void ToggleVisibility()
		{
			base.enabled = !base.enabled;
			HumanControls.freezeMouse = base.enabled;
			if (base.enabled)
			{
				MenuSystem.instance.EnterMenuInputMode();
			}
			else
			{
				MenuSystem.instance.ExitMenuInputMode();
			}
		}

		private void OnGUI()
		{
			AudioUI.EnsureStyle();
			GUILayout.BeginVertical(GUILayout.ExpandWidth(expand: true));
			Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
			rect.height = 32f;
			AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
			GUILayout.BeginHorizontal(GUILayout.Height(rect.height));
			Rect rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.Sounds)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.Sounds, "Sounds"))
			{
				currentTab = Tab.Sounds;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.MenuSounds)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.MenuSounds, "Menu"))
			{
				currentTab = Tab.MenuSounds;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.CharSounds)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.CharSounds, "Character"))
			{
				currentTab = Tab.CharSounds;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.Ambience)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.Ambience, "Ambience"))
			{
				currentTab = Tab.Ambience;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.Reverb)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.Reverb, "Reverb"))
			{
				currentTab = Tab.Reverb;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.MainLibrary)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.MainLibrary, "MainSamples"))
			{
				currentTab = Tab.MainLibrary;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.LevelLibrary)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.LevelLibrary, "Samples"))
			{
				currentTab = Tab.LevelLibrary;
			}
			rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
			if (currentTab == Tab.Grain)
			{
				AudioUI.DrawRect(rect2, new Color(0f, 0.3f, 0f, 0.7f));
			}
			if (GUI.Toggle(rect2, currentTab == Tab.Grain, "Grain"))
			{
				currentTab = Tab.Grain;
			}
			GUILayout.EndHorizontal();
			switch (currentTab)
			{
			case Tab.Ambience:
				ambience.OnGUI();
				break;
			case Tab.Reverb:
				reverb.OnGUI();
				break;
			case Tab.LevelLibrary:
				library.OnGUI();
				break;
			case Tab.MainLibrary:
				mainlibrary.OnGUI();
				break;
			case Tab.Sounds:
				sounds.type = SoundManagerType.Level;
				sounds.OnGUI();
				break;
			case Tab.MenuSounds:
				sounds.type = SoundManagerType.Menu;
				sounds.OnGUI();
				break;
			case Tab.CharSounds:
				sounds.type = SoundManagerType.Character;
				sounds.OnGUI();
				break;
			case Tab.Grain:
				grain.OnGUI();
				break;
			default:
				throw new NotImplementedException();
			}
			GUILayout.EndVertical();
			picker.OnGUI();
		}
	}
}
