using UnityEngine;

public class Shaders : MonoBehaviour
{
	public static Shaders instance;

	public Shader opaqueHumanShader;

	public Shader transparentHumanShader;

	public Shader transparentHumanShaderMetal;

	public Shader applyMaskedColors;

	public Shader paintShader;

	public Shader webcamShader;

	public Shader showMaskShader;

	public Shader customizeUnlitShader;

	private void Awake()
	{
		instance = this;
	}
}
