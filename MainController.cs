using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.BuildingManager;

public class MainController : MonoBehaviour {
    private GameObject grid;
    private TileBuildingData dataScript;
    private InputHandler mouseHandler;
    public Tilemap tilemapTemplate;
    public Tilemap tilemap;
    // public Building currentBuilding;
    // HashSet<Building> buildings = new HashSet<Building>();
    //public HashSet<Vector3Int> buildingBaseCoordinates = new HashSet<Vector3Int>();
    Vector3Int currentCell;

    Tilemap mapTilemap;
    // private bool invalidTilesAreVisible = false;
    // public Actions currentAction;
    private bool deleteIsHeld;
    private Vector3Int deleteStartCell;
    new private GameObject camera;
    string mapName;
    Tilemap floorTilemap;
    HashSet<Vector3Int> floorCells;
    // HashSet<Floor> floors;
    // FloorType currentFloorType;

    //TEMPORARY HELPING VARIABLES
    Tilemap tempTilemap;
    bool editIsHeld;
    Vector3Int editStartCell;

    void Start() {

        grid = GameObject.FindGameObjectWithTag("Grid");
        //placeHouse(1);
        currentCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        mapTilemap = Instantiate<Tilemap>(tilemapTemplate, grid.transform);
        mapTilemap.GetComponent<TilemapRenderer>().sortingOrder = -1000; //SortingOrder is Layer Number
        mapTilemap.name = "Map";
        mapTilemap.tag = "CurrentMap";

        floorTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
        floorCells = new HashSet<Vector3Int>();

        camera = GameObject.FindGameObjectWithTag("MainCamera");

        tilemap = GameObject.FindGameObjectWithTag("NormalMap").GetComponent<Tilemap>();


        tempTilemap = Instantiate<Tilemap>(tilemapTemplate, grid.transform);
        tempTilemap.name = "TempTilemap";



        dataScript = gameObject.AddComponent(typeof(TileBuildingData)) as TileBuildingData;
        //dataScript.addInvalidTilesData("Normal");

        //mouseHandler = gameObject.AddComponent(typeof(MouseHandler)) as MouseHandler;
        //mouseHandler = gameObject.GetComponent<MouseHandler>();

        //utils = gameObject.GetComponent<Utility>();
        //setMap(MapTypes.Normal);
        //grid.GetComponent<BuildingController>().PlaceBuilding(new ShippingBin(null, null, null), new Vector3Int(44, 14, 0));
    }

    void Update() {
        // Vector3Int currentCellnew = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // HashSet<Vector3Int> buildingBaseCoordinates = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>().GetUnavailableCoordinates();
        // if (currentCell != currentCellnew) {
        //     currentCell = currentCellnew;
        //     //mouseHandler.HandleMouseMove(currentCell, currentBuilding, currentFloorType, buildingBaseCoordinates, deleteStartCell, editStartCell, currentAction, deleteIsHeld, editIsHeld);
        // }

        // if (Input.GetKeyDown(KeyCode.Mouse1)) {
        //     // Open Building Settings Menu
        //     Building building = buildings.FirstOrDefault<Building>(building => (building.VectorInBaseCoordinates(currentCell) && building.GetBuildingInteractions() != null));
        //     if (!(building is null)) {
        //         if (building.GetBuildingInteractions() == null) return;
        //         if (building.GetBuildingInteractions().Length > 0) openBuildingSettings(building);
        //     } else DeleteFloor(currentCell);

        // }

       // if (Input.GetKeyDown(KeyCode.I)) ToggleRedTiles();

        //f (Input.GetKeyDown(KeyCode.R)) { dataScript.addInvalidTilesData(mapName); ToggleRedTiles(); ToggleRedTiles(); }

        //if (Input.GetKeyDown(KeyCode.Y)) PlaceFloor(currentCell, currentFloorType);


        // if (Input.GetKeyDown(KeyCode.E)) {
        //     currentAction = Actions.EDIT;
        // }

        //Place Building
        // if (Input.GetKeyDown(KeyCode.Mouse0)) {
        //    // mouseHandler.HandleMouseLeftClick(currentCell, currentBuilding, buildingBaseCoordinates, currentFloorType, currentAction, deleteIsHeld, deleteStartCell, tempTilemap, mapName);
        // }

        // if (Input.GetKeyUp(KeyCode.Mouse0)) {
        //     if (!EventSystem.current.IsPointerOverGameObject()) {
        //         if (currentAction == Actions.DELETE) {
        //             if (currentCell == deleteStartCell) { deleteBuilding(currentCell); return; }
        //             deleteIsHeld = false;
        //             int deleteWidth = Mathf.Abs(currentCell.x - deleteStartCell.x) + 1;
        //             int deleteHeight = Mathf.Abs(currentCell.y - deleteStartCell.y) + 1;
        //             Vector3Int[] deleteArea = null;
        //             Vector3Int lowerLeftCorner = new Vector3Int(Mathf.Min(currentCell.x, deleteStartCell.x), Mathf.Min(currentCell.y, deleteStartCell.y), 0);
        //             deleteArea = getAreaArroundPosition(lowerLeftCorner, deleteHeight, deleteWidth).ToArray<Vector3Int>();
        //             foreach (Vector3Int vec in deleteArea) deleteBuilding(vec);
        //         }
        //     }
        // }
    }

