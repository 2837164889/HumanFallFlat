using UnityEngine;

namespace HumanAPI
{
	public class ReverbOverlay
	{
		private Reverb activeReverb;

		private const int sliderWidth = 100;

		private const int sliderHeight = 20;

		private Vector2 soundScrollPos;

		private Reverb.ReverbZoneState clipboard;

		public void OnGUI()
		{
			if (activeReverb == null)
			{
				activeReverb = Object.FindObjectOfType<Reverb>();
			}
			if (!(activeReverb == null))
			{
				if (GUILayout.Button("Save Reverb", GUILayout.Width(200f)))
				{
					activeReverb.Save();
				}
				ShowReverbs();
			}
		}

		private void ShowReverbs()
		{
			soundScrollPos = GUILayout.BeginScrollView(soundScrollPos, GUILayout.Width(Screen.width));
			for (int i = 0; i < activeReverb.zones.Length; i++)
			{
				ReverbZone reverbZone = activeReverb.zones[i];
				Rect rect = GUILayoutUtility.GetRect(Screen.width, 0f);
				rect.height = 24f;
				if (ReverbManager.instance.zones.Contains(reverbZone))
				{
					AudioUI.DrawRect(rect, new Color(0.5f, 0.25f, 0.25f, 1f));
				}
				else if (i % 2 == 0)
				{
					AudioUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
				}
				else
				{
					AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
				}
				GUILayout.BeginHorizontal(GUILayout.Height(rect.height));
				string name = reverbZone.name;
				GUILayout.Label(name, GUILayout.Width(200f));
				GUILayout.Label("level", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				Rect rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				reverbZone.level = AudioUI.DrawHorizontalSlider(rect2, -48f, 12f, 0f, reverbZone.level, AudioSliderType.Level);
				GUILayout.Label("delay", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				reverbZone.delay = AudioUI.DrawHorizontalSlider(rect2, 0.1f, 3f, 0.5f, reverbZone.delay, AudioSliderType.Linear);
				GUILayout.Label("diffusion", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				reverbZone.diffusion = AudioUI.DrawHorizontalSlider(rect2, 0.01f, 1f, 0.5f, reverbZone.diffusion, AudioSliderType.Linear);
				GUILayout.Label("HP", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				reverbZone.highPass = AudioUI.DrawHorizontalSlider(rect2, 10f, 22000f, 10f, reverbZone.highPass, AudioSliderType.Log10);
				GUILayout.Label("LP", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				reverbZone.lowPass = AudioUI.DrawHorizontalSlider(rect2, 10f, 22000f, 22000f, reverbZone.lowPass, AudioSliderType.Log10);
				if (GUILayout.Button("C", GUILayout.Width(32f)))
				{
					clipboard = Reverb.Serialize(reverbZone);
				}
				if (GUILayout.Button("V", GUILayout.Width(32f)) && clipboard != null)
				{
					Reverb.Deserialize(reverbZone, clipboard);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
	}
}
