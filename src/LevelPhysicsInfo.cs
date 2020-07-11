using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LevelPhysicsInfo : MonoBehaviour
{
	public class SavedRigidBodyInfo
	{
		public string restartableRigidName;

		public string rigidBodyName;

		public Vector3 pos;

		public Quaternion rot;

		public SavedRigidBodyInfo(string savedRestartableRigidName, string savedRigidBodyName, Vector3 savedPos, Quaternion savedRot)
		{
			restartableRigidName = savedRestartableRigidName;
			rigidBodyName = savedRigidBodyName;
			pos = savedPos;
			rot = savedRot;
		}
	}

	public List<TextAsset> levels = new List<TextAsset>();

	private static Dictionary<string, List<SavedRigidBodyInfo>> levelInfos = new Dictionary<string, List<SavedRigidBodyInfo>>();

	private void Start()
	{
		foreach (TextAsset level in levels)
		{
			if (level != null)
			{
				List<SavedRigidBodyInfo> list = new List<SavedRigidBodyInfo>();
				string[] array = level.text.Split('\n');
				for (int i = 0; i < array.Length - 1; i += 4)
				{
					string text = array[i + 2];
					text = text.Substring(1, text.Length - 3);
					string[] array2 = text.Split(',');
					Vector3 savedPos = new Vector3(float.Parse(array2[0], CultureInfo.InvariantCulture), float.Parse(array2[1], CultureInfo.InvariantCulture), float.Parse(array2[2], CultureInfo.InvariantCulture));
					text = array[i + 3];
					text = text.Substring(1, text.Length - 3);
					array2 = text.Split(',');
					list.Add(new SavedRigidBodyInfo(savedRot: new Quaternion(float.Parse(array2[0], CultureInfo.InvariantCulture), float.Parse(array2[1], CultureInfo.InvariantCulture), float.Parse(array2[2], CultureInfo.InvariantCulture), float.Parse(array2[3], CultureInfo.InvariantCulture)), savedRestartableRigidName: array[i].TrimEnd(), savedRigidBodyName: array[i + 1].TrimEnd(), savedPos: savedPos));
				}
				levelInfos[level.name] = list;
			}
		}
	}

	public static void SetSavedRigidBodies(string levelName)
	{
		if (levelInfos.ContainsKey(levelName))
		{
			List<SavedRigidBodyInfo> list = levelInfos[levelName];
			if (list != null)
			{
				foreach (SavedRigidBodyInfo item in list)
				{
					GameObject gameObject = GameObject.Find(item.restartableRigidName);
					if (gameObject != null)
					{
						RestartableRigid component = gameObject.GetComponent<RestartableRigid>();
						if (component != null)
						{
							component.SetRecordedInfo(item.rigidBodyName, item.pos, item.rot);
						}
					}
				}
			}
		}
	}
}