    // public void setActionToPlace() {
    //     currentAction = Actions.PLACE;
    //     HashSet<Vector3Int> buildingBaseCoordinates = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>().GetUnavailableCoordinates();
    //     mouseHandler.PlaceMouseoverEffect(currentCell, currentBuilding, buildingBaseCoordinates);
    //     FloorBarHandler handleBar = GameObject.FindGameObjectWithTag("FloorSelectBar").GetComponent<FloorBarHandler>();
    //     handleBar.hideFloorBar();
    // }
    // public void setActionToDelete() {
    //     currentAction = Actions.DELETE;
    //     mouseHandler.DeleteMouseoverEffect(currentCell);
    //     FloorBarHandler handleBar = GameObject.FindGameObjectWithTag("FloorSelectBar").GetComponent<FloorBarHandler>();
    //     handleBar.hideFloorBar();
    // }
    // public void setActionToPlaceFloor() {
    //     currentAction = Actions.PLACE_FLOOR;
    //     FloorBarHandler handleBar = GameObject.FindGameObjectWithTag("FloorSelectBar").GetComponent<FloorBarHandler>();
    //     handleBar.showFloorBar();
    //     mouseHandler.PlaceFloorMouseoverEffect(currentCell, currentFloorType);
    // }

    // public FloorType getFloorType() { return currentFloorType; }

    // public Actions GetAction() { return currentAction; }

    private void openBuildingSettings(Building building) {
        //building.GetTilemap().gameObject.GetComponent<ChangeBuildingTier>().toggleTiers();
    }

    ///<summary>Change the current background (aka the map)</summary>
    ///<param name="mapName">The name of the farm, possible farms:
    ///Normal, Riverland, Forest, Hilltop, Wilderness, Four Corners, Beach. IS CASE SENSITIVE</param>
    // public void setMap(MapTypes mapType) {
    //     //deleteAll();
    //     this.mapName = mapType.ToString();
    //     mapTilemap.name = mapName + "Map";
    //     //clearBuildingBase();
    //     Vector3Int mapPos = new Vector3Int(-27, -36, 0);
    //     Texture2D mapTexture = Resources.Load("Maps/" + mapName + "Map") as Texture2D;
    //     Vector3Int[] spriteArrayCoordinates = getAreaArroundPosition(mapPos, mapTexture.height / 16, mapTexture.width / 16, true).ToArray<Vector3Int>();
    //     Tile[] tiles = SplitSprite(mapTexture);
    //     dataScript.addInvalidTilesData(mapName);
    //     mapTilemap.ClearAllTiles();
    //     mapTilemap.SetTiles(spriteArrayCoordinates, tiles);
    //     // ToggleRedTiles();
    //     // ToggleRedTiles();
    //     // placeHouse(1);
    //     camera.GetComponent<CameraController>().UpdateCameraBounds();
    // }

