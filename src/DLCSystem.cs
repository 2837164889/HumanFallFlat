public class DLCSystem : DLC
{
	public override void Init()
	{
	}

	public override bool BundleActive(DLCBundles bundle)
	{
		return true;
	}

	public override bool DLCBuiltIn()
	{
		return true;
	}

	public override bool SupportsDLC()
	{
		return false;
	}

	public override byte[] LoadFile(string filename)
	{
		return null;
	}
}
