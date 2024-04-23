using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.U2D;
using System.IO;
using System.Linq;

public class MapController : MonoBehaviour{
    public enum MapTypes {
        Normal,
        Riverland,
        Forest,
        Hilltop,
        Wilderness,
        FourCorners,
        Beach,
        GingerIsland,

    }

    private enum TileMode{
        nothing,
        addingInvalidTiles,
        removingInvalidTiles,
        addingPlantableTiles,
        removingPlantableTiles,
    }
    private SpriteAtlas atlas;
    public MapTypes CurrentMapType {get; private set;}
    Tile redTile;
    Tile greenTile;
    private bool unavailableCoordinatesAreVisible = false;
    private bool plantableCoordinatesAreVisible = false;
    // private bool addingInvalidTiles = false;
    // private bool addingPlantableTiles = false;
    private Actions currentAction;
    private TileMode tileMode;
    private Vector3Int startTile;
    // Start is called before the first frame update
    void Start(){
        atlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        SetMap(MapTypes.Normal);
    
        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;

        redTile = LoadTile("RedTile");
        greenTile = LoadTile("GreenTile");

        tileMode = TileMode.addingInvalidTiles;
    }

    public void Update(){//this is for adding invlid tiles and plantable tiles, should never be accesible to the user
        if (Input.GetKeyUp(KeyCode.A) && Input.GetKey(KeyCode.LeftControl)){
            #if UNITY_EDITOR
                tileMode = (TileMode)(((int) tileMode + 1) % 5);
                GetNotificationManager().SendNotification($"Mode set to {tileMode}");
                if (tileMode == TileMode.nothing) Building.CurrentAction = currentAction;
                else{
                    currentAction = Building.CurrentAction;
                    Building.CurrentAction = Actions.DO_NOTHING;
                    GetInputHandler().SetCursor(InputHandler.CursorType.Default);
                }
            #endif
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && (tileMode != TileMode.nothing)){
            startTile = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector3Int[] tileList;
            if (startTile != currentCell) tileList = GetAllCoordinatesInArea(startTile, currentCell).ToArray();
            else tileList = new Vector3Int[]{currentCell};
            if (tileMode == TileMode.addingInvalidTiles) foreach (Vector3Int tile in tileList) AddTileToCurrentMapInvalidTiles(tile);
            else if (tileMode == TileMode.addingPlantableTiles) foreach (Vector3Int tile in tileList) AddTileToCurrentMapPlantableTiles(tile);
            else if (tileMode == TileMode.removingInvalidTiles) foreach (Vector3Int tile in tileList) RemoveTileFromCurrentMapInvalidTiles(tile);
            else if (tileMode == TileMode.removingPlantableTiles) foreach (Vector3Int tile in tileList) RemoveTileFromCurrentMapPlantableTiles(tile);
        }
    }

    private void AddTileToCurrentMapInvalidTiles(Vector3Int tile){
        string path = $"Assets/Resources/Maps/{CurrentMapType}.txt";
        File.AppendAllText(path, $"{tile.x} {tile.y} {tile.z}\n");
        Debug.Log($"Added {tile.x} {tile.y} {tile.z} to {CurrentMapType} invalid tiles");
        GetBuildingController().GetUnavailableCoordinates().Add(tile);
        UpdateUnavailableCoordinates();
    }

    private void AddTileToCurrentMapPlantableTiles(Vector3Int tile){
        string path = $"Assets/Resources/Maps/{CurrentMapType}P.txt";
        File.AppendAllText(path, $"{tile.x} {tile.y} {tile.z}\n");
        Debug.Log($"Added {tile.x} {tile.y} {tile.z} to {CurrentMapType} plantable tiles");
        GetBuildingController().GetPlantableCoordinates().Add(tile);
        UpdatePlantableCoordinates();
    }

    private void RemoveTileFromCurrentMapInvalidTiles(Vector3Int tile){
        string path = $"Assets/Resources/Maps/{CurrentMapType}.txt";
        string vectorToRemove = $"{tile.x} {tile.y} {tile.z}";
        List<string> tiles = File.ReadAllText(path).Split('\n').ToList();
        tiles.Remove(vectorToRemove);
        string newText = string.Join("\n", tiles);
        File.WriteAllText(path, newText);
        Debug.Log($"Removed {tile.x} {tile.y} {tile.z} from {CurrentMapType} invalid tiles");
        GetBuildingController().GetUnavailableCoordinates().Remove(tile);
        UpdateUnavailableCoordinates();
    }

