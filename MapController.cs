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
    private SpriteAtlas atlas;
    public MapTypes CurrentMapType {get; private set;}
    Tile redTile;
    private bool unavailableCoordinatesAreVisible = false;
    private bool addingInvalidTiles = false;
    private Vector3Int startTile;
    // Start is called before the first frame update
    void Start(){
        atlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        SetMap(MapTypes.Normal);
    
        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;
    }

    public void Update(){
        if (Input.GetKeyUp(KeyCode.A)){
            Debug.Log("Ctrl A");
            addingInvalidTiles = !addingInvalidTiles;
            GetNotificationManager().SendNotification($"Toggled addingInvalidTiles, its now {addingInvalidTiles}");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && addingInvalidTiles){
            startTile = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }if (Input.GetKeyUp(KeyCode.Mouse0) && addingInvalidTiles){
            Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector3Int[] tileList;
            if (startTile != currentCell) tileList = GetAllCoordinatesInArea(startTile, currentCell).ToArray();
            else tileList = new Vector3Int[]{currentCell};
            foreach (Vector3Int tile in tileList) AddTileToCurrentMapInvalidTiles(tile);
        }
    }

    private void AddTileToCurrentMapInvalidTiles(Vector3Int tile){
        string path = "Assets/Resources/Maps/GingerIsland.txt";
        File.AppendAllText(path, $"{tile.x} {tile.y} {tile.z}\n");
        Debug.Log($"Added {tile.x} {tile.y} {tile.z} to {CurrentMapType}");
        GameObject map = GameObject.FindWithTag("CurrentMap");
        TileBuildingData dataScript = map.GetComponent<TileBuildingData>();
        GetBuildingController().GetUnavailableCoordinates().Add(tile);
        UpdateUnavailableCoordinates();
    }

    public void SetMap(MapTypes mapType) {
        CurrentMapType = mapType;
        BuildingController buildingController = GetBuildingController();
        buildingController.DeleteAllBuildings();
        buildingController.GetUnavailableCoordinates().Clear();
        GameObject map = GameObject.FindWithTag("CurrentMap");
        map.name = mapType.ToString() + "Map";
        Vector3Int mapPos = new Vector3Int(-27, -36, 0);
        Sprite mapTexture = atlas.GetSprite(map.name);
        Vector3Int[] spriteArrayCoordinates = GetAreaAroundPosition(mapPos, (int) mapTexture.textureRect.height / 16, (int) mapTexture.textureRect.width / 16).ToArray();
        Tile[] tiles = SplitSprite(mapTexture);
        TileBuildingData dataScript = map.AddComponent(typeof(TileBuildingData)) as TileBuildingData;
        dataScript.AddInvalidTilesData(mapType);
        Tilemap mapTilemap = map.GetComponent<Tilemap>();
        mapTilemap.ClearAllTiles();
        mapTilemap.SetTiles(spriteArrayCoordinates, tiles);
        // ToggleRedTiles();
        // ToggleRedTiles();
        if (mapType != MapTypes.GingerIsland) buildingController.PlaceHouse(1);
        GetCamera().GetComponent<CameraController>().UpdateCameraBounds();
    }

    public void SetMap(String mapType){
        MapTypes mapTypeEnum = MapTypes.Normal;
        try{
            mapTypeEnum = (MapTypes) Enum.Parse(typeof(MapTypes), mapType);
        }catch(Exception e){
           Debug.Log("Map type |" + mapType + "| does not exist" + e);
        }
        SetMap(mapTypeEnum);
    }

    public void ToggleMapUnavailableCoordinates(){
        HashSet<Vector3Int> unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates();
        Tilemap unavailableCoordinatesTilemap = GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>();
        if (unavailableCoordinatesAreVisible){
            foreach (Vector3Int coordinate in unavailableCoordinates) unavailableCoordinatesTilemap.SetTile(coordinate, null);
        }else{
            foreach (Vector3Int coordinate in unavailableCoordinates) unavailableCoordinatesTilemap.SetTile(coordinate, redTile);
        }
        unavailableCoordinatesAreVisible = !unavailableCoordinatesAreVisible;
    }

    public void UpdateUnavailableCoordinates(){
        ToggleMapUnavailableCoordinates();
        ToggleMapUnavailableCoordinates(); //easiest way to update the tiles
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
