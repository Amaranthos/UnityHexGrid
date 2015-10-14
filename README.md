# UnityHexGrid
A hex grid generation tool, for use in unity. Much of the hex logic is drawn from [Red Blob Games](http://www.redblobgames.com/grids/hexagons/). I made this because I initially struggled with the implementation so now you don't have to. Feel free to leave any feedback or suggestions, I will consider implementing suggestions that I think are generally useful to all users.

## Installation
Download unity package from releases

In unity Assets > Import Package > Custom Package

Navigate to downloaded file and select it

Create an empty gameObject `ctrl+shift+n` and add `Grid` script to it

## Generating a Grid in edit mode
Click Generate Grid in the inspector ensuring that relevant settings are selected

## Generating a Grid at runtime
Ensure relevant settings are set and call `GenerateGrid()` on an instance of Grid

```cs
public Material hexMaterial; //Assigned in inspector
public Material lineMaterial; //Assigned in inspector

private Grid grid;

private void Start() {
	//Set grid settings
	grid.mapShape = MapShape.Rectangle;
	grid.mapWidth = 5;
	grid.mapHeight = 5;
	grid.hexOrientation = HexOrientation.Flat;
	grid.hexRadius = 1;
	grid.hexMaterial = hexMaterial;
	grid.addColliders = true;
	grid.drawOutlines = true;
	grid.lineMaterial = lineMaterial;

	//Gen Grid
	grid.GenerateGrid();
}
```

## Access tiles at runtime

Call `Tiles` on an instance of Grid, returns a `Dictionary<string, Tile>` where the string is constructed from the tile's coordinates.

```cs
private Grid grid;

private void Start() {
	var tiles = grid.Tiles;
}

```

## Grid Settings
* `mapShape` determines the overall shape of the map, available options are
  * Rectangle
   
  ![Rectangle](http://i.imgur.com/I5eIjlu.jpg)
  * Hexagon
   
  ![Hexagon](http://i.imgur.com/pvCvkuT.jpg)
  * Parrallelogram
   
  ![Parrallelogram](http://i.imgur.com/ZtASYn0.jpg)
  * Triangle
  
  ![Triangle](http://i.imgur.com/uOEkZKF.jpg)
* `mapWidth` an `int`, controls the number tiles wide the map is, for hexagonal shape the larger of `mapWidth` and `mapHeight` is picked and used as a radius. 
* `mapHeight` an `int`, controls the number of tiles high the map is
* `hexOrientation` the orientation of the individual hexes, available options are
    * Pointy
  ![Pointy](http://i.imgur.com/CGWnE1M.jpg)
    * Flat
  ![Flat](http://i.imgur.com/es0TKVS.jpg)
* `hexRadius` a `float`, controls the radius of hex tile meshes, in unity units, measured from the centre of the hex to a corner, all corners are equidistant from the centre
* `hexMaterial` a `Material`, applied to the hex tile meshes, if not specified defaults to unity's diffuse material
* `addColliders` a `bool`, when true grid generation will add a mesh collider to the tiles using the same mesh as the tiles
* `drawOutlines` a `bool`, when true grid generation will add line renderers to the tiles and draw outlines for the meshes
* `lineMaterial` a `Material`, applied to the line renderers for drawing outlines, if you want outlines add the `Lines` material, included in the package, to the inspector field. Apparently I couldn't load the default Sprites/Default material via code
