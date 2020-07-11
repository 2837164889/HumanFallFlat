using System;

namespace HumanAPI
{
	[Serializable]
	public struct WallVolumeFace
	{
		public WallVolumeOrientaion orientation;

		public int posX;

		public int posY;

		public int posZ;

		public WallVolumeFace(int x, int y, int z, WallVolumeOrientaion orientation)
		{
			posX = x;
			posY = y;
			posZ = z;
			this.orientation = orientation;
		}
	}
}
