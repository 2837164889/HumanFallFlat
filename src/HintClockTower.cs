using UnityEngine;

public class HintClockTower : Hint
{
	public Rigidbody theCog;

	protected override bool StillValid()
	{
		if (theCog != null)
		{
			return !theCog.isKinematic;
		}
		return false;
	}
}
