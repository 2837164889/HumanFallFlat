using System.Collections;
using UnityEngine;

public class AnimateHumanLogo : MonoBehaviour
{
	public float animationTime = 0.1f;

	public IEnumerator DropAnimation()
	{
		float time = 0f;
		while (time < animationTime)
		{
			base.transform.localPosition = base.transform.localPosition.SetZ(Mathf.Lerp(-1500f, 0f, time / animationTime));
			time += Time.deltaTime;
			yield return null;
		}
		base.transform.localPosition = base.transform.localPosition.SetZ(Mathf.Lerp(-1500f, 0f, time / animationTime));
	}
}
