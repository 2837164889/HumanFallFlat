using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	public class Wire : Rope, ICircuitComponent
	{
		public PowerPlug startPlug;

		public PowerPlug endPlug;

		public float current
		{
			get;
			set;
		}

		public CircuitConnector forwardConnector => startPlug;

		public CircuitConnector reverseConnector => endPlug;

		public bool isOpen => false;

		public float CalculateVoltage(float I)
		{
			return 0f;
		}

		public void RunCurrent(float I)
		{
			current = I;
		}

		public void StopCurrent()
		{
			current = 0f;
		}

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
				startPlug.parent = this;
				startPlug.isForward = true;
			}
			if (endPlug != null)
			{
				endBody = endPlug.GetComponent<Rigidbody>();
				fixEnd = (fixEndDir = true);
				endPlug.parent = this;
				endPlug.isForward = false;
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
