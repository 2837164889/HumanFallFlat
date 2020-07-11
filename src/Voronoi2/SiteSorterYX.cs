using System.Collections.Generic;

namespace Voronoi2
{
	public class SiteSorterYX : IComparer<Site>
	{
		public int Compare(Site p1, Site p2)
		{
			Point coord = p1.coord;
			Point coord2 = p2.coord;
			if (coord.y < coord2.y)
			{
				return -1;
			}
			if (coord.y > coord2.y)
			{
				return 1;
			}
			if (coord.x < coord2.x)
			{
				return -1;
			}
			if (coord.x > coord2.x)
			{
				return 1;
			}
			return 0;
		}
	}
}
