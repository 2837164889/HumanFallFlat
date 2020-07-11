namespace Voronoi2
{
	public class Edge
	{
		public float a;

		public float b;

		public float c;

		public Site[] ep;

		public Site[] reg;

		public int edgenbr;

		public Edge()
		{
			ep = new Site[2];
			reg = new Site[2];
		}
	}
}
