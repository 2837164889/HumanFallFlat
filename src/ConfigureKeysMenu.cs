public class ConfigureKeysMenu : MenuTransition
{
	public KeyBindingButton[] keyButtons;

	public override void ApplyMenuEffects()
	{
		MenuCameraEffects.FadeInPauseMenu();
	}

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		keyButtons = GetComponentsInChildren<KeyBindingButton>();
		keyButtons[0].action = Options.keyboardBindings.LeftHand;
		keyButtons[1].action = Options.keyboardBindings.RightHand;
		keyButtons[2].action = Options.keyboardBindings.Forward;
		keyButtons[3].action = Options.keyboardBindings.Back;
		keyButtons[4].action = Options.keyboardBindings.Left;
		keyButtons[5].action = Options.keyboardBindings.Right;
		keyButtons[6].action = Options.keyboardBindings.Jump;
		keyButtons[7].action = Options.keyboardBindings.Unconscious;
		keyButtons[8].action = Options.keyboardBindings.ViewPlayerNames;
		for (int i = 0; i < keyButtons.Length; i++)
		{
			keyButtons[i].parent = this;
		}
	}

	public void ResetClick()
	{
		if (MenuSystem.CanInvoke)
		{
			Options.keyboardBindings.Reset();
			Options.SaveKeyboardBindings();
			RebindKeys();
		}
	}

	public void RebindKeys()
	{
		for (int i = 0; i < keyButtons.Length; i++)
		{
			keyButtons[i].BindData();
		}
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<KeyboardMouseMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}
}
