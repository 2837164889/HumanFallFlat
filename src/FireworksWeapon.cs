using UnityEngine;

public class FireworksWeapon : MonoBehaviour
{
	public Transform muzzle;

	private float cooldown;

	public void Place(Vector3 pos, Quaternion dir)
	{
		base.transform.position = pos;
		base.transform.rotation = dir;
	}

	public bool CanShoot()
	{
		return cooldown < Time.time;
	}

	public void Shoot()
	{
		FireworksProjectile projectile = Fireworks.instance.GetProjectile();
		if (projectile != null)
		{
			projectile.Shoot(muzzle);
			cooldown = Time.time + 1f;
		}
	}
}
