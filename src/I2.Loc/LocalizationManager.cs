using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace I2.Loc
{
	public static class LocalizationManager
	{
		private enum ProcessMode
		{
			kNone,
			kReadChar,
			kInQuote,
			kSkipLine
		}

		public enum ColumnsName
		{
			Key,
			Type,
			Description,
			English,
			French,
			Spanish,
			German,
			Russian,
			Italian,
			Chinese,
			Japanese,
			Korean,
			Brazilian,
			Turkish,
			Thai,
			Indonesian,
			Polish,
			Ukrainian,
			Arabic,
			Portuguese,
			Lithuanian,
			ColumnMax
		}

		public class LocalisationFile
		{
			public Dictionary<string, List<string>> mRows = new Dictionary<string, List<string>>();

			public List<string> mColumns = new List<string>(21);
		}

		private static string _CurrentLanguage;

		private static bool sInited = false;

		private static string sLanguageCode;

		private static int sLanguageColumn;

		private static string sPlatform = string.Empty;

		private const int kDefaultColumns = 21;

		private const char kSeparator = ',';

		private const char kQuote = '"';

		private const string kKeyName = "Key";

		private const int kMaxBuilderCapacity = 512;

		private static LocalisationFile sLocalisation;

		private static Dictionary<string, Func<string>> DecodeTable = new Dictionary<string, Func<string>>
		{
			{
				"{[ACCEPT_BUTTON]}",
				() => "\u20fd"
			},
			{
				"{[CANCEL_BUTTON]}",
				() => "\u20fe"
			},
			{
				"{[PRIMARY_BUTTON]}",
				() => "\u20fd"
			},
			{
				"{[SECONDARY_BUTTON]}",
				() => "\u20fe"
			},
			{
				"{[THIRD_BUTTON]}",
				() => "℈"
			},
			{
				"{[FOURTH_BUTTON]}",
				() => "ℇ"
			},
			{
				"{[LEFT_STICK]}",
				() => "℃"
			},
			{
				"{[LEFT_STICK_PRESS]}",
				() => "ℕ"
			},
			{
				"{[RIGHT_STICK]}",
				() => "℄"
			},
			{
				"{[RIGHT_STICK_PRESS]}",
				() => "℗"
			},
			{
				"{[LEFT_TRIGGER]}",
				() => "ℙ"
			},
			{
				"{[RIGHT_TRIGGER]}",
				() => "ℚ"
			},
			{
				"{[TILT-JOYCON]}",
				() => ScriptLocalization.Get("XTRA/Tilt")
			}
		};

		public static string CurrentLanguage
		{
			get
			{
				return _CurrentLanguage;
			}
			set
			{
				_CurrentLanguage = value;
				SetLanguageColumn(_CurrentLanguage);
				LocalizeAll(Force: true);
			}
		}

		private static List<string> sLanguageList => (sLocalisation == null) ? null : sLocalisation.mColumns;

		public static event Action OnLocalisation;

		public static List<string> GetAllTranslationsForKey(string key)
		{
			if (!sInited)
			{
				StartupLanguage();
			}
			List<string> value = null;
			if (sLocalisation == null || !sLocalisation.mRows.TryGetValue(key, out value))
			{
				return null;
			}
			return value;
		}

		public static List<string> GetLanguageCodes()
		{
			if (!sInited)
			{
				StartupLanguage();
			}
			return sLanguageList;
		}

		public static void SetPlatform(string platform)
		{
			sPlatform = platform;
		}

		public static string GetTermTranslation(string Term, int overrideLanguage = -1)
		{
			if (string.IsNullOrEmpty(Term))
			{
				return "NoTerm";
			}
			if (!sInited)
			{
				StartupLanguage();
			}
			int num = (overrideLanguage == -1) ? sLanguageColumn : overrideLanguage;
			string text = Term + sPlatform;
			List<string> value = null;
			if (sLocalisation.mRows.TryGetValue(text, out value))
			{
				Term = text;
			}
			else
			{
				sLocalisation.mRows.TryGetValue(Term, out value);
			}
			string text2 = null;
			if (value != null)
			{
				if (num < value.Count)
				{
					text2 = value[num];
					if (text2.Contains("{["))
					{
						text2 = DecodeParameters(text2);
					}
				}
				else
				{
					Debug.Log("GetTermTranslation() language column missing: " + num.ToString());
				}
			}
			else
			{
				Debug.Log("GetTermTranslation() : MISSING KEY!! " + Term);
			}
			if (text2 == null)
			{
				text2 = "Missing: " + Term;
			}
			return text2;
		}

		public static List<string> GetDecodeTableKeys()
		{
			List<string> list = new List<string>(DecodeTable.Count);
			foreach (KeyValuePair<string, Func<string>> item in DecodeTable)
			{
				list.Add(item.Key);
			}
			return list;
		}

		private static string DecodeParameters(string inputString)
		{
			foreach (KeyValuePair<string, Func<string>> item in DecodeTable)
			{
				inputString = inputString.Replace(item.Key, item.Value());
			}
			return inputString;
		}

		public static bool HasLanguage(string language)
		{
			if (!sInited)
			{
				StartupLanguage();
			}
			for (int i = 0; i < sLanguageList.Count; i++)
			{
				if (sLanguageList[i].IndexOf(language, StringComparison.OrdinalIgnoreCase) != -1)
				{
					return true;
				}
			}
			return false;
		}

		public static void StartupLanguage()
		{
			string empty = string.Empty;
			empty = CurrentLanguage;
			TextAsset hFFMasterLocalisationFile = HFFResources.instance.HFFMasterLocalisationFile;
			if (hFFMasterLocalisationFile == null)
			{
				Debug.Log("Accessing the language system too early, localisation asset not loaded yet.");
				return;
			}
			sLocalisation = Load(hFFMasterLocalisationFile.text);
			hFFMasterLocalisationFile = null;
			sInited = true;
			_CurrentLanguage = empty;
			PlayerPrefs.SetString("Language", empty);
			SetLanguageAndCode(empty, GetLanguageCode(empty));
		}

		public static string GetLanguageCode(string Language)
		{
			return _CurrentLanguage;
		}

		private static void SetLanguageColumn(string languageName)
		{
			sLanguageColumn = 3;
			if (sLanguageList == null)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < sLanguageList.Count)
				{
					if (sLanguageList[num].Equals(languageName))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			sLanguageColumn = num;
		}

		public static void SetLanguageAndCode(string LanguageName, string LanguageCode, bool RememberLanguage = true, bool Force = false)
		{
			if (_CurrentLanguage != LanguageName || sLanguageCode != LanguageCode || Force)
			{
				_CurrentLanguage = LanguageName;
				sLanguageCode = LanguageCode;
				SetLanguageColumn(LanguageCode);
				LocalizeAll(Force);
			}
		}

		internal static void LocalizeAll(bool Force = false)
		{
			Localize[] array = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				Localize localize = array[i];
				localize.OnLocalize(Force);
			}
			if (LocalizationManager.OnLocalisation != null)
			{
				LocalizationManager.OnLocalisation();
			}
		}

		private static void AppendChar(StringBuilder builder, char character)
		{
			if (character == '\u00a0')
			{
				character = ' ';
			}
			builder.Append(character);
		}

		private static void EOL(LocalisationFile localisationFile, StringBuilder builder, int lineNumber)
		{
			localisationFile.mColumns.Add(builder.ToString());
			if (!localisationFile.mRows.ContainsKey(localisationFile.mColumns[0]))
			{
				localisationFile.mRows.Add(localisationFile.mColumns[0], new List<string>(localisationFile.mColumns));
			}
			else
			{
				Debug.Log("String table already contains: " + localisationFile.mColumns[0]);
			}
			if (localisationFile.mColumns.Count != 21)
			{
				Debug.Log("Incorrect number of columns on line: " + lineNumber);
			}
			builder.Length = 0;
			localisationFile.mColumns.Clear();
		}

		private static void SkipLine(out bool skipQuote, out ProcessMode mode)
		{
			skipQuote = false;
			mode = ProcessMode.kSkipLine;
		}

		public static LocalisationFile Load(string line)
		{
			int lineNumber = 1;
			ProcessMode processMode = ProcessMode.kReadChar;
			StringBuilder stringBuilder = new StringBuilder(512);
			LocalisationFile localisationFile = new LocalisationFile();
			bool flag = false;
			for (int i = 0; i < line.Length; i++)
			{
				switch (processMode)
				{
				case ProcessMode.kSkipLine:
					if (line[i] != '\r')
					{
						if (line[i] == '"')
						{
							flag = !flag;
						}
						else if (line[i] == '\n' && !flag)
						{
							processMode = ProcessMode.kReadChar;
						}
					}
					break;
				case ProcessMode.kReadChar:
					if (line[i] != '\r')
					{
						if (line[i] == '"')
						{
							processMode = ProcessMode.kInQuote;
						}
						else if (line[i] == ',')
						{
							localisationFile.mColumns.Add(stringBuilder.ToString());
							stringBuilder.Length = 0;
						}
						else if (line[i] == '\n')
						{
							EOL(localisationFile, stringBuilder, lineNumber);
						}
						else
						{
							AppendChar(stringBuilder, line[i]);
						}
					}
					break;
				case ProcessMode.kInQuote:
					if (line[i] == '\r')
					{
						break;
					}
					if (line[i] == '"')
					{
						if (i + 1 < line.Length && line[i + 1] == '"')
						{
							i++;
							AppendChar(stringBuilder, line[i]);
						}
						else
						{
							processMode = ProcessMode.kReadChar;
						}
					}
					else
					{
						AppendChar(stringBuilder, line[i]);
					}
					break;
				}
			}
			localisationFile.mColumns.Clear();
			if (localisationFile.mRows.TryGetValue("Key", out localisationFile.mColumns))
			{
				localisationFile.mRows.Remove("Key");
			}
			return localisationFile;
		}
	}
}
