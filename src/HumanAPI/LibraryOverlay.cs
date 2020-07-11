using UnityEngine;

namespace HumanAPI
{
	public class LibraryOverlay
	{
		private SoundLibrary activeLibrary;

		public bool showMain;

		private SoundLibrary.SampleCategory activeCategory;

		private Vector2 categoryScrollPos;

		private Vector2 sampleScrollPos;

		private const int sliderWidth = 100;

		private const int sliderHeight = 20;

		public void OnGUI()
		{
			activeLibrary = ((!showMain) ? SoundLibrary.level : SoundLibrary.main);
			if (!(activeLibrary == null))
			{
				if (GUILayout.Button("Save Library", GUILayout.Width(200f)))
				{
					activeLibrary.Save();
				}
				if (GUILayout.Button("Reload Library", GUILayout.Width(200f)))
				{
					activeLibrary.LoadFilesystem();
				}
				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(expand: true));
				ShowCategories();
				ShowSamples();
				GUILayout.EndHorizontal();
			}
		}

		private void ShowCategories()
		{
			categoryScrollPos = GUILayout.BeginScrollView(categoryScrollPos, GUILayout.MinWidth(240f));
			for (int i = 0; i < activeLibrary.library.categories.Count; i++)
			{
				SoundLibrary.SampleCategory sampleCategory = activeLibrary.library.categories[i];
				Rect rect = GUILayoutUtility.GetRect(200f, 24f);
				AudioUI.DrawRect(rect, (sampleCategory != activeCategory) ? new Color(0.3f, 0.3f, 0.3f, 0.5f) : new Color(0f, 0.3f, 0f, 0.7f));
				if (GUI.Toggle(rect, sampleCategory == activeCategory, sampleCategory.name))
				{
					activeCategory = sampleCategory;
				}
			}
			GUILayout.EndScrollView();
		}

		private void ShowSamples()
		{
			if (activeCategory == null)
			{
				return;
			}
			sampleScrollPos = GUILayout.BeginScrollView(sampleScrollPos, GUILayout.MinWidth(640f));
			for (int i = 0; i < activeCategory.samples.Count; i++)
			{
				Rect rect = GUILayoutUtility.GetRect(600f, 0f);
				rect.height = 24f;
				if (i % 2 == 0)
				{
					AudioUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.5f));
				}
				else
				{
					AudioUI.DrawRect(rect, new Color(0f, 0f, 0f, 0.5f));
				}
				GUILayout.BeginHorizontal(GUILayout.Height(rect.height));
				SoundLibrary.SerializedSample serializedSample = activeCategory.samples[i];
				string text = serializedSample.name;
				if (serializedSample.isSwitch)
				{
					text = ((!serializedSample.isLoop) ? (text + " S") : (text + " SL"));
				}
				else if (serializedSample.isLoop)
				{
					text += " L";
				}
				GUILayout.Label(text);
				GUILayout.Label("lvl", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				Rect rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num = AudioUI.DrawHorizontalSlider(rect2, 0f, 1f, 1f, serializedSample.vB, AudioSliderType.Volume);
				rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
				serializedSample.vR = AudioUI.DrawHorizontalSlider(rect2, 0f, 24f, 0f, serializedSample.vR, AudioSliderType.Linear);
				GUILayout.Label("tune", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(75f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num2 = AudioUI.DrawHorizontalSlider(rect2, -2400f, 2400f, 1f, serializedSample.pB, AudioSliderType.Pitch);
				rect2 = GUILayoutUtility.GetRect(50f, rect.height, GUILayout.ExpandWidth(expand: false));
				serializedSample.pR = AudioUI.DrawHorizontalSlider(rect2, 0f, 2400f, 1f, serializedSample.pR, AudioSliderType.Linear);
				GUILayout.Label("xfade", AudioUI.style, GUILayout.ExpandWidth(expand: false));
				rect2 = GUILayoutUtility.GetRect(100f, rect.height, GUILayout.ExpandWidth(expand: false));
				float num3 = AudioUI.DrawHorizontalSlider(rect2, 10f, 5000f, 100f, serializedSample.crossFade * 1000f, AudioSliderType.Log2);
				if (serializedSample.crossFade * 1000f != num3)
				{
					serializedSample.crossFade = num3 / 1000f;
				}
				if (num != serializedSample.vB || num2 != serializedSample.pB)
				{
					serializedSample.vB = num;
					serializedSample.pB = num2;
					if (SoundManager.level != null)
					{
						SoundManager.level.RefreshSampleParameters(serializedSample);
					}
					if (SoundManager.main != null)
					{
						SoundManager.main.RefreshSampleParameters(serializedSample);
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
	}
}
