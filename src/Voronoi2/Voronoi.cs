using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voronoi2
{
	public class Voronoi
	{
		private float borderMinX;

		private float borderMaxX;

		private float borderMinY;

		private float borderMaxY;

		private int siteidx;

		private float xmin;

		private float xmax;

		private float ymin;

		private float ymax;

		private float deltax;

		private float deltay;

		private int nvertices;

		private int nedges;

		private int nsites;

		private Site[] sites;

		private Site bottomsite;

		private int sqrt_nsites;

		private float minDistanceBetweenSites;

		private int PQcount;

		private int PQmin;

		private int PQhashsize;

		private Halfedge[] PQhash;

		private const int LE = 0;

		private const int RE = 1;

		private int ELhashsize;

		private Halfedge[] ELhash;

		private Halfedge ELleftend;

		private Halfedge ELrightend;

		private List<GraphEdge> allEdges;

		public Voronoi(float minDistanceBetweenSites)
		{
			siteidx = 0;
			sites = null;
			allEdges = null;
			this.minDistanceBetweenSites = minDistanceBetweenSites;
		}

		public List<GraphEdge> generateVoronoi(float[] xValuesIn, float[] yValuesIn, float minX, float maxX, float minY, float maxY)
		{
			sort(xValuesIn, yValuesIn, xValuesIn.Length);
			float num = 0f;
			if (minX > maxX)
			{
				num = minX;
				minX = maxX;
				maxX = num;
			}
			if (minY > maxY)
			{
				num = minY;
				minY = maxY;
				maxY = num;
			}
			borderMinX = minX;
			borderMinY = minY;
			borderMaxX = maxX;
			borderMaxY = maxY;
			siteidx = 0;
			voronoi_bd();
			return allEdges;
		}

		private void sort(float[] xValuesIn, float[] yValuesIn, int count)
		{
			sites = null;
			allEdges = new List<GraphEdge>();
			nsites = count;
			nvertices = 0;
			nedges = 0;
			float num = (float)nsites + 4f;
			sqrt_nsites = (int)Math.Sqrt(num);
			float[] array = new float[count];
			float[] array2 = new float[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = xValuesIn[i];
				array2[i] = yValuesIn[i];
			}
			sortNode(array, array2, count);
		}

		private void qsort(Site[] sites)
		{
			List<Site> list = new List<Site>(sites.Length);
			for (int i = 0; i < sites.Length; i++)
			{
				list.Add(sites[i]);
			}
			list.Sort(new SiteSorterYX());
			for (int j = 0; j < sites.Length; j++)
			{
				sites[j] = list[j];
			}
		}

		private void sortNode(float[] xValues, float[] yValues, int numPoints)
		{
			nsites = numPoints;
			sites = new Site[nsites];
			xmin = xValues[0];
			ymin = yValues[0];
			xmax = xValues[0];
			ymax = yValues[0];
			for (int i = 0; i < nsites; i++)
			{
				sites[i] = new Site();
				sites[i].coord.setPoint(xValues[i], yValues[i]);
				sites[i].sitenbr = i;
				if (xValues[i] < xmin)
				{
					xmin = xValues[i];
				}
				else if (xValues[i] > xmax)
				{
					xmax = xValues[i];
				}
				if (yValues[i] < ymin)
				{
					ymin = yValues[i];
				}
				else if (yValues[i] > ymax)
				{
					ymax = yValues[i];
				}
			}
			qsort(sites);
			deltax = xmax - xmin;
			deltay = ymax - ymin;
		}

		private Site nextone()
		{
			if (siteidx < nsites)
			{
				Site result = sites[siteidx];
				siteidx++;
				return result;
			}
			return null;
		}

		private Edge bisect(Site s1, Site s2)
		{
			Edge edge = new Edge();
			edge.reg[0] = s1;
			edge.reg[1] = s2;
			edge.ep[0] = null;
			edge.ep[1] = null;
			float num = s2.coord.x - s1.coord.x;
			float num2 = s2.coord.y - s1.coord.y;
			float num3 = (!(num > 0f)) ? (0f - num) : num;
			float num4 = (!(num2 > 0f)) ? (0f - num2) : num2;
			edge.c = (float)((double)(s1.coord.x * num + s1.coord.y * num2) + (double)(num * num + num2 * num2) * 0.5);
			if (num3 > num4)
			{
				edge.a = 1f;
				edge.b = num2 / num;
				edge.c /= num;
			}
			else
			{
				edge.a = num / num2;
				edge.b = 1f;
				edge.c /= num2;
			}
			edge.edgenbr = nedges;
			nedges++;
			return edge;
		}

		private void makevertex(Site v)
		{
			v.sitenbr = nvertices;
			nvertices++;
		}

		private bool PQinitialize()
		{
			PQcount = 0;
			PQmin = 0;
			PQhashsize = 4 * sqrt_nsites;
			PQhash = new Halfedge[PQhashsize];
			for (int i = 0; i < PQhashsize; i++)
			{
				PQhash[i] = new Halfedge();
			}
			return true;
		}

		private int PQbucket(Halfedge he)
		{
			int num = (int)((he.ystar - ymin) / deltay * (float)PQhashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= PQhashsize)
			{
				num = PQhashsize - 1;
			}
			if (num < PQmin)
			{
				PQmin = num;
			}
			return num;
		}

		private void PQinsert(Halfedge he, Site v, float offset)
		{
			he.vertex = v;
			he.ystar = v.coord.y + offset;
			Halfedge halfedge = PQhash[PQbucket(he)];
			Halfedge pQnext;
			while ((pQnext = halfedge.PQnext) != null && (he.ystar > pQnext.ystar || (he.ystar == pQnext.ystar && v.coord.x > pQnext.vertex.coord.x)))
			{
				halfedge = pQnext;
			}
			he.PQnext = halfedge.PQnext;
			halfedge.PQnext = he;
			PQcount++;
		}

		private void PQdelete(Halfedge he)
		{
			if (he.vertex != null)
			{
				Halfedge halfedge = PQhash[PQbucket(he)];
				while (halfedge.PQnext != he)
				{
					halfedge = halfedge.PQnext;
				}
				halfedge.PQnext = he.PQnext;
				PQcount--;
				he.vertex = null;
			}
		}

		private bool PQempty()
		{
			return PQcount == 0;
		}

		private Point PQ_min()
		{
			Point point = new Point();
			while (PQhash[PQmin].PQnext == null)
			{
				PQmin++;
			}
			point.x = PQhash[PQmin].PQnext.vertex.coord.x;
			point.y = PQhash[PQmin].PQnext.ystar;
			return point;
		}

		private Halfedge PQextractmin()
		{
			Halfedge pQnext = PQhash[PQmin].PQnext;
			PQhash[PQmin].PQnext = pQnext.PQnext;
			PQcount--;
			return pQnext;
		}

		private Halfedge HEcreate(Edge e, int pm)
		{
			Halfedge halfedge = new Halfedge();
			halfedge.ELedge = e;
			halfedge.ELpm = pm;
			halfedge.PQnext = null;
			halfedge.vertex = null;
			return halfedge;
		}

		private bool ELinitialize()
		{
			ELhashsize = 2 * sqrt_nsites;
			ELhash = new Halfedge[ELhashsize];
			for (int i = 0; i < ELhashsize; i++)
			{
				ELhash[i] = null;
			}
			ELleftend = HEcreate(null, 0);
			ELrightend = HEcreate(null, 0);
			ELleftend.ELleft = null;
			ELleftend.ELright = ELrightend;
			ELrightend.ELleft = ELleftend;
			ELrightend.ELright = null;
			ELhash[0] = ELleftend;
			ELhash[ELhashsize - 1] = ELrightend;
			return true;
		}

		private Halfedge ELright(Halfedge he)
		{
			return he.ELright;
		}

		private Halfedge ELleft(Halfedge he)
		{
			return he.ELleft;
		}

		private Site leftreg(Halfedge he)
		{
			if (he.ELedge == null)
			{
				return bottomsite;
			}
			return (he.ELpm != 0) ? he.ELedge.reg[1] : he.ELedge.reg[0];
		}

		private void ELinsert(Halfedge lb, Halfedge newHe)
		{
			newHe.ELleft = lb;
			newHe.ELright = lb.ELright;
			lb.ELright.ELleft = newHe;
			lb.ELright = newHe;
		}

		private void ELdelete(Halfedge he)
		{
			he.ELleft.ELright = he.ELright;
			he.ELright.ELleft = he.ELleft;
			he.deleted = true;
		}

		private Halfedge ELgethash(int b)
		{
			if (b < 0 || b >= ELhashsize)
			{
				return null;
			}
			Halfedge halfedge = ELhash[b];
			if (halfedge == null || !halfedge.deleted)
			{
				return halfedge;
			}
			ELhash[b] = null;
			return null;
		}

		private Halfedge ELleftbnd(Point p)
		{
			int num = (int)((p.x - xmin) / deltax * (float)ELhashsize);
			if (num < 0)
			{
				num = 0;
			}
			if (num >= ELhashsize)
			{
				num = ELhashsize - 1;
			}
			Halfedge halfedge = ELgethash(num);
			if (halfedge == null)
			{
				for (int i = 1; i < ELhashsize; i++)
				{
					if ((halfedge = ELgethash(num - i)) != null)
					{
						break;
					}
					if ((halfedge = ELgethash(num + i)) != null)
					{
						break;
					}
				}
			}
			if (halfedge == ELleftend || (halfedge != ELrightend && right_of(halfedge, p)))
			{
				do
				{
					halfedge = halfedge.ELright;
				}
				while (halfedge != ELrightend && right_of(halfedge, p));
				halfedge = halfedge.ELleft;
			}
			else
			{
				do
				{
					halfedge = halfedge.ELleft;
				}
				while (halfedge != ELleftend && !right_of(halfedge, p));
			}
			if (num > 0 && num < ELhashsize - 1)
			{
				ELhash[num] = halfedge;
			}
			return halfedge;
		}

		private void pushGraphEdge(Site leftSite, Site rightSite, float x1, float y1, float x2, float y2)
		{
			GraphEdge graphEdge = new GraphEdge();
			allEdges.Add(graphEdge);
			graphEdge.x1 = x1;
			graphEdge.y1 = y1;
			graphEdge.x2 = x2;
			graphEdge.y2 = y2;
			graphEdge.site1 = leftSite.sitenbr;
			graphEdge.site2 = rightSite.sitenbr;
		}

		private void clip_line(Edge e)
		{
			float x = e.reg[0].coord.x;
			float y = e.reg[0].coord.y;
			float x2 = e.reg[1].coord.x;
			float y2 = e.reg[1].coord.y;
			float num = x2 - x;
			float num2 = y2 - y;
			if (Math.Sqrt(num * num + num2 * num2) < (double)minDistanceBetweenSites)
			{
				return;
			}
			float num3 = borderMinX;
			float num4 = borderMinY;
			float num5 = borderMaxX;
			float num6 = borderMaxY;
			Site site;
			Site site2;
			if ((double)e.a == 1.0 && (double)e.b >= 0.0)
			{
				site = e.ep[1];
				site2 = e.ep[0];
			}
			else
			{
				site = e.ep[0];
				site2 = e.ep[1];
			}
			if ((double)e.a == 1.0)
			{
				y = num4;
				if (site != null && site.coord.y > num4)
				{
					y = site.coord.y;
				}
				if (y > num6)
				{
					y = num6;
				}
				x = e.c - e.b * y;
				y2 = num6;
				if (site2 != null && site2.coord.y < num6)
				{
					y2 = site2.coord.y;
				}
				if (y2 < num4)
				{
					y2 = num4;
				}
				x2 = e.c - e.b * y2;
				if ((x > num5 && x2 > num5) | (x < num3 && x2 < num3))
				{
					return;
				}
				if (x > num5)
				{
					x = num5;
					y = (e.c - x) / e.b;
				}
				if (x < num3)
				{
					x = num3;
					y = (e.c - x) / e.b;
				}
				if (x2 > num5)
				{
					x2 = num5;
					y2 = (e.c - x2) / e.b;
				}
				if (x2 < num3)
				{
					x2 = num3;
					y2 = (e.c - x2) / e.b;
				}
			}
			else
			{
				x = num3;
				if (site != null && site.coord.x > num3)
				{
					x = site.coord.x;
				}
				if (x > num5)
				{
					x = num5;
				}
				y = e.c - e.a * x;
				x2 = num5;
				if (site2 != null && site2.coord.x < num5)
				{
					x2 = site2.coord.x;
				}
				if (x2 < num3)
				{
					x2 = num3;
				}
				y2 = e.c - e.a * x2;
				if ((y > num6 && y2 > num6) | (y < num4 && y2 < num4))
				{
					return;
				}
				if (y > num6)
				{
					y = num6;
					x = (e.c - y) / e.a;
				}
				if (y < num4)
				{
					y = num4;
					x = (e.c - y) / e.a;
				}
				if (y2 > num6)
				{
					y2 = num6;
					x2 = (e.c - y2) / e.a;
				}
				if (y2 < num4)
				{
					y2 = num4;
					x2 = (e.c - y2) / e.a;
				}
			}
			pushGraphEdge(e.reg[0], e.reg[1], x, y, x2, y2);
		}

		private void endpoint(Edge e, int lr, Site s)
		{
			e.ep[lr] = s;
			if (e.ep[1 - lr] != null)
			{
				clip_line(e);
			}
		}

		private bool right_of(Halfedge el, Point p)
		{
			Edge eLedge = el.ELedge;
			Site site = eLedge.reg[1];
			bool flag = (p.x > site.coord.x) ? true : false;
			if (flag && el.ELpm == 0)
			{
				return true;
			}
			if (!flag && el.ELpm == 1)
			{
				return false;
			}
			bool flag3;
			if ((double)eLedge.a == 1.0)
			{
				float num = p.x - site.coord.x;
				float num2 = p.y - site.coord.y;
				bool flag2 = false;
				if ((!flag & ((double)eLedge.b < 0.0)) | (flag & ((double)eLedge.b >= 0.0)))
				{
					flag3 = (num2 >= eLedge.b * num);
					flag2 = flag3;
				}
				else
				{
					flag3 = (p.x + p.y * eLedge.b > eLedge.c);
					if ((double)eLedge.b < 0.0)
					{
						flag3 = !flag3;
					}
					if (!flag3)
					{
						flag2 = true;
					}
				}
				if (!flag2)
				{
					float num3 = site.coord.x - eLedge.reg[0].coord.x;
					flag3 = ((double)(eLedge.b * (num * num - num2 * num2)) < (double)(num3 * num2) * (1.0 + 2.0 * (double)num / (double)num3 + (double)(eLedge.b * eLedge.b)));
					if (eLedge.b < 0f)
					{
						flag3 = !flag3;
					}
				}
			}
			else
			{
				float num4 = eLedge.c - eLedge.a * p.x;
				float num5 = p.y - num4;
				float num6 = p.x - site.coord.x;
				float num7 = num4 - site.coord.y;
				flag3 = (num5 * num5 > num6 * num6 + num7 * num7);
			}
			return (el.ELpm != 0) ? (!flag3) : flag3;
		}

		private Site rightreg(Halfedge he)
		{
			if (he.ELedge == null)
			{
				return bottomsite;
			}
			return (he.ELpm != 0) ? he.ELedge.reg[0] : he.ELedge.reg[1];
		}

		private float dist(Site s, Site t)
		{
			float num = s.coord.x - t.coord.x;
			float num2 = s.coord.y - t.coord.y;
			return Mathf.Sqrt(num * num + num2 * num2);
		}

		private Site intersect(Halfedge el1, Halfedge el2)
		{
			Edge eLedge = el1.ELedge;
			Edge eLedge2 = el2.ELedge;
			if (eLedge == null || eLedge2 == null)
			{
				return null;
			}
			if (eLedge.reg[1] == eLedge2.reg[1])
			{
				return null;
			}
			float num = eLedge.a * eLedge2.b - eLedge.b * eLedge2.a;
			if (-1E-10 < (double)num && (double)num < 1E-10)
			{
				return null;
			}
			float num2 = (eLedge.c * eLedge2.b - eLedge2.c * eLedge.b) / num;
			float y = (eLedge2.c * eLedge.a - eLedge.c * eLedge2.a) / num;
			Halfedge halfedge;
			Edge edge;
			if (eLedge.reg[1].coord.y < eLedge2.reg[1].coord.y || (eLedge.reg[1].coord.y == eLedge2.reg[1].coord.y && eLedge.reg[1].coord.x < eLedge2.reg[1].coord.x))
			{
				halfedge = el1;
				edge = eLedge;
			}
			else
			{
				halfedge = el2;
				edge = eLedge2;
			}
			bool flag = num2 >= edge.reg[1].coord.x;
			if ((flag && halfedge.ELpm == 0) || (!flag && halfedge.ELpm == 1))
			{
				return null;
			}
			Site site = new Site();
			site.coord.x = num2;
			site.coord.y = y;
			return site;
		}

		private bool voronoi_bd()
		{
			Point point = null;
			PQinitialize();
			ELinitialize();
			bottomsite = nextone();
			Site site = nextone();
			while (true)
			{
				if (!PQempty())
				{
					point = PQ_min();
				}
				if (site != null && (PQempty() || site.coord.y < point.y || (site.coord.y == point.y && site.coord.x < point.x)))
				{
					Halfedge halfedge = ELleftbnd(site.coord);
					Halfedge el = ELright(halfedge);
					Site s = rightreg(halfedge);
					Edge e = bisect(s, site);
					Halfedge halfedge2 = HEcreate(e, 0);
					ELinsert(halfedge, halfedge2);
					Site site2;
					if ((site2 = intersect(halfedge, halfedge2)) != null)
					{
						PQdelete(halfedge);
						PQinsert(halfedge, site2, dist(site2, site));
					}
					halfedge = halfedge2;
					halfedge2 = HEcreate(e, 1);
					ELinsert(halfedge, halfedge2);
					if ((site2 = intersect(halfedge2, el)) != null)
					{
						PQinsert(halfedge2, site2, dist(site2, site));
					}
					site = nextone();
					continue;
				}
				if (!PQempty())
				{
					Halfedge halfedge = PQextractmin();
					Halfedge halfedge3 = ELleft(halfedge);
					Halfedge el = ELright(halfedge);
					Halfedge el2 = ELright(el);
					Site s = leftreg(halfedge);
					Site site3 = rightreg(el);
					Site vertex = halfedge.vertex;
					makevertex(vertex);
					endpoint(halfedge.ELedge, halfedge.ELpm, vertex);
					endpoint(el.ELedge, el.ELpm, vertex);
					ELdelete(halfedge);
					PQdelete(el);
					ELdelete(el);
					int num = 0;
					if (s.coord.y > site3.coord.y)
					{
						Site site4 = s;
						s = site3;
						site3 = site4;
						num = 1;
					}
					Edge e = bisect(s, site3);
					Halfedge halfedge2 = HEcreate(e, num);
					ELinsert(halfedge3, halfedge2);
					endpoint(e, 1 - num, vertex);
					Site site2;
					if ((site2 = intersect(halfedge3, halfedge2)) != null)
					{
						PQdelete(halfedge3);
						PQinsert(halfedge3, site2, dist(site2, s));
					}
					if ((site2 = intersect(halfedge2, el2)) != null)
					{
						PQinsert(halfedge2, site2, dist(site2, s));
					}
					continue;
				}
				break;
			}
			for (Halfedge halfedge = ELright(ELleftend); halfedge != ELrightend; halfedge = ELright(halfedge))
			{
				Edge e = halfedge.ELedge;
				clip_line(e);
			}
			return true;
		}
	}
}
