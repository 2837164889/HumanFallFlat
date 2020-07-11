using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanAPI
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class WallVolume : MonoBehaviour
	{
		public float gridSize = 0.5f;

		public List<WallVolumeFace> faces = new List<WallVolumeFace>();

		private static Vector3[] vertices;

		private static Vector3[] normals;

		private static int[] tris;

		[NonSerialized]
		public Vector3 gizmoStart;

		[NonSerialized]
		public Vector3 gizmoOffset;

		[NonSerialized]
		public Vector3 gizmoSize;

		[NonSerialized]
		public Color gizmoColor;

		private void Resize<T>(ref T[] array, int minSize, int desiredSize)
		{
			int num = (array == null) ? minSize : array.Length;
			int num2;
			for (num2 = num; num2 < desiredSize; num2 *= 2)
			{
			}
			while (num2 > minSize && num2 / 2 > desiredSize)
			{
				num2 /= 2;
			}
			if (num2 != num || array == null)
			{
				array = new T[num2];
			}
		}

		public void FillMesh(Mesh mesh, bool forceExact)
		{
			if (forceExact)
			{
				vertices = new Vector3[faces.Count * 4];
				normals = new Vector3[faces.Count * 4];
				tris = new int[faces.Count * 2 * 6];
			}
			else
			{
				Resize(ref vertices, 64, faces.Count * 4);
				Resize(ref normals, 64, faces.Count * 4);
				Resize(ref tris, 96, faces.Count * 6);
			}
			for (int i = 0; i < faces.Count; i++)
			{
				WallVolumeFace wallVolumeFace = faces[i];
				Vector3 a;
				Vector3 vector;
				Vector3 b;
				Vector3 b2;
				switch (wallVolumeFace.orientation)
				{
				case WallVolumeOrientaion.Back:
					a = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ);
					vector = Vector3.back;
					b = Vector3.up;
					b2 = Vector3.right;
					break;
				case WallVolumeOrientaion.Right:
					a = new Vector3(wallVolumeFace.posX + 1, wallVolumeFace.posY, wallVolumeFace.posZ);
					vector = Vector3.right;
					b = Vector3.up;
					b2 = Vector3.forward;
					break;
				case WallVolumeOrientaion.Forward:
					a = new Vector3(wallVolumeFace.posX + 1, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
					vector = Vector3.forward;
					b = Vector3.up;
					b2 = Vector3.left;
					break;
				case WallVolumeOrientaion.Left:
					a = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
					vector = Vector3.left;
					b = Vector3.up;
					b2 = Vector3.back;
					break;
				case WallVolumeOrientaion.Down:
					a = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY, wallVolumeFace.posZ + 1);
					vector = Vector3.down;
					b = Vector3.back;
					b2 = Vector3.right;
					break;
				case WallVolumeOrientaion.Up:
					a = new Vector3(wallVolumeFace.posX, wallVolumeFace.posY + 1, wallVolumeFace.posZ);
					vector = Vector3.up;
					b = Vector3.forward;
					b2 = Vector3.right;
					break;
				default:
					throw new InvalidOperationException();
				}
				vertices[i * 4] = a * gridSize;
				vertices[i * 4 + 1] = (a + b) * gridSize;
				vertices[i * 4 + 2] = (a + b2 + b) * gridSize;
				vertices[i * 4 + 3] = (a + b2) * gridSize;
				normals[i * 4] = vector;
				normals[i * 4 + 1] = vector;
				normals[i * 4 + 2] = vector;
				normals[i * 4 + 3] = vector;
				tris[i * 6] = i * 4;
				tris[i * 6 + 1] = i * 4 + 1;
				tris[i * 6 + 2] = i * 4 + 2;
				tris[i * 6 + 3] = i * 4;
				tris[i * 6 + 4] = i * 4 + 2;
				tris[i * 6 + 5] = i * 4 + 3;
			}
			for (int j = faces.Count * 6; j < tris.Length; j++)
			{
				tris[j] = 0;
			}
			mesh.name = base.name;
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = tris;
			mesh.RecalculateBounds();
		}

		private bool IsOpen(int x, int y, int z, WallVolumeOrientaion orientation)
		{
			bool flag = false;
			switch (orientation)
			{
			case WallVolumeOrientaion.Up:
			{
				for (int l = 0; l < faces.Count; l++)
				{
					WallVolumeFace wallVolumeFace16 = faces[l];
					if (wallVolumeFace16.posX != x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace17 = faces[l];
					if (wallVolumeFace17.posZ != z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace18 = faces[l];
					if (wallVolumeFace18.posY <= y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace19 = faces[l];
					if (wallVolumeFace19.orientation != 0)
					{
						WallVolumeFace wallVolumeFace20 = faces[l];
						if (wallVolumeFace20.orientation != WallVolumeOrientaion.Down)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			case WallVolumeOrientaion.Down:
			{
				for (int n = 0; n < faces.Count; n++)
				{
					WallVolumeFace wallVolumeFace26 = faces[n];
					if (wallVolumeFace26.posX != x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace27 = faces[n];
					if (wallVolumeFace27.posZ != z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace28 = faces[n];
					if (wallVolumeFace28.posY >= y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace29 = faces[n];
					if (wallVolumeFace29.orientation != 0)
					{
						WallVolumeFace wallVolumeFace30 = faces[n];
						if (wallVolumeFace30.orientation != WallVolumeOrientaion.Down)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			case WallVolumeOrientaion.Left:
			{
				for (int j = 0; j < faces.Count; j++)
				{
					WallVolumeFace wallVolumeFace6 = faces[j];
					if (wallVolumeFace6.posY != y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace7 = faces[j];
					if (wallVolumeFace7.posZ != z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace8 = faces[j];
					if (wallVolumeFace8.posX >= x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace9 = faces[j];
					if (wallVolumeFace9.orientation != WallVolumeOrientaion.Left)
					{
						WallVolumeFace wallVolumeFace10 = faces[j];
						if (wallVolumeFace10.orientation != WallVolumeOrientaion.Right)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			case WallVolumeOrientaion.Right:
			{
				for (int m = 0; m < faces.Count; m++)
				{
					WallVolumeFace wallVolumeFace21 = faces[m];
					if (wallVolumeFace21.posY != y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace22 = faces[m];
					if (wallVolumeFace22.posZ != z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace23 = faces[m];
					if (wallVolumeFace23.posX <= x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace24 = faces[m];
					if (wallVolumeFace24.orientation != WallVolumeOrientaion.Left)
					{
						WallVolumeFace wallVolumeFace25 = faces[m];
						if (wallVolumeFace25.orientation != WallVolumeOrientaion.Right)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			case WallVolumeOrientaion.Forward:
			{
				for (int k = 0; k < faces.Count; k++)
				{
					WallVolumeFace wallVolumeFace11 = faces[k];
					if (wallVolumeFace11.posX != x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace12 = faces[k];
					if (wallVolumeFace12.posY != y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace13 = faces[k];
					if (wallVolumeFace13.posZ <= z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace14 = faces[k];
					if (wallVolumeFace14.orientation != WallVolumeOrientaion.Forward)
					{
						WallVolumeFace wallVolumeFace15 = faces[k];
						if (wallVolumeFace15.orientation != WallVolumeOrientaion.Back)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			case WallVolumeOrientaion.Back:
			{
				for (int i = 0; i < faces.Count; i++)
				{
					WallVolumeFace wallVolumeFace = faces[i];
					if (wallVolumeFace.posX != x)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace2 = faces[i];
					if (wallVolumeFace2.posY != y)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace3 = faces[i];
					if (wallVolumeFace3.posZ >= z)
					{
						continue;
					}
					WallVolumeFace wallVolumeFace4 = faces[i];
					if (wallVolumeFace4.orientation != WallVolumeOrientaion.Forward)
					{
						WallVolumeFace wallVolumeFace5 = faces[i];
						if (wallVolumeFace5.orientation != WallVolumeOrientaion.Back)
						{
							continue;
						}
					}
					flag = !flag;
				}
				break;
			}
			default:
				throw new InvalidOperationException();
			}
			return flag;
		}

		private void ClearVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
		{
			for (int i = 0; i < faces.Count; i++)
			{
				WallVolumeFace wallVolumeFace = faces[i];
				if (((wallVolumeFace.posX >= x && wallVolumeFace.posX <= x + brushX - 1) || (wallVolumeFace.posX == x - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Right) || (wallVolumeFace.posX == x + brushX && wallVolumeFace.orientation == WallVolumeOrientaion.Left)) && ((wallVolumeFace.posY >= y && wallVolumeFace.posY <= y + brushY - 1) || (wallVolumeFace.posY == y - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Up) || (wallVolumeFace.posY == y + brushY && wallVolumeFace.orientation == WallVolumeOrientaion.Down)) && ((wallVolumeFace.posZ >= z && wallVolumeFace.posZ <= z + brushZ - 1) || (wallVolumeFace.posZ == z - 1 && wallVolumeFace.orientation == WallVolumeOrientaion.Forward) || (wallVolumeFace.posZ == z + brushZ && wallVolumeFace.orientation == WallVolumeOrientaion.Back)))
				{
					faces.RemoveAt(i);
					i--;
				}
			}
		}

		public void AddVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
		{
			ClearVoxel(x, y, z, brushX, brushY, brushZ);
			for (int i = 0; i < brushX; i++)
			{
				for (int j = 0; j < brushY; j++)
				{
					if (!IsOpen(x + i, y + j, z, WallVolumeOrientaion.Back))
					{
						faces.Add(new WallVolumeFace(x + i, y + j, z, WallVolumeOrientaion.Back));
					}
					if (!IsOpen(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward))
					{
						faces.Add(new WallVolumeFace(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward));
					}
				}
			}
			for (int k = 0; k < brushY; k++)
			{
				for (int l = 0; l < brushZ; l++)
				{
					if (!IsOpen(x, y + k, z + l, WallVolumeOrientaion.Left))
					{
						faces.Add(new WallVolumeFace(x, y + k, z + l, WallVolumeOrientaion.Left));
					}
					if (!IsOpen(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right))
					{
						faces.Add(new WallVolumeFace(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right));
					}
				}
			}
			for (int m = 0; m < brushX; m++)
			{
				for (int n = 0; n < brushZ; n++)
				{
					if (!IsOpen(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up))
					{
						faces.Add(new WallVolumeFace(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up));
					}
					if (!IsOpen(x + m, y, z + n, WallVolumeOrientaion.Down))
					{
						faces.Add(new WallVolumeFace(x + m, y, z + n, WallVolumeOrientaion.Down));
					}
				}
			}
		}

		public void RemoveVoxel(int x, int y, int z, int brushX, int brushY, int brushZ)
		{
			ClearVoxel(x, y, z, brushX, brushY, brushZ);
			for (int i = 0; i < brushX; i++)
			{
				for (int j = 0; j < brushY; j++)
				{
					if (IsOpen(x + i, y + j, z, WallVolumeOrientaion.Back))
					{
						faces.Add(new WallVolumeFace(x + i, y + j, z - 1, WallVolumeOrientaion.Forward));
					}
					if (IsOpen(x + i, y + j, z + brushZ - 1, WallVolumeOrientaion.Forward))
					{
						faces.Add(new WallVolumeFace(x + i, y + j, z + brushZ, WallVolumeOrientaion.Back));
					}
				}
			}
			for (int k = 0; k < brushY; k++)
			{
				for (int l = 0; l < brushZ; l++)
				{
					if (IsOpen(x, y + k, z + l, WallVolumeOrientaion.Left))
					{
						faces.Add(new WallVolumeFace(x - 1, y + k, z + l, WallVolumeOrientaion.Right));
					}
					if (IsOpen(x + brushX - 1, y + k, z + l, WallVolumeOrientaion.Right))
					{
						faces.Add(new WallVolumeFace(x + brushX, y + k, z + l, WallVolumeOrientaion.Left));
					}
				}
			}
			for (int m = 0; m < brushX; m++)
			{
				for (int n = 0; n < brushZ; n++)
				{
					if (IsOpen(x + m, y + brushY - 1, z + n, WallVolumeOrientaion.Up))
					{
						faces.Add(new WallVolumeFace(x + m, y + brushY, z + n, WallVolumeOrientaion.Down));
					}
					if (IsOpen(x + m, y, z + n, WallVolumeOrientaion.Down))
					{
						faces.Add(new WallVolumeFace(x + m, y - 1, z + n, WallVolumeOrientaion.Up));
					}
				}
			}
		}

		public void OnDrawGizmosSelected()
		{
			if (!(gizmoSize == Vector3.zero))
			{
				Gizmos.color = gizmoColor;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawCube(gizmoStart + gizmoSize / 2f, gizmoSize + gizmoOffset);
				Gizmos.DrawWireCube(gizmoStart + gizmoSize / 2f, gizmoSize + gizmoOffset);
			}
		}
	}
}
