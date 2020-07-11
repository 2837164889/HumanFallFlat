using ProBuilder2.Common;
using UnityEngine;

namespace ProBuilder2.Examples
{
	public class RuntimeEdit : MonoBehaviour
	{
		private class pb_Selection
		{
			public pb_Object pb;

			public pb_Face face;

			public pb_Selection(pb_Object _pb, pb_Face _face)
			{
				pb = _pb;
				face = _face;
			}

			public bool HasObject()
			{
				return pb != null;
			}

			public bool IsValid()
			{
				return pb != null && face != null;
			}

			public bool Equals(pb_Selection sel)
			{
				if (sel != null && sel.IsValid())
				{
					return pb == sel.pb && face == sel.face;
				}
				return false;
			}

			public void Destroy()
			{
				if (pb != null)
				{
					Object.Destroy(pb.gameObject);
				}
			}

			public override string ToString()
			{
				return ("pb_Object: " + pb != null) ? (pb.name + "\npb_Face: " + ((face != null) ? face.ToString() : "Null")) : "Null";
			}
		}

		private pb_Selection currentSelection;

		private pb_Selection previousSelection;

		private pb_Object preview;

		public Material previewMaterial;

		private Vector2 mousePosition_initial = Vector2.zero;

		private bool dragging;

		public float rotateSpeed = 100f;

		private void Awake()
		{
			SpawnCube();
		}

		private void OnGUI()
		{
			if (GUI.Button(new Rect(5f, Screen.height - 25, 80f, 20f), "Reset"))
			{
				currentSelection.Destroy();
				Object.Destroy(preview.gameObject);
				SpawnCube();
			}
		}

		private void SpawnCube()
		{
			pb_Object pb_Object = pb_ShapeGenerator.CubeGenerator(Vector3.one);
			pb_Object.gameObject.AddComponent<MeshCollider>().convex = false;
			currentSelection = new pb_Selection(pb_Object, null);
		}

		public void LateUpdate()
		{
			if (!currentSelection.HasObject())
			{
				return;
			}
			if (Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt)))
			{
				mousePosition_initial = Input.mousePosition;
				dragging = true;
			}
			if (dragging)
			{
				Vector2 vector = (Vector3)mousePosition_initial - Input.mousePosition;
				Vector3 axis = new Vector3(vector.y, vector.x, 0f);
				currentSelection.pb.gameObject.transform.RotateAround(Vector3.zero, axis, rotateSpeed * Time.deltaTime);
				if (currentSelection.IsValid())
				{
					RefreshSelectedFacePreview();
				}
			}
			if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
			{
				dragging = false;
			}
		}

		public void Update()
		{
			if (!Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.LeftAlt) || !FaceCheck(Input.mousePosition) || !currentSelection.IsValid())
			{
				return;
			}
			if (!currentSelection.Equals(previousSelection))
			{
				previousSelection = new pb_Selection(currentSelection.pb, currentSelection.face);
				RefreshSelectedFacePreview();
				return;
			}
			Vector3 vector = pb_Math.Normal(currentSelection.pb.vertices.ValuesWithIndices(currentSelection.face.distinctIndices));
			if (Input.GetKey(KeyCode.LeftShift))
			{
				currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices, vector.normalized * -0.5f);
			}
			else
			{
				currentSelection.pb.TranslateVertices(currentSelection.face.distinctIndices, vector.normalized * 0.5f);
			}
			currentSelection.pb.Refresh();
			RefreshSelectedFacePreview();
		}

		public bool FaceCheck(Vector3 pos)
		{
			Ray ray = Camera.main.ScreenPointToRay(pos);
			if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInfo))
			{
				pb_Object component = hitInfo.transform.gameObject.GetComponent<pb_Object>();
				if (component == null)
				{
					return false;
				}
				Mesh msh = component.msh;
				int[] tri = new int[3]
				{
					msh.triangles[hitInfo.triangleIndex * 3],
					msh.triangles[hitInfo.triangleIndex * 3 + 1],
					msh.triangles[hitInfo.triangleIndex * 3 + 2]
				};
				currentSelection.pb = component;
				return component.FaceWithTriangle(tri, out currentSelection.face);
			}
			return false;
		}

		private void RefreshSelectedFacePreview()
		{
			Vector3[] array = currentSelection.pb.VerticesInWorldSpace(currentSelection.face.indices);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = i;
			}
			Vector3 vector = pb_Math.Normal(array);
			for (int j = 0; j < array.Length; j++)
			{
				array[j] += vector.normalized * 0.01f;
			}
			if ((bool)preview)
			{
				Object.Destroy(preview.gameObject);
			}
			preview = pb_Object.CreateInstanceWithVerticesFaces(array, new pb_Face[1]
			{
				new pb_Face(array2)
			});
			preview.SetFaceMaterial(preview.faces, previewMaterial);
			preview.ToMesh();
			preview.Refresh();
		}
	}
}
