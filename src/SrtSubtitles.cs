using System.Collections.Generic;

public class SrtSubtitles
{
	public class SrtLine
	{
		public int index;

		public float start;

		public float end;

		public string text;

		public string key;

		public bool ShouldShow(float time)
		{
			return start < time && time < end;
		}
	}

	private List<SrtLine> lines = new List<SrtLine>();

	public void Load(string name, string srt)
	{
		name = name.Substring(0, name.IndexOf("Tutorial"));
		lines.Clear();
		string[] array = srt.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
		}
		int j = 0;
		while (j < array.Length)
		{
			for (; j < array.Length && string.IsNullOrEmpty(array[j]); j++)
			{
			}
			if (j >= array.Length || !int.TryParse(array[j++], out int result) || j >= array.Length)
			{
				continue;
			}
			string[] array2 = array[j++].Split(' ');
			if (array2.Length >= 3 && string.Equals(array2[1], "-->") && TryParseTime(array2[0], out float time) && TryParseTime(array2[2], out float time2) && j < array.Length)
			{
				string text = array[j++];
				while (j < array.Length && !string.IsNullOrEmpty(array[j]))
				{
					text = text + "\r\n" + array[j++];
				}
				lines.Add(new SrtLine
				{
					index = result,
					start = time,
					end = time2,
					text = text,
					key = $"{name}_{result}"
				});
			}
		}
	}

	public bool TryParseTime(string str, out float time)
	{
		time = 0f;
		string[] array = str.Split(':', ',');
		if (array.Length != 4)
		{
			return false;
		}
		if (int.TryParse(array[0], out int result) && int.TryParse(array[1], out int result2) && int.TryParse(array[2], out int result3) && int.TryParse(array[3], out int result4))
		{
			time = (float)((result * 60 + result2) * 60 + result3) + (float)result4 * 0.001f;
			return true;
		}
		return false;
	}

	public SrtLine GetLineToDisplay(float time)
	{
		for (int i = 0; i < lines.Count; i++)
		{
			if (lines[i].ShouldShow(time))
			{
				return lines[i];
			}
		}
		return null;
	}
}
