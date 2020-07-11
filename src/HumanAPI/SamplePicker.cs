using UnityEngine;

namespace HumanAPI
{
	public class SamplePicker
	{
		public static SoundManager.SoundMaster pickSampleSound;

		public static string pickCategoryName;

		public static SoundLibrary pickLibrary;

		private Rect pickerRect = new Rect(100f, 100f, 500f, 400f);

		private Vector2 categoryScrollPos;

		private Vector2 sampleScrollPos;

		public void OnGUI()
		{
			if (pickSampleSound != null)
			{
				pickerRect = GUILayout.Window(15, pickerRect, Window, "Pick Sample");
			}
		}

		private void Window(int id)
		{
			GUILayout.BeginHorizontal();
			categoryScrollPos = GUILayout.BeginScrollView(categoryScrollPos, GUILayout.MinWidth(240f));
			GUILayout.BeginVertical();
			GUILayout.Label("Level Library");
			for (int i = 0; i < SoundLibrary.level.library.categories.Count; i++)
			{
				SoundLibrary.SampleCategory sampleCategory = SoundLibrary.level.library.categories[i];
				Rect rect = GUILayoutUtility.GetRect(200f, 24f);
				AudioUI.DrawRect(rect, (!(sampleCategory.name == pickCategoryName)) ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(0f, 0.3f, 0f, 1f));
				if (GUI.Toggle(rect, sampleCategory.name == pickCategoryName, sampleCategory.name))
				{
					pickCategoryName = sampleCategory.name;
					pickLibrary = SoundLibrary.level;
				}
			}
			GUILayout.Label("Main Library");
			for (int j = 0; j < SoundLibrary.main.library.categories.Count; j++)
			{
				SoundLibrary.SampleCategory sampleCategory2 = SoundLibrary.main.library.categories[j];
				Rect rect2 = GUILayoutUtility.GetRect(200f, 24f);
				AudioUI.DrawRect(rect2, (!(sampleCategory2.name == pickCategoryName)) ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(0f, 0.3f, 0f, 1f));
				if (GUI.Toggle(rect2, sampleCategory2.name == pickCategoryName, sampleCategory2.name))
				{
					pickCategoryName = sampleCategory2.name;
					pickLibrary = SoundLibrary.main;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			sampleScrollPos = GUILayout.BeginScrollView(categoryScrollPos, GUILayout.MinWidth(240f));
			GUILayout.BeginVertical();
			SoundLibrary.SerializedSample serializedSample = null;
			if (pickLibrary != null)
			{
				for (int k = 0; k < pickLibrary.library.samples.Count; k++)
				{
					SoundLibrary.SerializedSample serializedSample2 = pickLibrary.library.samples[k];
					if (!(serializedSample2.category != pickCategoryName))
					{
						bool flag = pickSampleSound.master.sample == serializedSample2.name;
						Rect rect3 = GUILayoutUtility.GetRect(200f, 24f);
						AudioUI.DrawRect(rect3, (!flag) ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(0f, 0.3f, 0f, 1f));
						if (flag != GUI.Toggle(rect3, flag, serializedSample2.name))
						{
							serializedSample = serializedSample2;
						}
					}
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndHorizontal();
			GUI.DragWindow();
			if (serializedSample != null)
			{
				if (pickSampleSound.master.sample != serializedSample.name)
				{
					pickSampleSound.SetSample(serializedSample.name);
				}
				pickSampleSound = null;
			}
		}
	}
}
