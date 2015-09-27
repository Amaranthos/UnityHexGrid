using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class Grid : MonoBehaviour {
	//Map settings
	public MapShape mapShape;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public int hexRadius;
	public Material material;
	
	//Generation Options
	public bool addColliders = true;
	public bool drawOutlines = true;


	//Private variables
	private Tile[] grid;
	private Mesh hexMesh;

	public void GetMesh() {
		hexMesh = Tile.GetHexMesh(hexRadius);
	}

	public void GenerateGrid() {
		switch(mapShape) {
		case MapShape.Hexagonal:
			break;

		case MapShape.Rectangle:
			GenRectShape();
			break;

		default:
			break;
		}
	}

	public void ClearGrid() {
		Debug.Log ("Clearing grid...");

		var list = grid.ToList();
		for(int i = 0; i < list.Count; i++)
			DestroyImmediate(list[i].gameObject, true);

		Array.Clear(grid, 0, grid.Length);
	}

	private void GenHexShape() {
		Debug.Log ("Generating hexagonal shaped grid...");
	}

	private void GenRectShape() {
		Debug.Log ("Generating rectangular shaped grid...");
		grid = new Tile[mapWidth * mapHeight];

		float width = hexRadius * 2;
		float height = (Mathf.Sqrt(3)/2) * width;

		for(int i = 0; i < mapWidth; i++)
			for(int j = 0; j < mapHeight; j++)
				CreateHexGO(i, j, width, height);
	}

	private void CreateHexGO(int i, int j, float width, float height) {
		GameObject go = new GameObject("Hex [" + i + "," + j + "]", typeof(MeshFilter), typeof(MeshRenderer), typeof(Tile));

		if(addColliders)
			go.AddComponent<MeshCollider>();

		if(drawOutlines)
			go.AddComponent<LineRenderer>();

		go.transform.position = new Vector3(i * width * 3/4, 0.0f, j * height + ((i&1) * 0.5f * height));
		go.transform.parent = this.transform;

		Tile tile = go.GetComponent<Tile>();
		MeshFilter fil = go.GetComponent<MeshFilter>();
		MeshRenderer ren = go.GetComponent<MeshRenderer>();

		fil.sharedMesh = hexMesh;

		if(material)
			ren.material = material;
		else
			ren.material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

		if(addColliders){
			MeshCollider col = go.GetComponent<MeshCollider>();
			col.sharedMesh = hexMesh;
		}

		if(drawOutlines) {
			LineRenderer lines = go.GetComponent<LineRenderer>();
			lines.useLightProbes = false;
			lines.receiveShadows = false;

			lines.SetWidth(0.1f, 0.1f);
			lines.SetColors(Color.black, Color.black);
			lines.material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

			lines.SetVertexCount(7);

			for(int vert = 0; vert <= 6; vert++)
				lines.SetPosition(vert, Tile.Corner(tile.transform.position, hexRadius, vert));
		}

		grid[i + j * mapWidth] = tile;
	}
}

[System.Serializable]
public enum MapShape {
	Rectangle,
	Hexagonal
}