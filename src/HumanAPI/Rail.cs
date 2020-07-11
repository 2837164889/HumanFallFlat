using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class Rail : MonoBehaviour
	{
		private static List<Rail> all = new List<Rail>();

		public RailEnd start;

		public RailEnd end;

		public int first;

		public int hide;

		public Vector3[] points;

		public float precision = 0.1f;

		private void OnEnable()
		{
			all.Add(this);
		}

		private void OnDisable()
		{
			all.Remove(this);
		}

		private void Awake()
		{
			if (points != null && points.Length != 0)
			{
				for (int i = 0; i < points.Length; i++)
				{
					points[i] = base.transform.TransformPoint(points[i]);
				}
			}
		}

		private void Start()
		{
			if (start == null || end == null)
			{
				Debug.LogError("Rail missing it's ends", this);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (points != null && points.Length != 0)
			{
				Gizmos.color = Color.blue;
				for (int i = 0; i < points.Length - 1; i++)
				{
					Gizmos.DrawLine(base.transform.TransformPoint(points[i]), base.transform.TransformPoint(points[(i + 1) % points.Length]));
				}
				for (int j = 0; j < points.Length; j++)
				{
					Gizmos.DrawSphere(base.transform.TransformPoint(points[j]), 0.1f);
				}
			}
		}

		public void ProjectSegment(Vector3 worldPos, int index, ref float bestDistSqr, ref Vector3 bestProjected, ref Rail bestRail, ref int bestIndex)
		{
			Vector3 vector = Math3d.ProjectPointOnLineSegment(points[index], points[(index + 1) % points.Length], worldPos);
			float sqrMagnitude = (vector - worldPos).sqrMagnitude;
			if (sqrMagnitude < bestDistSqr)
			{
				bestDistSqr = sqrMagnitude;
				bestProjected = vector;
				bestRail = this;
				bestIndex = index;
			}
		}

		public static bool Next(ref Rail currentRail, ref int currentIndex, ref bool fwd)
		{
			RailEnd railEnd = null;
			if (fwd)
			{
				if (currentIndex < currentRail.points.Length - 2)
				{
					currentIndex++;
					return true;
				}
				railEnd = currentRail.end.connectedTo;
			}
			else
			{
				if (currentIndex > 0)
				{
					currentIndex--;
					return true;
				}
				railEnd = currentRail.start.connectedTo;
			}
			if (railEnd == null)
			{
				currentRail = null;
				currentIndex = -1;
				return false;
			}
			currentRail = railEnd.rail;
			if (currentRail.start == railEnd)
			{
				fwd = true;
				currentIndex = 0;
			}
			else
			{
				fwd = false;
				currentIndex = currentRail.points.Length - 1;
			}
			return true;
		}

		public static bool Project(Vector3 worldPos, ref Vector3 projected, ref Rail currentRail, ref int currentIndex)
		{
			Debug.DrawRay(worldPos, Vector3.up, Color.gray);
			float bestDistSqr = float.MaxValue;
			Vector3 bestProjected = Vector3.zero;
			Rail bestRail = null;
			int bestIndex = 0;
			if (currentRail != null)
			{
				currentRail.ProjectSegment(worldPos, currentIndex, ref bestDistSqr, ref bestProjected, ref bestRail, ref bestIndex);
				Rail currentRail2 = currentRail;
				int currentIndex2 = currentIndex;
				bool fwd = true;
				for (int i = 0; i < 5; i++)
				{
					if (!Next(ref currentRail2, ref currentIndex2, ref fwd))
					{
						break;
					}
					currentRail2.ProjectSegment(worldPos, currentIndex2, ref bestDistSqr, ref bestProjected, ref bestRail, ref bestIndex);
				}
				currentRail2 = currentRail;
				currentIndex2 = currentIndex;
				fwd = false;
				for (int j = 0; j < 5; j++)
				{
					if (!Next(ref currentRail2, ref currentIndex2, ref fwd))
					{
						break;
					}
					currentRail2.ProjectSegment(worldPos, currentIndex2, ref bestDistSqr, ref bestProjected, ref bestRail, ref bestIndex);
				}
				if (bestDistSqr > 1f)
				{
					currentRail = null;
				}
			}
			if (currentRail == null)
			{
				for (int k = 0; k < all.Count; k++)
				{
					for (int l = 0; l < all[k].points.Length - 1; l++)
					{
						all[k].ProjectSegment(worldPos, l, ref bestDistSqr, ref bestProjected, ref bestRail, ref bestIndex);
					}
				}
			}
			if (bestDistSqr <= 1f)
			{
				currentRail = bestRail;
				currentIndex = bestIndex;
				projected = bestProjected;
				Debug.DrawRay(projected, Vector3.up, Color.green);
				return true;
			}
			currentRail = null;
			currentIndex = -1;
			projected = Vector3.zero;
			return false;
		}
	}
}
