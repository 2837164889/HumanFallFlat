using Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class Fireworks : NetScope
{
	public bool enableWeapons;

	public FireworksProjectile prefab;

	public FireworksWeapon weaponPrefab;

	public static Fireworks instance;

	private Queue<FireworksProjectile> queue = new Queue<FireworksProjectile>();

	private List<FireworksWeapon> weapons = new List<FireworksWeapon>();

	private List<FireworksProjectile> projectiles = new List<FireworksProjectile>();

	private float timer;

	protected override void OnEnable()
	{
		instance = this;
		weaponPrefab.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		instance = null;
	}

	private void Start()
	{
		NetGame.instance.preUpdate += OnPreFixedUpdate;
		queue.Enqueue(prefab);
		prefab.GetComponent<NetIdentity>().sceneId = 0u;
		for (int i = 1; i < 20; i++)
		{
			FireworksProjectile fireworksProjectile = Object.Instantiate(prefab, Vector3.down * 200f, Quaternion.identity, base.transform);
			fireworksProjectile.GetComponent<NetIdentity>().sceneId = (uint)i;
			queue.Enqueue(fireworksProjectile);
		}
		StartNetwork();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		NetGame.instance.preUpdate -= OnPreFixedUpdate;
	}

	internal void Kill(FireworksProjectile projectile)
	{
		if (projectiles.Contains(projectile))
		{
			projectiles.Remove(projectile);
			projectile.transform.position = Vector3.down * 200f;
			queue.Enqueue(projectile);
		}
	}

	public void MarkUsed(FireworksProjectile projectile)
	{
		if (!projectiles.Contains(projectile))
		{
			projectiles.Add(projectile);
		}
	}

	public FireworksProjectile GetProjectile()
	{
		while (queue.Count > 0)
		{
			FireworksProjectile fireworksProjectile = queue.Dequeue();
			if (!projectiles.Contains(fireworksProjectile))
			{
				projectiles.Add(fireworksProjectile);
				return fireworksProjectile;
			}
		}
		return null;
	}

	public void OnPreFixedUpdate()
	{
		if (NetGame.isClient || ReplayRecorder.isPlaying)
		{
			return;
		}
		for (int num = projectiles.Count - 1; num >= 0; num--)
		{
			if (!projectiles[num].isActiveAndEnabled)
			{
				Kill(projectiles[num]);
			}
		}
		SyncWeapons(fire: true);
	}

	private void SyncWeapons(bool fire)
	{
		int num = enableWeapons ? Human.all.Count : 0;
		while (weapons.Count > num)
		{
			Object.Destroy(weapons[0].gameObject);
			weapons.RemoveAt(0);
		}
		while (weapons.Count < num)
		{
			FireworksWeapon fireworksWeapon = Object.Instantiate(weaponPrefab, base.transform);
			fireworksWeapon.gameObject.SetActive(value: true);
			weapons.Add(fireworksWeapon);
		}
		for (int i = 0; i < num; i++)
		{
			Human human = Human.all[i];
			FireworksWeapon fireworksWeapon2 = weapons[i];
			if (human.targetDirection != Vector3.zero)
			{
				fireworksWeapon2.Place(human.ragdoll.partRightHand.transform.position, Quaternion.LookRotation(human.targetDirection));
			}
			if (fire)
			{
				bool rightGrab = human.controls.rightGrab;
				bool shootingFirework = human.controls.shootingFirework;
				human.controls.rightExtend = (rightGrab ? 1f : ((human.state != HumanState.Climb && human.state != 0 && human.state != HumanState.Jump && human.state != HumanState.Slide && human.state != HumanState.Walk) ? 0f : 0.95f));
				human.controls.rightGrab = rightGrab;
				if (!rightGrab)
				{
					human.ragdoll.partRightHand.sensor.ReleaseGrab(Time.fixedDeltaTime * 3f);
				}
				if (shootingFirework && fireworksWeapon2.CanShoot())
				{
					fireworksWeapon2.Shoot();
				}
			}
		}
	}

	private void LateUpdate()
	{
		SyncWeapons(fire: false);
	}

	public void ShootFirework()
	{
		FireworksProjectile projectile = instance.GetProjectile();
		if (projectile != null)
		{
			projectile.Shoot(base.transform.position, 2f * Vector3.up + Random.insideUnitSphere);
		}
	}
}
