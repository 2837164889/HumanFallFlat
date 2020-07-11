using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class HosePipe : Rope
	{
		public HoseConnector startPlug;

		public HoseConnector endPlug;

		public override Vector3[] GetHandlePositions()
		{
			List<Vector3> list = new List<Vector3>();
			if (startPlug != null)
			{
				list.Add(startPlug.ropeFixPoint.position);
				list.Add(startPlug.ropeFixPoint.position - 2f * startPlug.alignmentTransform.forward);
			}
			for (int i = 0; i < handles.Length; i++)
			{
				list.Add(handles[i].position);
			}
			if (endPlug != null)
			{
				list.Add(endPlug.ropeFixPoint.position - 2f * endPlug.alignmentTransform.forward);
				list.Add(endPlug.ropeFixPoint.position);
			}
			return list.ToArray();
		}

		public override void OnEnable()
		{
			if (startPlug != null)
			{
				startBody = startPlug.GetComponent<Rigidbody>();
				fixStart = (fixStartDir = true);
			}
			if (endPlug != null)
			{
				endBody = endPlug.GetComponent<Rigidbody>();
				fixEnd = (fixEndDir = true);
			}
			base.OnEnable();
			if (startPlug != null)
			{
				startPlug.grablist = new GameObject[3]
				{
					startPlug.gameObject,
					bones[0].gameObject,
					bones[1].gameObject
				};
			}
			if (endPlug != null)
			{
				endPlug.grablist = new GameObject[3]
				{
					endPlug.gameObject,
					bones[bones.Length - 1].gameObject,
					bones[bones.Length - 2].gameObject
				};
			}
		}
	}
}
