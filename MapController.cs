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
    MapTypes currentMapType;
    Tile redTile;
    private bool unavailableCoordinatesAreVisible = false;
    private bool addingInvalidTiles = false;
    // Start is called before the first frame update
    void Start(){
        atlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        SetMap(MapTypes.Normal);
    
        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;
    }

    public void Update(){
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyUp(KeyCode.A)){
            addingInvalidTiles = !addingInvalidTiles;
            GetNotificationManager().SendNotification($"Toggled addingInvalidTiles, its now {addingInvalidTiles}");
        }
        else
        if (Input.GetKeyUp(KeyCode.Mouse0) && addingInvalidTiles) AddTileToCurrentMapInvalidTiles();
    }

    private void AddTileToCurrentMapInvalidTiles(){
        string path = "Assets/Resources/Maps/GingerIsland.txt";
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        File.AppendAllText(path, $"{currentCell.x} {currentCell.y} {currentCell.z}\n");
        Debug.Log($"Added {currentCell.x} {currentCell.y} {currentCell.z} to {currentMapType}");
        GameObject map = GameObject.FindWithTag("CurrentMap");
        TileBuildingData dataScript = map.GetComponent<TileBuildingData>();
        GetBuildingController().GetUnavailableCoordinates().Add(currentCell);
        UpdateUnavailableCoordinates();
    }

    public void SetMap(MapTypes mapType) {
        currentMapType = mapType;
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

    public MapTypes GetCurrentMapType() { return currentMapType; }

}
