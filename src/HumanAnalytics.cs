using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class HumanAnalytics : MonoBehaviour
{
	public static HumanAnalytics instance;

	private void OnEnable()
	{
		instance = this;
	}

	public void LoadLevel(string levelName, int levelNumber, int checkpoint, float timeSinceStart)
	{
		Analytics.CustomEvent("loadLevel", new Dictionary<string, object>
		{
			{
				"level",
				levelName
			},
			{
				"cp",
				checkpoint.ToString()
			}
		});
	}

	public void GameOver()
	{
		Analytics.CustomEvent("gameComplete", new Dictionary<string, object>());
	}

	public void BeginMultiplayer(bool host)
	{
		Analytics.CustomEvent("multiplayer", new Dictionary<string, object>
		{
			{
				"isServer",
				host
			}
		});
	}

	public void LogVersion(string version)
	{
		Analytics.CustomEvent("version", new Dictionary<string, object>
		{
			{
				"version",
				version
			}
		});
	}
}