    // ///<summary>Place a building at a desired porition</summary>
    // ///<param name="position">the vector containing the bottom left coordinates of the building position</param>
    // ///<param name="buildingToPlace">the Building you want to place</param>
    // public void PlaceBuildingAtPosition(Vector3Int position, Building buildingToPlace, int sortingOrderOffset = 0) {
    //     //if (buildingToPlace.getName() == "Fish Pond") { placeFishPond(position); return; }
    //     Vector3Int[] spriteArrayCoordinates = getAreaArroundPosition(position, buildingToPlace.GetHeight(), buildingToPlace.GetWidth(), true).ToArray<Vector3Int>();
    //     Vector3Int[] baseArea = getAreaArroundPosition(position, buildingToPlace.GetBaseHeight(), buildingToPlace.GetWidth()).ToArray<Vector3Int>();
    //     Tile[] tiles = SplitSprite(buildingToPlace);
    //     Tilemap emptyGrid = Instantiate<Tilemap>(tilemapTemplate, grid.transform);
    //     emptyGrid.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50 + sortingOrderOffset; //SortingOrder is Layer Number
    //     emptyGrid.gameObject.name = buildingToPlace + "";
    //     emptyGrid.SetTiles(spriteArrayCoordinates, tiles);
        // if (buildingToPlace.GetName() == "Greenhouse") {
        //     List<Vector3Int> addedArea = getAreaArroundPosition(new Vector3Int(position.x + 2, position.y - 2, 0), 2, 3).ToList<Vector3Int>();
        //     List<Vector3Int> newBaseArea = baseArea.ToList<Vector3Int>();
        //     newBaseArea.AddRange(addedArea);
        //     baseArea = newBaseArea.ToArray<Vector3Int>();
        // }
    //     if (buildingToPlace.GetName() == "FishPond") addFishPodExtras(emptyGrid.gameObject, spriteArrayCoordinates);
    //     if (invalidTilesAreVisible) foreach (Vector3Int vec in baseArea) AddInvalidTileToTilemap(vec);
    //     //        Debug.Log("Index is " + indexToPlaceBuilding);
    //     //Building buildingPlaced = GenerateBuilding(buildingToPlace.getName(), spriteArrayCoordinates, baseArea, emptyGrid);
    //     //buildings.Add(buildingPlaced);
    //     //if (currentAction == Actions.PLACE && !(buildingToPlace is House)) mouseHandler.placeMouseoverEffect(position, buildingPlaced, buildingBaseCoordinates);
    //     //        Debug.Log("Placed " + buildingToPlace + "(" + buildings.Count + ")");
    //     foreach (Vector3Int vec in baseArea) buildingBaseCoordinates.Add(vec);
    //     Tilemap insideAreaTilemap = null;
    //     //if (buildingToPlace.getInsideAreaTexture() != null) insideAreaTilemap = placeBuildingInsideArea(buildingPlaced);
    //     //        Debug.Log(buildingToPlace + " inside area: " + buildingToPlace.getInsideAreaTexture());
    //     if (buildingToPlace.GetNumberOfButtons() > 0) {
    //         emptyGrid.gameObject.AddComponent<ChangeBuildingTier>();
    //         //buildingPlaced.buttonParent = emptyGrid.gameObject.GetComponent<ChangeBuildingTier>().init(buildingPlaced, emptyGrid.gameObject, insideAreaTilemap);
    //     }
    // }

