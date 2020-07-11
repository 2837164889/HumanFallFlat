using Multiplayer;
using UnityEngine;

public class CementBagSegment : MonoBehaviour
{
	private CementBag bag;

	private uint evtCollision;

	private NetIdentity identity;

	private void OnEnable()
	{
		bag = GetComponentInParent<CementBag>();
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length != 0 && bag.ReportCollision(collision.impulse.magnitude, collision.contacts[0].point) && (NetGame.isServer || ReplayRecorder.isRecording))
		{
			NetStream netStream = identity.BeginEvent(evtCollision);
			int x = NetFloat.Quantize(collision.impulse.magnitude, 10000f, 11);
			netStream.Write(x, 10);
			Vector3 vec = collision.contacts[0].point - base.transform.position;
			NetVector3.Quantize(vec, 100f, 10).Write(netStream, 3);
			identity.EndEvent();
		}
	}

	public void OnCollisionStay(Collision collision)
	{
		OnCollisionEnter(collision);
	}

	public void Start()
	{
		identity = GetComponentInParent<NetIdentity>();
		if (identity != null)
		{
			evtCollision = identity.RegisterEvent(OnReportCollision);
		}
	}

	private void OnReportCollision(NetStream stream)
	{
		float impulse = NetFloat.Dequantize(stream.ReadInt32(10), 10000f, 11);
		Vector3 b = NetVector3.Read(stream, 3, 10).Dequantize(100f);
		Vector3 pos = base.transform.position + b;
		bag.ReportCollision(impulse, pos);
	}
}
