using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public static Vector3 Corner(Vector3 origin, float radius, int corner){
		float angle = Mathf.PI / 180 * 60 * corner;
		return new Vector3(origin.x + radius * Mathf.Cos(angle), 0.0f, origin.z + radius * Mathf.Sin(angle));
	}

	public static Mesh GetHexMesh(float radius) {
		Mesh mesh = new Mesh();

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int i = 0; i < 6; i++)
			verts.Add(Corner(Vector3.zero, radius, i));

		tris.Add(0);
		tris.Add(2);
		tris.Add(1);
		
		tris.Add(0);
		tris.Add(5);
		tris.Add(2);
		
		tris.Add(2);
		tris.Add(5);
		tris.Add(3);
		
		tris.Add(3);
		tris.Add(5);
		tris.Add(4);
		
		uvs.Add(new Vector2(0.5f, 1f));
		uvs.Add(new Vector2(1, 0.75f));
		uvs.Add(new Vector2(1, 0.25f));
		uvs.Add(new Vector2(0.5f, 0));
		uvs.Add(new Vector2(0, 0.25f));
		uvs.Add(new Vector2(0, 0.75f));

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.name = "Hexagonal Plane";

		mesh.RecalculateNormals();

		return mesh;
	}
}
