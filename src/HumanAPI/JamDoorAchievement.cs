using UnityEngine;

namespace HumanAPI
{
	public class JamDoorAchievement : MonoBehaviour
	{
		public GapJoint door;

		private Collider trackedHuman;

		private float entryX;

		public void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player")
			{
				ServoMotor component = door.GetComponent<ServoMotor>();
				if (!(component == null) && !(component.input.value > 0.5f) && CheckLevelName("Carry"))
				{
					Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
					entryX = vector.x;
					trackedHuman = other;
				}
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other == trackedHuman)
			{
				Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
				if (vector.x * entryX < 0f)
				{
					StatsAndAchievements.UnlockAchievement(Achievement.ACH_CARRY_JAM_DOOR);
				}
			}
		}

		public void OnTriggerStay(Collider other)
		{
			if (trackedHuman != null)
			{
				ServoMotor component = door.GetComponent<ServoMotor>();
				if (component == null || component.input.value > 0.5f)
				{
					trackedHuman = null;
				}
			}
		}

		public static bool CheckLevelName(string whatWeNeed)
		{
			if (Game.instance == null)
			{
				return false;
			}
			int currentLevelNumber = Game.instance.currentLevelNumber;
			string[] levels = Game.instance.levels;
			if (currentLevelNumber < 0 || currentLevelNumber >= levels.Length)
			{
				return false;
			}
			string text = levels[currentLevelNumber];
			return !string.IsNullOrEmpty(text) && whatWeNeed == text;
		}
	}
}
