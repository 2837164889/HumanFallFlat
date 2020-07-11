using ProBuilder2.Common;
using ProBuilder2.MeshOperations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtrudeRandomEdges : MonoBehaviour
{
	private pb_Object pb;

	private pb_Face lastExtrudedFace;

	public float distance = 1f;

	private void Start()
	{
		pb = pb_ShapeGenerator.PlaneGenerator(1f, 1f, 0, 0, Axis.Up, smooth: false);
		pb.SetFaceMaterial(pb.faces, pb_Constant.DefaultMaterial);
		lastExtrudedFace = pb.faces[0];
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Extrude Random Edge"))
		{
			ExtrudeEdge();
		}
	}

	private void ExtrudeEdge()
	{
		pb_Face sourceFace = lastExtrudedFace;
		List<pb_WingedEdge> wingedEdges = pb_WingedEdge.GetWingedEdges(pb);
		IEnumerable<pb_WingedEdge> source = wingedEdges.Where((pb_WingedEdge x) => x.face == sourceFace);
		List<pb_Edge> list = (from x in source
			where x.opposite == null
			select x into y
			select y.edge.local).ToList();
		int index = Random.Range(0, list.Count);
		pb_Edge pb_Edge = list[index];
		Vector3 a = (pb.vertices[pb_Edge.x] + pb.vertices[pb_Edge.y]) * 0.5f - sourceFace.distinctIndices.Average((int x) => pb.vertices[x]);
		a.Normalize();
		pb.Extrude(new pb_Edge[1]
		{
			pb_Edge
		}, 0f, extrudeAsGroup: false, enableManifoldExtrude: true, out pb_Edge[] extrudedEdges);
		lastExtrudedFace = pb.faces.Last();
		pb.SetSelectedEdges(extrudedEdges);
		pb.TranslateVertices(pb.SelectedTriangles, a * distance);
		pb.ToMesh();
		pb.Refresh();
	}
}
