using UnityEngine;

public class FlammableScriptBoilerCollider1 : MonoBehaviour
{
	[Tooltip("Use this in order to show the prints coming from the script")]
	public bool showDebug;

	private FlammableSourceBoiler1 boiler;

	private void OnEnable()
	{
		boiler = GetComponentInParent<FlammableSourceBoiler1>();
	}

	public void OnTriggerEnter(Collider boilerFuel)
	{
		Flammable component = boilerFuel.gameObject.GetComponent<Flammable>();
		if (component != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Flammable thing hit boiler collision ");
			}
			boiler.AddFuel(component);
		}
		Flame component2 = boilerFuel.gameObject.GetComponent<Flame>();
		if (component2 != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Flame thing hit boiler collision ");
			}
			boiler.AddFlame(component2);
		}
	}

	public void OnTriggerExit(Collider fuel)
	{
		Flammable component = fuel.gameObject.GetComponent<Flammable>();
		if (component != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Removing fuel ");
			}
			boiler.RemoveFuel(component);
		}
		Flame component2 = fuel.gameObject.GetComponent<Flame>();
		if (component2 != null)
		{
			if (showDebug)
			{
				Debug.Log(base.name + " Removing flame ");
			}
			boiler.RemoveFlame(component2);
		}
	}
}
