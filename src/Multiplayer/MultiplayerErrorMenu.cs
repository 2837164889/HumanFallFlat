using System;
using TMPro;

namespace Multiplayer
{
	public class MultiplayerErrorMenu : MenuTransition
	{
		public TextMeshProUGUI titleText;

		public TextMeshProUGUI descriptionText;

		public TextMeshProUGUI backText;

		private Action onBack;

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeInPauseMenu();
		}

		public void BackClick()
		{
			if (MenuSystem.CanInvoke)
			{
				onBack();
			}
		}

		public override void OnBack()
		{
			BackClick();
		}

		public static void Show(string title, string description, string backLabel, Action backAction, bool hideOldMenu = false)
		{
			Dialogs.HideProgress();
			MultiplayerErrorMenu menu = MenuSystem.instance.GetMenu<MultiplayerErrorMenu>();
			menu.titleText.text = title;
			menu.descriptionText.text = description;
			menu.backText.text = backLabel;
			menu.onBack = backAction;
			if (DialogOverlay.GetOpacity() >= 0.99f)
			{
				hideOldMenu = true;
			}
			MenuSystem.instance.ShowMenu<MultiplayerErrorMenu>(hideOldMenu);
		}
	}
}
