using InControl;
using UnityEngine;

namespace BindingsExample
{
	public class BindingsExample : MonoBehaviour
	{
		private Renderer cachedRenderer;

		private PlayerActions playerActions;

		private string saveData;

		private void OnEnable()
		{
			playerActions = PlayerActions.CreateWithDefaultBindings();
			LoadBindings();
		}

		private void OnDisable()
		{
			playerActions.Destroy();
		}

		private void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
		}

		private void Update()
		{
			base.transform.Rotate(Vector3.down, 500f * Time.deltaTime * playerActions.Move.X, Space.World);
			base.transform.Rotate(Vector3.right, 500f * Time.deltaTime * playerActions.Move.Y, Space.World);
			Color a = (!playerActions.Fire.IsPressed) ? Color.white : Color.red;
			Color b = (!playerActions.Jump.IsPressed) ? Color.white : Color.green;
			cachedRenderer.material.color = Color.Lerp(a, b, 0.5f);
		}

		private void SaveBindings()
		{
			saveData = playerActions.Save();
			PlayerPrefs.SetString("Bindings", saveData);
		}

		private void LoadBindings()
		{
			if (PlayerPrefs.HasKey("Bindings"))
			{
				saveData = PlayerPrefs.GetString("Bindings");
				playerActions.Load(saveData);
			}
		}

		private void OnApplicationQuit()
		{
			PlayerPrefs.Save();
		}

		private void OnGUI()
		{
			float num = 10f;
			GUI.Label(new Rect(10f, num, 300f, num + 22f), "Last Input Type: " + playerActions.LastInputType);
			num += 22f;
			GUI.Label(new Rect(10f, num, 300f, num + 22f), "Last Device Class: " + playerActions.LastDeviceClass);
			num += 22f;
			GUI.Label(new Rect(10f, num, 300f, num + 22f), "Last Device Style: " + playerActions.LastDeviceStyle);
			num += 22f;
			int count = playerActions.Actions.Count;
			for (int i = 0; i < count; i++)
			{
				PlayerAction playerAction = playerActions.Actions[i];
				string text = playerAction.Name;
				if (playerAction.IsListeningForBinding)
				{
					text += " (Listening)";
				}
				text = text + " = " + playerAction.Value;
				GUI.Label(new Rect(10f, num, 500f, num + 22f), text);
				num += 22f;
				int count2 = playerAction.Bindings.Count;
				for (int j = 0; j < count2; j++)
				{
					BindingSource bindingSource = playerAction.Bindings[j];
					GUI.Label(new Rect(75f, num, 300f, num + 22f), bindingSource.DeviceName + ": " + bindingSource.Name);
					if (GUI.Button(new Rect(20f, num + 3f, 20f, 17f), "-"))
					{
						playerAction.RemoveBinding(bindingSource);
					}
					if (GUI.Button(new Rect(45f, num + 3f, 20f, 17f), "+"))
					{
						playerAction.ListenForBindingReplacing(bindingSource);
					}
					num += 22f;
				}
				if (GUI.Button(new Rect(20f, num + 3f, 20f, 17f), "+"))
				{
					playerAction.ListenForBinding();
				}
				if (GUI.Button(new Rect(50f, num + 3f, 50f, 17f), "Reset"))
				{
					playerAction.ResetBindings();
				}
				num += 25f;
			}
			if (GUI.Button(new Rect(20f, num + 3f, 50f, 22f), "Load"))
			{
				LoadBindings();
			}
			if (GUI.Button(new Rect(80f, num + 3f, 50f, 22f), "Save"))
			{
				SaveBindings();
			}
			if (GUI.Button(new Rect(140f, num + 3f, 50f, 22f), "Reset"))
			{
				playerActions.Reset();
			}
		}
	}
}
