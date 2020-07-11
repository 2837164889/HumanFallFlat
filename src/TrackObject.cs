using Multiplayer;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
	[SerializeField]
	private GameObject objectToTrack;

	private void FixedUpdate()
	{
		if (!NetGame.isClient && objectToTrack != null)
		{
			base.transform.position = objectToTrack.transform.position;
			base.transform.rotation = objectToTrack.transform.rotation;
		}
	}
}
