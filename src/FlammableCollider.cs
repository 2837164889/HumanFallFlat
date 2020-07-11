using UnityEngine;

public class FlammableCollider : MonoBehaviour
{
	private Flammable flammable;

	private void OnEnable()
	{
		flammable = GetComponentInParent<Flammable>();
	}

	public void OnTriggerEnter(Collider other)
	{
		Flame component = other.gameObject.GetComponent<Flame>();
		if (component != null)
		{
			flammable.AddFlame(component);
		}
		Flammable component2 = other.gameObject.GetComponent<Flammable>();
		if (component2 != null)
		{
			component2.AddFlammable(flammable);
		}
		FlammableExtinguisher component3 = other.gameObject.GetComponent<FlammableExtinguisher>();
		if (component3 != null)
		{
			flammable.AddExtinguisher(component3);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Flame component = other.gameObject.GetComponent<Flame>();
		if (component != null)
		{
			flammable.RemoveFlame(component);
		}
		Flammable component2 = other.gameObject.GetComponent<Flammable>();
		if (component2 != null)
		{
			component2.RemoveFlammable(flammable);
		}
		FlammableExtinguisher component3 = other.gameObject.GetComponent<FlammableExtinguisher>();
		if (component3 != null)
		{
			flammable.RemoveExtinguisher(component3);
		}
	}
}
