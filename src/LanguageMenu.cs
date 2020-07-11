using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageMenu : MenuTransition
{
	public Button english;

	public Button french;

	public Button spanish;

	public Button german;

	public Button russian;

	public Button italian;

	public Button chineseSimplified;

	public Button japanese;

	public Button korean;

	public Button brazilianPortuguese;

	public Button turkish;

	public Button thai;

	public Button indonesian;

	public Button polish;

	public Button ukrainian;

	public Button arabic;

	public Button portuguese;

	public Button lithuanian;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		switch (LocalizationManager.CurrentLanguage.ToLower())
		{
		case "english":
			EventSystem.current.SetSelectedGameObject(english.gameObject);
			break;
		case "french":
			EventSystem.current.SetSelectedGameObject(french.gameObject);
			break;
		case "spanish":
			EventSystem.current.SetSelectedGameObject(spanish.gameObject);
			break;
		case "german":
			EventSystem.current.SetSelectedGameObject(german.gameObject);
			break;
		case "russian":
			EventSystem.current.SetSelectedGameObject(russian.gameObject);
			break;
		case "italian":
			EventSystem.current.SetSelectedGameObject(italian.gameObject);
			break;
		case "chinese simplified":
			EventSystem.current.SetSelectedGameObject(chineseSimplified.gameObject);
			break;
		case "japanese":
			EventSystem.current.SetSelectedGameObject(japanese.gameObject);
			break;
		case "korean":
			EventSystem.current.SetSelectedGameObject(korean.gameObject);
			break;
		case "brazilian portuguese":
			EventSystem.current.SetSelectedGameObject(brazilianPortuguese.gameObject);
			break;
		case "turkish":
			EventSystem.current.SetSelectedGameObject(turkish.gameObject);
			break;
		case "thai":
			EventSystem.current.SetSelectedGameObject(thai.gameObject);
			break;
		case "indonesian":
			EventSystem.current.SetSelectedGameObject(indonesian.gameObject);
			break;
		case "polish":
			EventSystem.current.SetSelectedGameObject(polish.gameObject);
			break;
		case "ukrainian":
			EventSystem.current.SetSelectedGameObject(ukrainian.gameObject);
			break;
		case "arabic":
			EventSystem.current.SetSelectedGameObject(arabic.gameObject);
			break;
		case "portuguese":
			EventSystem.current.SetSelectedGameObject(portuguese.gameObject);
			break;
		case "lithuanian":
			EventSystem.current.SetSelectedGameObject(lithuanian.gameObject);
			break;
		}
	}

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public void English()
	{
		SetLanguage("English");
	}

	public void French()
	{
		SetLanguage("French");
	}

	public void Spanish()
	{
		SetLanguage("Spanish");
	}

	public void German()
	{
		SetLanguage("German");
	}

	public void Russian()
	{
		SetLanguage("Russian");
	}

	public void Italian()
	{
		SetLanguage("Italian");
	}

	public void ChineseSimplified()
	{
		SetLanguage("Chinese Simplified");
	}

	public void Japanese()
	{
		SetLanguage("Japanese");
	}

	public void Korean()
	{
		SetLanguage("Korean");
	}

	public void BrazilianPortuguese()
	{
		SetLanguage("Brazilian Portuguese");
	}

	public void Turkish()
	{
		SetLanguage("Turkish");
	}

	public void Thai()
	{
		SetLanguage("Thai");
	}

	public void Indonesian()
	{
		SetLanguage("Indonesian");
	}

	public void Polish()
	{
		SetLanguage("Polish");
	}

	public void Ukrainian()
	{
		SetLanguage("Ukrainian");
	}

	public void Arabic()
	{
		SetLanguage("Arabic");
	}

	public void Portuguese()
	{
		SetLanguage("Portuguese");
	}

	public void Lithuanian()
	{
		SetLanguage("Lithuanian");
	}

	private void SetLanguage(string language)
	{
		if (MenuSystem.CanInvoke)
		{
			LocalizationManager.CurrentLanguage = language;
			LocalizeOnAwake[] array = (LocalizeOnAwake[])Resources.FindObjectsOfTypeAll(typeof(LocalizeOnAwake));
			LocalizeOnAwake[] array2 = array;
			foreach (LocalizeOnAwake localizeOnAwake in array2)
			{
				localizeOnAwake.Localize();
			}
			PlayerPrefs.SetString("Language", language);
			TransitionForward<OptionsMenu>();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<OptionsMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
