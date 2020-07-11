using System;
using System.IO;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
	[Serializable]
	public struct VersionNumber
	{
		public int majorVersion;

		public int minorVersion;

		public int build;

		public int revision;

		public string version;
	}

	public static string gameVersion = "v1.4a0";

	public static string gameBuild = "2617";

	public string gameVersionString;

	public string gameBuildString;

	public const byte presenceBinaryVersion = 1;

	public static uint netCode = 18u;

	private bool lastDisplayedVersion;

	public static string fullVersion => $"{gameVersion} {gameBuild}";

	private void Start()
	{
		TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
		VersionNumber versionNumber = default(VersionNumber);
		TextAsset textAsset = Resources.Load<TextAsset>("Curve/build_version_number");
		XmlSerializer xmlSerializer = new XmlSerializer(versionNumber.GetType());
		StringReader textReader = new StringReader(textAsset.text);
		versionNumber = (VersionNumber)xmlSerializer.Deserialize(textReader);
		gameVersionString = $"v{versionNumber.majorVersion}{versionNumber.minorVersion}{versionNumber.build}{versionNumber.revision}";
		gameVersionString = gameVersionString.Substring(0, 8);
		DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(versionNumber.build).AddSeconds(versionNumber.revision * 2);
		gameBuildString = $"v{versionNumber.majorVersion}{versionNumber.minorVersion}{versionNumber.build}{versionNumber.revision}";
		gameBuildString = gameBuildString.Substring(0, 8);
		GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 700f);
		makeGameBuildString(component);
	}

	private void makeGameBuildString(TextMeshProUGUI text)
	{
		text.text = gameVersionString;
	}

	private void Update()
	{
	}
}
