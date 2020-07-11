namespace HumanAPI
{
	public abstract class SteamPort : Node
	{
		public SteamNode node;

		public abstract SteamPort connectedPort
		{
			get;
		}

		public abstract bool isOpen
		{
			get;
		}

		public abstract float ownPressure
		{
			get;
		}

		public abstract void ApplySystemState(bool isOpenSystem, float pressure);

		protected override void OnEnable()
		{
			base.OnEnable();
			if (node != null && !node.ports.Contains(this))
			{
				node.ports.Add(this);
				SteamSystem.Recalculate(node);
			}
		}
	}
}
