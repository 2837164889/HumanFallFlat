using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class iTweenGraphic : MonoBehaviour
{
	private Graphic graphic;

	private void OnEnable()
	{
		graphic = GetComponent<Graphic>();
	}

	public void iTweenOnUpdateColor(Color value)
	{
		graphic.color = value;
	}

	public static void ColorTo(GameObject go, Color color, float duration)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("from", go.GetComponent<Graphic>().color);
		hashtable.Add("to", color);
		hashtable.Add("time", duration);
		hashtable.Add("onupdate", "iTweenOnUpdateColor");
		iTween.ValueTo(go, hashtable);
	}
}
