using UnityEngine;

public class BoilerCollider : MonoBehaviour
{
	private Boiler boiler;

	private void OnEnable()
	{
		boiler = GetComponentInParent<Boiler>();
	}

	public void OnTriggerEnter(Collider other)
	{
		Coal component = other.gameObject.GetComponent<Coal>();
		if (component != null)
		{
			boiler.AddCoal(component);
		}
		Flame component2 = other.gameObject.GetComponent<Flame>();
		if (component2 != null)
		{
			boiler.AddFlame(component2);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Coal component = other.gameObject.GetComponent<Coal>();
		if (component != null)
		{
			boiler.RemoveCoal(component);
		}
		Flame component2 = other.gameObject.GetComponent<Flame>();
		if (component2 != null)
		{
			boiler.RemoveFlame(component2);
		}
	}
}
