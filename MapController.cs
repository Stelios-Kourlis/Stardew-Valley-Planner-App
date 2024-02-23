using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using UnityEngine.Tilemaps;
using System;

public class MapController : MonoBehaviour{

    MapTypes currentMapType;
    Tile redTile;
    private bool unavailableCoordinatesAreVisible = false;
    // Start is called before the first frame update
    void Start(){
        SetMap(MapTypes.Normal);

        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;
    }

    public void SetMap(MapTypes mapType) {
        currentMapType = mapType;
        BuildingController buildingController = GetBuildingController();
        buildingController.DeleteAllBuildings();
        buildingController.GetUnavailableCoordinates().Clear();
        GameObject map = GameObject.FindWithTag("CurrentMap");
        map.name = mapType.ToString() + "Map";
        Vector3Int mapPos = new Vector3Int(-27, -36, 0);
        Texture2D mapTexture = Resources.Load("Maps/" + mapType.ToString() + "Map") as Texture2D;
        Vector3Int[] spriteArrayCoordinates = GetAreaAroundPosition(mapPos, mapTexture.height / 16, mapTexture.width / 16, true).ToArray();
        Tile[] tiles = SplitSprite(mapTexture);
        TileBuildingData dataScript = map.AddComponent(typeof(TileBuildingData)) as TileBuildingData;
        dataScript.AddInvalidTilesData(mapType.ToString());
        Tilemap mapTilemap = map.GetComponent<Tilemap>();
        mapTilemap.ClearAllTiles();
        mapTilemap.SetTiles(spriteArrayCoordinates, tiles);
        // ToggleRedTiles();
        // ToggleRedTiles();
        buildingController.PlaceHouse(1);
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
