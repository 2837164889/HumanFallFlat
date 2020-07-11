using I2.Loc;
using System;
using TMPro;
using UnityEngine;

public class StartupExperienceUI : MonoBehaviour, IDependency
{
	public TextMeshProUGUI pressPrompt;

	public TextMeshProUGUI writtenByLabel;

	public string controllerStartTerm;

	public string keybaordStartTerm;

	public string writtenByTerm;

	public static StartupExperienceUI instance;

	public Animator noBrakesLogo;

	public Animator humanLogo;

	public Animator pressAnything;

	public Animator curveLogo;

	public Animator gameByLine1;

	public Animator gameByLine2;

	public Animator teyonLogo;

	public TMP_FontAsset menuSDFFont;

	public TMP_FontAsset writtenByFont;

	public TMP_FontAsset removeFont;

	public TMP_FontAsset replaceFont;

	public static bool isJapanese;

	[NonSerialized]
	[HideInInspector]
	public static Texture sJapaneseHFFLogo;

	public static bool ControllerAvailable
	{
		get
		{
			string[] joystickNames = Input.GetJoystickNames();
			for (int i = 0; i < joystickNames.Length; i++)
			{
				if (!string.IsNullOrEmpty(joystickNames[i]))
				{
					return true;
				}
			}
			return false;
		}
	}

	public void Initialize()
	{
		instance = this;
		Dependencies.OnInitialized(this);
	}

	private void Start()
	{
		pressPrompt.text = ScriptLocalization.Get((!ControllerAvailable) ? keybaordStartTerm : controllerStartTerm);
		string text = ScriptLocalization.Get(writtenByTerm);
		bool flag = true;
		int i = 0;
		for (int length = text.Length; i < length; i++)
		{
			int num = text[i];
			if ((num < 32 || num >= 127) && num != 201)
			{
				flag = false;
				break;
			}
		}
		writtenByLabel.font = ((!flag) ? menuSDFFont : writtenByFont);
		writtenByLabel.text = text;
	}
}