    // public void addFishPodExtras(GameObject fishPond, Vector3Int[] spriteArrayCoordinates) {
    //     Tile[] fishPondBottom = SplitSprite(new FishPondBottom(null, null, null));
    //     Tilemap fishPondBottomTilemap = Instantiate<Tilemap>(tilemapTemplate, fishPond.transform);
    //     fishPondBottomTilemap.GetComponent<TilemapRenderer>().sortingOrder = fishPond.GetComponent<TilemapRenderer>().sortingOrder - 1;
    //     fishPondBottomTilemap.gameObject.name = "FishPondBottom";
    //     fishPondBottomTilemap.SetTiles(spriteArrayCoordinates, fishPondBottom);
    //     Tilemap fishPondDeco = Instantiate<Tilemap>(tilemapTemplate, fishPond.transform);
    //     fishPondDeco.GetComponent<TilemapRenderer>().sortingOrder = fishPond.GetComponent<TilemapRenderer>().sortingOrder + 1;
    //     fishPondDeco.gameObject.name = "FishPondDeco";
    //     FishDeco deco = new FishDeco(getAreaArroundPosition(spriteArrayCoordinates[0], 3, 5).ToArray<Vector3Int>(), fishPondDeco);
    //     Tile[] decoTiles = deco.getDeco(0);
    //     //        Debug.Log(deco.getPosition().Length);
    //     //      Debug.Log(decoTiles.Length);
    //     // foreach (Tile tile in decoTiles) if (tile == null) Debug.Log("NULL");
    //     fishPondDeco.SetTiles(deco.getPosition(), decoTiles);
    // }


    // public void changeBuilding(Vector3Int position, Building newBuilding) {
    //     Vector3Int[] spriteArrayCoordinates = getAreaArroundPosition(position, newBuilding.GetHeight(), newBuilding.GetWidth(), true).ToArray<Vector3Int>();
    //     Vector3Int[] baseArea = getAreaArroundPosition(position, newBuilding.GetBaseHeight(), newBuilding.GetWidth()).ToArray<Vector3Int>();
    //     Tile[] tiles = SplitSprite(newBuilding);
    //     Building oldBuilding = buildings.FirstOrDefault<Building>(building => building.VectorInBaseCoordinates(position));
    //     newBuilding = GenerateBuilding(newBuilding.GetName(), oldBuilding.GetSpriteCoordinates().ToList<Vector3Int>(), oldBuilding.GetBaseCoordinates().ToList<Vector3Int>(), oldBuilding.GetTilemap());
    //     if (oldBuilding is null) return;
    //     oldBuilding.GetTilemap().ClearAllTiles();
    //     oldBuilding.GetTilemap().SetTiles(spriteArrayCoordinates, tiles);
    //     if (oldBuilding.GetBuildingInteractions().Contains(ButtonTypes.ENTER)) {
    //         GameObject.Destroy(oldBuilding.GetTilemap().gameObject.transform.GetChild(0).gameObject);
    //         Tilemap newInsideArea = placeBuildingInsideArea(newBuilding);
    //         // BuildingContextMenu contextMenu = null; //GET IT SOMEHOW;
    //         //contextMenu.setInsideArea(newInsideArea);
    //     }
    //     // if (invalidTilesAreVisible) foreach (Vector3Int vec in baseArea) invalidTilemap.SetTile(vec, redTile);
    //     // if (currentAction == Actions.PLACE) placeMouseoverEffect();
    // }

    // public void setFishPondColor(Color color, Vector3Int fishPondPosition) {
    //     Building fishPond = buildings.FirstOrDefault<Building>(building => building.VectorInBaseCoordinates(fishPondPosition));
    //     fishPond.GetTilemap().color = color;
    // }

    // private Tilemap placeBuildingInsideArea(Building building, Tilemap tilemap = null) {
    //     Vector3Int insideAreaRoot = new Vector3Int(building.GetBaseCoordinates()[0].x + building.GetWidth(), building.GetBaseCoordinates()[0].y, 0);
    //     Vector3Int[] baseArea = getAreaArroundPosition(insideAreaRoot, building.GetInsideAreaTexture().height / 16, building.GetInsideAreaTexture().width / 16, true).ToArray<Vector3Int>();
    //     Tile[] tiles = SplitSprite(building.GetInsideAreaTexture());
    //     Tilemap emptyGrid;
    //     if (tilemap is null) emptyGrid = Instantiate<Tilemap>(tilemapTemplate, building.GetTilemap().gameObject.transform);
    //     else emptyGrid = tilemap;
    //     emptyGrid.ClearAllTiles();
    //     emptyGrid.GetComponent<TilemapRenderer>().sortingOrder = -building.GetBaseCoordinates()[0].y + 500; //SortingOrder is Layer Number
    //     emptyGrid.gameObject.name = building + "Inside";
    //     if (tilemap is null) emptyGrid.transform.localScale *= 0;
    //     emptyGrid.SetTiles(baseArea, tiles);
    //     return emptyGrid;
    // }

