using Multiplayer;
using System.Collections;
using UnityEngine;

public class GiftHole : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (NetGame.isClient || ReplayRecorder.isPlaying)
		{
			return;
		}
		GiftBase giftBase = other.GetComponent<GiftBase>();
		if (giftBase == null)
		{
			CapsuleCollider component = other.GetComponent<CapsuleCollider>();
			if (component != null && component.height > 3f)
			{
				giftBase = other.GetComponentInParent<GiftBase>();
			}
		}
		if (giftBase != null)
		{
			if (GiftService.instance != null)
			{
				GiftService.instance.DeliverGift(giftBase.userId, giftBase.giftId);
			}
			if (giftBase is BigGift)
			{
				StartCoroutine(BigGiftChutes());
			}
			else
			{
				Fireworks.instance.ShootFirework();
			}
		}
	}

	private IEnumerator BigGiftChutes()
	{
		for (int i = 0; i < 10; i++)
		{
			Fireworks.instance.ShootFirework();
			yield return new WaitForSeconds(Random.Range(0.05f, 1f));
		}
	}
}
