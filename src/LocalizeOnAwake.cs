using I2.Loc;
using UnityEngine;

public class LocalizeOnAwake : MonoBehaviour
{
	public string term;

	[SerializeField]
	public LocalizeEvent onLocalize;

	public void Awake()
	{
		Localize();
	}

	public void Localize()
	{
		onLocalize.Invoke(ScriptLocalization.Get(term));
	}
}
