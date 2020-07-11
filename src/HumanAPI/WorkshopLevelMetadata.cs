namespace HumanAPI
{
	public class WorkshopLevelMetadata : WorkshopItemMetadata
	{
		public string dataPath => FileTools.Combine(folder, "data");
	}
}