    ///<summary>Change the current house</summary>
    ///<param name="tier">the tier of the new house you want to place</param>
    // public void placeHouse(int tier) {
    //     //        Debug.Log("Placed House");
    //     var housePos = mapName switch{
    //         "Four Corners" => new Vector3Int(32, 27, 0),
    //         "Beach" => new Vector3Int(33, 57, 0),
    //         _ => new Vector3Int(32, 12, 0),
    //     };
    //     Building house = buildings.FirstOrDefault<Building>(building => building is House);
    //     HashSet<Vector3Int> buildingBaseCoordinates = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>().GetUnavailableCoordinates();
    //     if (house != null) {
    //         foreach (Vector3Int cell in house.GetBaseCoordinates()) buildingBaseCoordinates.Remove(cell);
    //         buildings.Remove(house);
    //         GameObject.Destroy(house.GetTilemap().gameObject);
    //     }
    //     house = tier switch{
    //         1 => new HouseT1(null, null, null),
    //         2 => new HouseT2(null, null, null),
    //         3 => new HouseT3(null, null, null),
    //         _ => null,
    //     };
    //     GameObject.Destroy(house.buttonParent);
    //     grid.GetComponent<BuildingController>().PlaceBuilding(house, housePos);
    // }

    ///<summary>Delete ALL buildings except the house</summary>
    // public void deleteAll() {
    //     //Building house = buildings.FirstOrDefault<Building>(building => building is House);
    //     //int currentHouseTier = 1;
    //     HashSet<Vector3Int> buildingBaseCoordinates = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingPlacer>().GetUnavailableCoordinates();
    //     //if (!(house is null)) currentHouseTier = int.Parse(house.getName()[house.getName().Length - 1] + "");
    //     foreach (Building building in buildings) {
    //         if (building is House) continue;
    //         buildingBaseCoordinates.RemoveWhere(vec => building.VectorInBaseCoordinates(vec));
    //         if (invalidTilesAreVisible) RemoveInvalidTilesFromTilemap(building.GetBaseCoordinates());
    //         building.Delete();
    //     }
    //     buildings.RemoveWhere(building => !(building is House)); //Remove everything except the house
    //     // placeHouse(currentHouseTier);
    // }

    ///<summary>Delete ALL buildings except the house and clear all tiles except the house</summary>
    // public void clearBuildingBase() {
    //     deleteAll();
    //     HashSet<Vector3Int> buildingBaseCoordinates = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingPlacer>().GetUnavailableCoordinates();
    //     Building house = buildings.FirstOrDefault<Building>(building => building is House);
    //     if (house != null) buildingBaseCoordinates.RemoveWhere(vec => !(house.VectorInBaseCoordinates(vec))); //Remove everything except the tiles that belong to the house
    //     else buildingBaseCoordinates.Clear();
    // }

    // public void WriteString(string text, string file) {
    //     string path = "Assets/Resources/" + file + ".txt";
    //     StreamWriter writer = new StreamWriter(path, true);
    //     writer.WriteLine(text);
    //     Debug.Log("Pushed " + text + " to " + path);
    //     writer.Close();

    // }

