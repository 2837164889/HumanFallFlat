namespace HumanAPI
{
	public class GrabSensor : Node, IGrabbable
	{
		public NodeOutput output;

		public void OnGrab()
		{
			output.SetValue(1f);
		}

		public void OnRelease()
		{
			output.SetValue(0f);
		}
	}
}
