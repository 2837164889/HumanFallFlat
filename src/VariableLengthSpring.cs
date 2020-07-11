using UnityEngine;

public class VariableLengthSpring : MonoBehaviour
{
	private void FixedUpdate()
	{
		ConfigurableJoint[] componentsInChildren = GetComponentsInChildren<ConfigurableJoint>();
		ConfigurableJoint[] array = componentsInChildren;
		foreach (ConfigurableJoint configurableJoint in array)
		{
			configurableJoint.linearLimit = new SoftJointLimit
			{
				limit = Mathf.Sin(Time.time) * 1.5f + 1.5f
			};
		}
	}
}
