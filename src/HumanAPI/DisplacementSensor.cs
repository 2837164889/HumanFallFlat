using UnityEngine;

namespace HumanAPI
{
	public class DisplacementSensor : Node
	{
		public NodeOutput value;

		public Transform relativeTo;

		private Vector3 startPos;

		private void Awake()
		{
			startPos = base.transform.position;
			if (relativeTo != null)
			{
				startPos = relativeTo.InverseTransformPoint(startPos);
			}
		}

		private void FixedUpdate()
		{
			if ((bool)relativeTo)
			{
				value.SetValue((relativeTo.TransformPoint(startPos) - base.transform.position).magnitude);
			}
			else
			{
				value.SetValue((startPos - base.transform.position).magnitude);
			}
		}
	}
}
