using HumanAPI;
using UnityEngine.UI;

public class CustomizationEditMenu : MenuTransition
{
	public UnityEngine.UI.Button modelButton;

	public UnityEngine.UI.Button headButton;

	public UnityEngine.UI.Button upperBodyButton;

	public UnityEngine.UI.Button lowerBodyButton;

	public UnityEngine.UI.Button paintButton;

	public UnityEngine.UI.Button webcamButton;

	public override void OnGotFocus()
	{
		base.OnGotFocus();
		CustomizationController.instance.cameraController.FocusCharacterFrontal();
		RagdollCustomization activeCustomization = CustomizationController.instance.activeCustomization;
		modelButton.gameObject.SetActive(WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelFull).Count > 1);
		headButton.gameObject.SetActive(activeCustomization.allowHead && WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelHead).Count > 1);
		upperBodyButton.gameObject.SetActive(activeCustomization.allowUpper && WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelUpperBody).Count > 1);
		lowerBodyButton.gameObject.SetActive(activeCustomization.allowLower && WorkshopRepository.instance.GetPartRepository(WorkshopItemType.ModelLowerBody).Count > 1);
	}

	public void BackClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionBack<CustomizationRootMenu>();
		}
	}

	public override void OnBack()
	{
		BackClick();
	}

	public void ModelClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationSelectModelMenu.part = WorkshopItemType.ModelFull;
			TransitionForward<CustomizationSelectModelMenu>();
		}
	}

	public void HeadClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationSelectModelMenu.part = WorkshopItemType.ModelHead;
			TransitionForward<CustomizationSelectModelMenu>();
		}
	}

	public void UpperClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationSelectModelMenu.part = WorkshopItemType.ModelUpperBody;
			TransitionForward<CustomizationSelectModelMenu>();
		}
	}

	public void LowerClick()
	{
		if (MenuSystem.CanInvoke)
		{
			CustomizationSelectModelMenu.part = WorkshopItemType.ModelLowerBody;
			TransitionForward<CustomizationSelectModelMenu>();
		}
	}

	public void ColorsClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<CustomizationColorsMenu>();
		}
	}

	public void PaintClick()
	{
		if (MenuSystem.CanInvoke)
		{
			TransitionForward<CustomizationPaintMenu>();
			CustomizationPaintMenu.sJustTransitioned = true;
		}
	}
}
