using Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanAPI
{
	public class BuiltinLevel : Level
	{
		public Transform debugSpawnPoint;

		public GameObject gamePrefab;

		public GameObject resourcePrefab;

		private void Start()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			if (resourcePrefab != null)
			{
				Object.Instantiate(resourcePrefab);
			}
		}

		private void OnDisable()
		{
			FreeRoamCam.CleanUp();
		}

		protected override void OnEnable()
		{
			if (Game.instance == null)
			{
				Object.Instantiate(gamePrefab);
				Dependencies.Initialize<App>();
				for (int i = 0; i < Game.instance.levelCount; i++)
				{
					if (string.Equals(SceneManager.GetActiveScene().name, Game.instance.levels[i]))
					{
						Game.instance.currentLevelNumber = i;
					}
				}
				App.instance.BeginLevel();
			}
			base.OnEnable();
		}

		public override void CompleteLevel()
		{
			base.CompleteLevel();
			DisableOnExit.ExitingLevel(this);
		}
	}
}
