using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Grid))]
public class GridInspector : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Grid grid = target as Grid;

		if(GUILayout.Button("Generate Hex Grid"))
			grid.GenerateGrid();

		if(GUILayout.Button("Clear Hex Grid"))
			grid.ClearGrid();
	}
}