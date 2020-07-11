using UnityEngine;

namespace HumanAPI
{
	public class CollisionSound : MonoBehaviour, IReset
	{
		public bool acceptPlayer;

		public Collider[] acceptColliders;

		public float minImpulse;

		public float maxImpulse;

		public float minVolume;

		public float minDelay = 0.2f;

		private float lastSoundTime;

		private Sound2 sound;

		private void Awake()
		{
			sound = GetComponentInChildren<Sound2>();
		}

		private void OnCollisionEnter(Collision collision)
		{
			float magnitude = collision.impulse.magnitude;
			if (magnitude < minImpulse)
			{
				return;
			}
			if (acceptPlayer)
			{
				if (collision.collider.GetComponentInParent<Human>() == null)
				{
					return;
				}
			}
			else
			{
				if (acceptColliders.Length <= 0 || !(collision.collider.tag != "Player"))
				{
					return;
				}
				bool flag = false;
				for (int i = 0; i < acceptColliders.Length; i++)
				{
					if (acceptColliders[i] == collision.collider)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return;
				}
			}
			float time = Time.time;
			if (!(lastSoundTime + minDelay > time))
			{
				float num = Mathf.InverseLerp(minImpulse, maxImpulse, magnitude);
				if (!(num < minVolume))
				{
					sound.PlayOneShot(num);
					lastSoundTime = time;
				}
			}
		}

		public void ResetState(int checkpoint, int subObjectives)
		{
			lastSoundTime = 0f;
		}
	}
}