    // static void WriteString(Vector3Int[] vectors, string file) {
    //     string path = "Assets/Resources/" + file + ".txt";
    //     StreamWriter writer = new StreamWriter(path, true);
    //     foreach (Vector3Int vec in vectors) {
    //         string text = vec.x + " " + vec.y + " " + vec.z;
    //         writer.WriteLine(text);
    //         Debug.Log("Pushed " + text + " to " + path);
    //     }
    //     writer.Close();
    // }
    // public void ExitApp() {
    //     Application.Quit();
    // }

    // public void PlaceFloor(Vector3Int cell, FloorType type, bool updateNeighbours = true) {
    //     Floor floor = new Floor(cell, type);
    //     HashSet<FloorFlag> flags = new HashSet<FloorFlag>();
    //     List<Vector3Int> neigbours = new List<Vector3Int>(4);
    //     neigbours.Add(new Vector3Int(cell.x - 1, cell.y, 0));
    //     neigbours.Add(new Vector3Int(cell.x + 1, cell.y, 0));
    //     neigbours.Add(new Vector3Int(cell.x, cell.y - 1, 0));
    //     neigbours.Add(new Vector3Int(cell.x, cell.y + 1, 0));
    //     if (floors.FirstOrDefault<Floor>(floor => neigbours[0] == floor.getPosition()) != null) flags.Add(FloorFlag.LEFT_ATTACHED);
    //     if (floors.FirstOrDefault<Floor>(floor => neigbours[1] == floor.getPosition()) != null) flags.Add(FloorFlag.RIGHT_ATTACHED);
    //     if (floors.FirstOrDefault<Floor>(floor => neigbours[2] == floor.getPosition()) != null) flags.Add(FloorFlag.BOTTOM_ATTACHED);
    //     if (floors.FirstOrDefault<Floor>(floor => neigbours[3] == floor.getPosition()) != null) flags.Add(FloorFlag.TOP_ATTACHED);
    //     Tile floorTile = floor.getFloorConfig(flags.ToArray<FloorFlag>(), type);
    //     floorTilemap.SetTile(cell, floorTile);
    //     //floors.Add(floor);
    //     if (updateNeighbours) { // updating neighbours means this tile doesnt exist yet
    //         floors.Add(floor);
    //         foreach (Floor floorIt in floors) if (neigbours.Contains<Vector3Int>(floorIt.getPosition())) PlaceFloor(floorIt.getPosition(), floorIt.getType(), false);
    //     }
    //     Debug.Log(floors.Count + "placed floors");
    // }
    //Not 0 refrences just refrenced from the Unity Editor

    //public void setFloorType(FloorType type) { currentFloorType = type; }



    //Not 0 refrences just refrenced from the Unity Editor
    // public void SetBuilding(string buildingName) {
    //     Building building = GenerateBuilding(buildingName);
    //     currentBuilding = building;
    //     currentAction = Actions.PLACE;
    //     FloorBarHandler handleBar = GameObject.FindGameObjectWithTag("FloorSelectBar").GetComponent<FloorBarHandler>();
    //     // if (currentBuilding is Floor) handleBar.showFloorBar();
    //     handleBar.hideFloorBar();
    // }

    // private void DeleteFloor(Vector3Int cell) {
    //     List<Vector3Int> neigbours = new List<Vector3Int>(4);
    //     neigbours.Add(new Vector3Int(cell.x - 1, cell.y, 0));
    //     neigbours.Add(new Vector3Int(cell.x + 1, cell.y, 0));
    //     neigbours.Add(new Vector3Int(cell.x, cell.y - 1, 0));
    //     neigbours.Add(new Vector3Int(cell.x, cell.y + 1, 0));
    //     floorTilemap.SetTile(cell, null);
    //     floors.RemoveWhere(floor => floor.getPosition() == cell);
    //     //foreach (Floor floorIt in floors) if (neigbours.Contains<Vector3Int>(floorIt.getPosition())) PlaceFloor(floorIt.getPosition(), floorIt.getType(), false);
    // }

    //public void ToggleRedTiles() { ToggleInvalidTiles(invalidTilesAreVisible, GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>().GetUnavailableCoordinates()); invalidTilesAreVisible = !invalidTilesAreVisible; }
}
