namespace Voronoi2
{
	public class Halfedge
	{
		public Halfedge ELleft;

		public Halfedge ELright;

		public Edge ELedge;

		public bool deleted;

		public int ELpm;

		public Site vertex;

		public float ystar;

		public Halfedge PQnext;

		public Halfedge()
		{
			PQnext = null;
		}
	}
}