    private void RemoveTileFromCurrentMapPlantableTiles(Vector3Int tile){
        string path = $"Assets/Resources/Maps/{CurrentMapType}P.txt";
        string vectorToRemove = $"{tile.x} {tile.y} {tile.z}";
        List<string> tiles = File.ReadAllText(path).Split('\n').ToList();
        tiles.Remove(vectorToRemove);
        string newText = string.Join("\n", tiles);
        File.WriteAllText(path, newText);
        Debug.Log($"Removed {tile.x} {tile.y} {tile.z} from {CurrentMapType} plantable tiles");
        GetBuildingController().GetPlantableCoordinates().Remove(tile);
        UpdatePlantableCoordinates();
    }

    public void SetMap(MapTypes mapType) {
        CurrentMapType = mapType;
        BuildingController buildingController = GetBuildingController();
        buildingController.DeleteAllBuildings();
        buildingController.GetUnavailableCoordinates().Clear();
        GameObject map = GameObject.FindWithTag("CurrentMap");
        map.name = mapType.ToString() + "Map";
        Vector3Int mapPos = new(-27, -36, 0);
        Sprite mapTexture = atlas.GetSprite(map.name);
        Vector3Int[] spriteArrayCoordinates = GetAreaAroundPosition(mapPos, (int) mapTexture.textureRect.height / 16, (int) mapTexture.textureRect.width / 16).ToArray();
        Tile[] tiles = SplitSprite(mapTexture);
        TileBuildingData dataScript = map.AddComponent(typeof(TileBuildingData)) as TileBuildingData;
        dataScript.AddInvalidTilesData(mapType);
        dataScript.AddPlantableTilesData(mapType);
        Tilemap mapTilemap = map.GetComponent<Tilemap>();
        mapTilemap.ClearAllTiles();
        mapTilemap.SetTiles(spriteArrayCoordinates, tiles);
        if (mapType != MapTypes.GingerIsland) buildingController.PlaceHouse(1);
        GetCamera().GetComponent<CameraController>().UpdateCameraBounds();
        UpdateAllCoordinates();
    }

    public void SetMap(string mapType){
        MapTypes mapTypeEnum = MapTypes.Normal;
        try{
            mapTypeEnum = (MapTypes) Enum.Parse(typeof(MapTypes), mapType);
        }catch(Exception e){
           Debug.Log("Map type |" + mapType + "| does not exist" + e);
        }
        SetMap(mapTypeEnum);
    }

    public void ToggleAllCoordinates(){
        ToggleMapUnavailableCoordinates();
        ToggleMapPlantableCoordinates();
    }

    public void ToggleMapUnavailableCoordinates(){
        HashSet<Vector3Int> unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates();
        Tilemap unavailableCoordinatesTilemap = GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>();
        unavailableCoordinatesTilemap.ClearAllTiles();
        if (unavailableCoordinatesAreVisible){
            foreach (Vector3Int coordinate in unavailableCoordinates) unavailableCoordinatesTilemap.SetTile(coordinate, null);
        }else{
            foreach (Vector3Int coordinate in unavailableCoordinates) unavailableCoordinatesTilemap.SetTile(coordinate, redTile);
        }
        unavailableCoordinatesAreVisible = !unavailableCoordinatesAreVisible;
    }

    public void ToggleMapPlantableCoordinates(){
        HashSet<Vector3Int> plantableCoordinates = GetBuildingController().GetPlantableCoordinates();
        Tilemap plantableCoordinatesTilemap = GameObject.FindWithTag("PlantableTilemap").GetComponent<Tilemap>();
        plantableCoordinatesTilemap.ClearAllTiles();
        if (plantableCoordinatesAreVisible){
            foreach (Vector3Int coordinate in plantableCoordinates) plantableCoordinatesTilemap.SetTile(coordinate, null);
        }else{
            foreach (Vector3Int coordinate in plantableCoordinates) plantableCoordinatesTilemap.SetTile(coordinate, greenTile);
        }
        plantableCoordinatesAreVisible = !plantableCoordinatesAreVisible;
    }

    public void UpdatePlantableCoordinates(){
        ToggleMapPlantableCoordinates();
        ToggleMapPlantableCoordinates(); //easiest way to update the tiles
    }

    public void UpdateUnavailableCoordinates(){
        ToggleMapUnavailableCoordinates();
        ToggleMapUnavailableCoordinates(); //easiest way to update the tiles
    }

    public void UpdateAllCoordinates(){
        UpdatePlantableCoordinates();
        UpdateUnavailableCoordinates();
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
