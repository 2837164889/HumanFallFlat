using HumanAPI;
using System.Collections.Generic;

public class JITFixupWorkshopTypeRepository
{
	private Dictionary<WorkshopItemSource, List<WorkshopLevelMetadata>> j = new Dictionary<WorkshopItemSource, List<WorkshopLevelMetadata>>();

	private Dictionary<WorkshopItemSource, List<RagdollModelMetadata>> k = new Dictionary<WorkshopItemSource, List<RagdollModelMetadata>>();

	private Dictionary<WorkshopItemSource, List<RagdollPresetMetadata>> l = new Dictionary<WorkshopItemSource, List<RagdollPresetMetadata>>();

	public static JITFixupWorkshopTypeRepository VoodooDoNotCall()
	{
		return new JITFixupWorkshopTypeRepository();
	}
}
