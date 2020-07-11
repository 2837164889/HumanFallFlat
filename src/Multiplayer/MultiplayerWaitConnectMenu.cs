namespace Multiplayer
{
	public class MultiplayerWaitConnectMenu : MenuTransition
	{
		public override void OnGotFocus()
		{
			base.OnGotFocus();
		}

		public void BackClick()
		{
			if (MenuSystem.CanInvoke && NetGame.isClient)
			{
				App.instance.CancelConnect();
			}
		}

		public override void ApplyMenuEffects()
		{
			MenuCameraEffects.FadeToBlack(0.001f);
		}

		public override void OnBack()
		{
			BackClick();
		}
	}
}
