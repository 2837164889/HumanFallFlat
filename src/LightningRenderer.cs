using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LightningRenderer : MonoBehaviour
{
	private LineRenderer line;

	public int points = 60;

	public float step = 1f;

	public float scaleX = 1f;

	public float scaleY = 1f;

	public float scaleZ = 1f;

	public float perlinScale = 1f;

	public float perlinScroll = 0.1f;

	public float randomSkipMin = 0.01f;

	public float randomSkipMax = 0.1f;

	public int disableAfterSkips;

	private int disableCounter;

	public bool anchorEndPoint;

	private float perlinY;

	private float skipCount;

	private float currentSkip;

	public Vector3 tendrilAngleRange = new Vector3(30f, 30f, 30f);

	public float tendrilStart = 0.5f;

	public float tendrilEnd = 0.9f;

	public LightningRenderer[] tendrils;

	private int[] tendrilAttachPoints;

	private float widthMultiplier;

	private LightningRenderer parentLightning;

	private float GetAlpha()
	{
		float num = (randomSkipMax == 0f) ? 1f : (widthMultiplier * ((currentSkip - skipCount) / currentSkip));
		if (parentLightning != null)
		{
			num *= parentLightning.GetAlpha();
		}
		return num;
	}

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		line.positionCount = points;
		line.useWorldSpace = false;
		perlinY = Random.Range(0f, 2000f);
		tendrilAttachPoints = new int[tendrils.Length];
		widthMultiplier = line.widthMultiplier;
		RandomiseTendrils(skipChildren: false);
	}

	private void OnEnable()
	{
		disableCounter = 0;
	}

	private void SkipAhead()
	{
		currentSkip = Random.Range(randomSkipMin, randomSkipMax);
		perlinY += Random.Range(1f, 10f);
		skipCount = 0f;
		disableCounter++;
		if (disableCounter == disableAfterSkips)
		{
			base.gameObject.SetActive(value: false);
		}
		RandomiseTendrils(skipChildren: true);
	}

	private void Update()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < points; i++)
		{
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			if (i != 0 && (!anchorEndPoint || i != points - 1))
			{
				num4 = ((!anchorEndPoint) ? num : 0f) + Mathf.PerlinNoise((float)i * perlinScale, perlinY) * 2f - 1f;
				num5 = ((!anchorEndPoint) ? num2 : 0f) + Mathf.PerlinNoise((float)i * perlinScale, perlinY + 17f) * 2f - 1f;
				num6 = ((!anchorEndPoint) ? num3 : 0f) + Mathf.PerlinNoise((float)i * perlinScale, perlinY + 27f) * 2f - 1f;
			}
			num = num4;
			num2 = num5;
			line.SetPosition(i, new Vector3(num4 * scaleX, num5 * scaleY, num6 * scaleZ + (float)i * step));
		}
		line.widthMultiplier = GetAlpha();
		perlinY += perlinScroll * Time.deltaTime;
		skipCount += Time.deltaTime;
		if (randomSkipMax != 0f && skipCount > currentSkip)
		{
			SkipAhead();
		}
		UpdateTendrils();
	}

	private void UpdateTendrils()
	{
		for (int i = 0; i < tendrils.Length; i++)
		{
			Vector3 position = line.GetPosition(tendrilAttachPoints[i]);
			position = base.transform.TransformPoint(position);
			tendrils[i].transform.position = position;
			tendrils[i].UpdateTendrils();
		}
	}

	private void RandomiseTendrils(bool skipChildren)
	{
		for (int i = 0; i < tendrils.Length; i++)
		{
			tendrilAttachPoints[i] = Mathf.Clamp((int)Random.Range(tendrilStart * (float)(points - 1), tendrilEnd * (float)(points - 1)), 0, points - 1);
			Quaternion rotation = base.transform.rotation;
			rotation *= Quaternion.Euler(Random.Range(0f - tendrilAngleRange.x, tendrilAngleRange.x), Random.Range(0f - tendrilAngleRange.y, tendrilAngleRange.y), Random.Range(0f - tendrilAngleRange.z, tendrilAngleRange.z));
			tendrils[i].transform.rotation = rotation;
			tendrils[i].parentLightning = this;
			if (skipChildren)
			{
				tendrils[i].SkipAhead();
			}
		}
	}
}
