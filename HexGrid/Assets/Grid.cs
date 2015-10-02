using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Grid : MonoBehaviour {
	public static Grid inst;

	//Map settings
	public MapShape mapShape = MapShape.Rectangle;
	public OffsetCoords offsetCoords = OffsetCoords.Odd;
	public int mapWidth;
	public int mapHeight;

	//Hex Settings
	public HexOrientation hexOrientation = HexOrientation.Flat;
	public float hexRadius = 1;
	public Material hexMaterial;
	
	//Generation Options
	public bool addColliders = true;
	public bool drawOutlines = true;
	public Material lineMaterial;

	//Internal variables
	private Dictionary<string, Tile> grid = new Dictionary<string, Tile>();
	private Mesh hexMesh = null;
	private CubeIndex[] directions = new CubeIndex[] {new CubeIndex(1, -1, 0), new CubeIndex(1, 0, -1), new CubeIndex(0, 1, -1), new CubeIndex(-1, 1, 0), new CubeIndex(-1, 0, 1), new CubeIndex(0, -1, 1)}; 

	#region Getters and Setters
	public Dictionary<string, Tile> Tiles {
		get {return grid;}
	}
	#endregion

	#region Public Methods
	public void GenerateGrid() {
		ClearGrid();
		GetMesh();

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

		foreach(var tile in grid)
			DestroyImmediate(tile.Value.gameObject, false);
	
		grid.Clear();
	}

	public List<Tile> Neighbours(Tile tile) {
		List<Tile> ret = new List<Tile>();
		CubeIndex o;

		for(int i = 0; i < 6; i++) {

			o = tile.index + directions[i];
			if(grid.ContainsKey(o.ToString()))
				ret.Add(grid[o.ToString()]);
		}
		return ret;
	}
	#endregion

	#region Private Methods
	private void Awake() {
		if(!inst)
			inst = this;

		GenerateGrid();
	}

	private void GetMesh() {
		hexMesh = null;
		Tile.GetHexMesh(hexRadius, hexOrientation, ref hexMesh);
	}

	private void GenHexShape() {
		Debug.Log ("Generating hexagonal shaped grid...");
	}

	private void GenRectShape() {
		Debug.Log ("Generating rectangular shaped grid...");
//		grid = new Dictionary<int, Tile>();

		CubeIndex index;
		Tile tile;

		for (int q = 0; q <= mapWidth; q++){
			for(int r = 0; r <= mapHeight - q; r++){
				tile = CreateHexGO(new Vector3(q * HorizontalSpacing(), 0.0f, r * VerticalSpacing() + ((q&1) * 0.5f * VerticalSpacing())), ("Hex " ));
				tile.index = new CubeIndex(q,r,-q-r);
				grid.Add(tile.index.ToString(), tile);
			}
		}

//		for(int i = 0; i < mapWidth; i++){
//			for(int j = 0; j < mapHeight; j++){
//				index = new CubeIndex(i,j);
//				switch(hexOrientation){
//				case HexOrientation.Flat:
//					tile = CreateHexGO(new Vector3(i * HorizontalSpacing(), 0.0f, j * VerticalSpacing() + ((i&1) * 0.5f * VerticalSpacing())), ("Hex " + index.ToString()));
//					tile.index = index;
//					grid[index.ToString()] = tile;
//					break;
//
//				case HexOrientation.Pointy:
//					tile = CreateHexGO(new Vector3(i * HorizontalSpacing() + (j&1) * 0.5f * HorizontalSpacing(), 0.0f, j * VerticalSpacing()), ("Hex " + index.ToString()));
//					tile.index = index;
//					grid[index.ToString()] = tile;
//					break;
//				}
//			}
//		}
	}

	private Tile CreateHexGO(Vector3 postion, string name) {
		GameObject go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(Tile));

		if(addColliders)
			go.AddComponent<MeshCollider>();

		if(drawOutlines)
			go.AddComponent<LineRenderer>();

		go.transform.position = postion;
		go.transform.parent = this.transform;

		switch(hexOrientation){
		case HexOrientation.Flat:
			break;

		case HexOrientation.Pointy:
			break;
		}

		Tile tile = go.GetComponent<Tile>();
		MeshFilter fil = go.GetComponent<MeshFilter>();
		MeshRenderer ren = go.GetComponent<MeshRenderer>();

		fil.sharedMesh = hexMesh;

		ren.material = (hexMaterial)? hexMaterial : UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

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
			lines.material = lineMaterial;

			lines.SetVertexCount(7);

			for(int vert = 0; vert <= 6; vert++)
				lines.SetPosition(vert, Tile.Corner(tile.transform.position, hexRadius, vert, hexOrientation));
		}

		return tile;
	}

	private float VerticalSpacing() {
		switch(hexOrientation){
		case HexOrientation.Flat:
			return Mathf.Sqrt(3)/2 * (2 * hexRadius);

		case HexOrientation.Pointy:
			return (2 * hexRadius) * 3 / 4;
		}
		return 0;
	}

	private float HorizontalSpacing() {
		switch(hexOrientation){
		case HexOrientation.Flat:
			return (2 * hexRadius) * 3 / 4;
			
		case HexOrientation.Pointy:
			return Mathf.Sqrt(3)/2 * (2 * hexRadius);
		}
		return 0;
	}
	#endregion
}

[System.Serializable]
public enum MapShape {
	Rectangle,
	Hexagonal
}

[System.Serializable]
public enum HexOrientation {
	Pointy,
	Flat
}

[System.Serializable]
public enum OffsetCoords {
	Odd,
	Even
}