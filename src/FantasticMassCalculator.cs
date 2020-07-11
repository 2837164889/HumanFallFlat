using UnityEngine;

public class FantasticMassCalculator : MonoBehaviour
{
	private enum Material
	{
		Custom = 0,
		SoftWood_25 = 25,
		HardWood_110 = 110,
		Plastic_90 = 90,
		Tin_200 = 200,
		Iron_785 = 785,
		Ice_930 = 930,
		Water_997 = 997
	}

	[SerializeField]
	private Material material;

	[SerializeField]
	private int customDensity;

	private void Start()
	{
		if ((bool)GetComponent<Rigidbody>())
		{
			if (material == Material.Custom)
			{
				GetComponent<Rigidbody>().SetDensity((float)customDensity * 0.01f);
			}
			else
			{
				GetComponent<Rigidbody>().SetDensity((float)material * 0.01f);
			}
			GetComponent<Rigidbody>().mass = GetComponent<Rigidbody>().mass;
		}
		else
		{
			Debug.Log("Can't set material density on " + base.gameObject.name, base.gameObject);
		}
	}

	private void Update()
	{
	}
}
