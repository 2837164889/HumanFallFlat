using HumanAPI;
using UnityEngine;

public class Telescope : Node
{
	public Light rayLight;

	public Light bounceLight;

	public Light sun;

	public float intensityPower;

	public NodeInput focus;

	private float rayLightintensity;

	private float bounceLightintensity;

	public void Awake()
	{
		rayLightintensity = rayLight.intensity;
		bounceLightintensity = bounceLight.intensity;
	}

	private void Update()
	{
		bool flag = Physics.Raycast(base.transform.position, base.transform.forward);
		bool flag2 = Physics.Raycast(base.transform.position, -sun.transform.forward);
		float num = 0f - Vector3.Dot(base.transform.forward, sun.transform.forward);
		float num2 = Mathf.Pow(num, intensityPower);
		float num3 = 0f;
		if (!flag2)
		{
			num3 += num2;
		}
		num3 *= Mathf.Lerp(0.1f, 1f, Mathf.InverseLerp(0.99f, 0.997f, num));
		if (!flag)
		{
			num3 += 0.1f;
		}
		if (num3 > 0f)
		{
			RaycastHit hitInfo;
			bool flag3 = Physics.Raycast(rayLight.transform.position - base.transform.forward * 3f, -base.transform.forward, out hitInfo);
			bounceLight.transform.position = hitInfo.point + base.transform.forward * 0.2f;
			float num4 = Mathf.InverseLerp(-1f, 1f, focus.value);
			num3 *= Mathf.Lerp(0.2f, 1f, num4 * num4);
			rayLight.spotAngle = Mathf.Lerp(20f, 5f, num4 * num4);
		}
		if (rayLight.intensity != rayLightintensity * num3)
		{
			rayLight.intensity = rayLightintensity * num3;
		}
		if (bounceLight.intensity != bounceLightintensity * num3)
		{
			bounceLight.intensity = bounceLightintensity * num3;
		}
	}
}
