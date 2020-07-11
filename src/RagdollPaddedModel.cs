using UnityEngine;

public class RagdollPaddedModel : MonoBehaviour
{
	private Renderer[] renderers;

	public void CreatePaddedMesh(Renderer[] modelRenderers)
	{
		renderers = new Renderer[modelRenderers.Length];
		for (int i = 0; i < modelRenderers.Length; i++)
		{
			Mesh mesh;
			if (modelRenderers[i] is SkinnedMeshRenderer)
			{
				mesh = new Mesh();
				(modelRenderers[i] as SkinnedMeshRenderer).BakeMesh(mesh);
			}
			else
			{
				mesh = modelRenderers[i].GetComponent<MeshFilter>().sharedMesh;
			}
			mesh = BuildPaddedMesh.GeneratePadded(mesh, 0.00390625f);
			GameObject gameObject = new GameObject("UnwrapMesh");
			gameObject.transform.SetParent(modelRenderers[i].transform, worldPositionStays: false);
			gameObject.layer = 31;
			gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
			renderers[i] = gameObject.AddComponent<MeshRenderer>();
			renderers[i].enabled = false;
		}
	}

	public void Teardown()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			Object.Destroy(renderers[i].gameObject);
		}
		renderers = null;
	}

	public void Enable(bool enable)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = enable;
		}
	}

	public void SetMaterial(Material material)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sharedMaterial = material;
		}
	}
}
