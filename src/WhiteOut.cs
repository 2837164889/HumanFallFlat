using UnityEngine;

public sealed class WhiteOut : MonoBehaviour
{
	public Shader shaderWhiteOut;

	private Material materialWhiteOut;

	private float mWhiteProportion = 1f;

	public void SetWhiteProportion(float proportion)
	{
		mWhiteProportion = proportion;
	}

	public bool CheckResources()
	{
		if (materialWhiteOut == null)
		{
			materialWhiteOut = new Material(shaderWhiteOut);
			materialWhiteOut.hideFlags = HideFlags.DontSave;
		}
		return true;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (CheckResources() && materialWhiteOut != null)
		{
			materialWhiteOut.SetFloat("_WhiteProportion", mWhiteProportion);
			Graphics.Blit(source, destination, materialWhiteOut);
		}
	}
}
