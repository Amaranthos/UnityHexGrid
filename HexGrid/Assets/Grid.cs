using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[ExecuteInEditMode]
public class Grid : MonoBehaviour {
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

	//Internal variables
	private Tile[] grid = null;
	private Mesh hexMesh = null;
	private bool gridExists = false;
	private CubeIndex[] directions = new CubeIndex[] {new CubeIndex(1, -1, 0), new CubeIndex(1, 0, -1), new CubeIndex(0, 1, -1), new CubeIndex(-1, 1, 0), new CubeIndex(-1, 0, 1), new CubeIndex(0, -1, 1)}; 

	#region Getters and Setters
	public Tile[] Tiles {
		get {return grid;}
	}

	public bool GridExists {
		get {return gridExists;}
	}
	#endregion

	#region Public Methods
	public void GenerateGrid() {
		if(gridExists){
			Debug.Log ("Grid already exists");
			return;
		}

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
		gridExists = true;
	}

	public void ClearGrid() {
		if(!gridExists){
			Debug.Log ("Grid does not exist");
			return;
		}

		Debug.Log ("Clearing grid...");

		var list = grid.ToList();
		for(int i = 0; i < list.Count; i++)
			DestroyImmediate(list[i].gameObject, true);

		Array.Clear(grid, 0, grid.Length);

		gridExists = false;
	}

	public List<Tile> Neighbours(Tile tile) {
		List<Tile> ret = new List<Tile>();
		OffsetIndex o;

		for(int i = 0; i <= 6; i++) {
			switch(offsetCoords){
			case OffsetCoords.Even:
				switch(hexOrientation){
				case HexOrientation.Flat:
					o = Tile.CubeToEvenFlat(tile.index + directions[i]);
					if(o.row + o.col * mapWidth > 0 && o.row + o.col * mapWidth < mapWidth * mapHeight)
						ret.Add(grid[o.row + o.col * mapWidth]);
					break;

				case HexOrientation.Pointy:
					o = Tile.CubeToEvenPointy(tile.index + directions[i]);
					if(o.row + o.col * mapWidth < mapWidth * mapHeight)
						ret.Add(grid[o.row + o.col * mapWidth]);
					break;
				}
				break;

			case OffsetCoords.Odd:
				switch(hexOrientation){
				case HexOrientation.Flat:
					o = Tile.CubeToOddFlat(tile.index + directions[i]);
					if(o.row + o.col * mapWidth < mapWidth * mapHeight)
						ret.Add(grid[o.row + o.col * mapWidth]);
					break;
					
				case HexOrientation.Pointy:
					o = Tile.CubeToOddPointy(tile.index + directions[i]);
					if(o.row + o.col * mapWidth < mapWidth * mapHeight)
						ret.Add(grid[o.row + o.col * mapWidth]);
					break;
				}
				break;
			}
		}
		return ret;
	}
	#endregion

	#region Private Methods
	private void GetMesh() {
		hexMesh = null;
		Tile.GetHexMesh(hexRadius, hexOrientation, ref hexMesh);
	}

	private void GenHexShape() {
		Debug.Log ("Generating hexagonal shaped grid...");
	}

	private void GenRectShape() {
		Debug.Log ("Generating rectangular shaped grid...");
		grid = new Tile[mapWidth * mapHeight];
		
		for(int i = 0; i < mapWidth; i++){
			for(int j = 0; j < mapHeight; j++){
			switch(hexOrientation){
				case HexOrientation.Flat:
					grid[i + j * mapWidth] = CreateHexGO(new Vector3(i * HorizontalSpacing(), 0.0f, j * VerticalSpacing() + ((i&1) * 0.5f * VerticalSpacing())), ("Hex [" + i + "," + j + "]"));
					break;

				case HexOrientation.Pointy:
					grid[i + j * mapWidth] = CreateHexGO(new Vector3(i * HorizontalSpacing() + (j&1) * 0.5f * HorizontalSpacing(), 0.0f, j * VerticalSpacing()), ("Hex [" + i + "," + j + "]"));
					break;
				}
			}
		}
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
			lines.material = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

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