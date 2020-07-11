using System.Collections.Generic;

public abstract class DLC
{
	public enum DLCBundles
	{
		kNone
	}

	public const int kNumberOfDLCLevels = 0;

	protected static Dictionary<string, DLCBundles> DLCNameMappings = new Dictionary<string, DLCBundles>();

	private static Dictionary<int, DLCBundles> DLCLevelMappings = new Dictionary<int, DLCBundles>();

	public static DLC instance;

	public DLC()
	{
		instance = this;
	}

	public abstract void Init();

	public abstract bool SupportsDLC();

	public abstract bool BundleActive(DLCBundles bundle);

	public abstract bool DLCBuiltIn();

	public abstract byte[] LoadFile(string filename);

	public bool LevelIsAvailable(int levelNumber)
	{
		if (DLCLevelMappings.TryGetValue(levelNumber, out DLCBundles value))
		{
			return BundleActive(value);
		}
		return true;
	}

	public DLCBundles LevelIsDLC(string levelName)
	{
		if (DLCNameMappings.TryGetValue(levelName, out DLCBundles value))
		{
			return value;
		}
		return DLCBundles.kNone;
	}

	public DLCBundles LevelIsDLC(ulong levelNumber)
	{
		if (levelNumber < 16 && DLCLevelMappings.TryGetValue((int)levelNumber, out DLCBundles value))
		{
			return value;
		}
		return DLCBundles.kNone;
	}

	public virtual bool Poll()
	{
		return false;
	}

	public virtual void OpenStorePage(DLCBundles bundle)
	{
	}
}
