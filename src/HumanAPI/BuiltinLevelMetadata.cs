namespace HumanAPI
{
	public class BuiltinLevelMetadata : WorkshopLevelMetadata
	{
		public string _thumbPath;

		public string internalName;

		public override string thumbPath => _thumbPath;
	}
}
