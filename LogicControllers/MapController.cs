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
using UnityEngine.SceneManagement;
using static Utility.InvalidTileLoader;
using TMPro;
using UnityEngine.UI;

public class MapController : MonoBehaviour {
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

    private enum TileMode {
        nothing,
        addingInvalidTiles,
        removingInvalidTiles,
        addingPlantableTiles,
        removingPlantableTiles,
        showMouseCoordinates
    }

    private SpriteAtlas atlas;
    public MapTypes CurrentMapType { get; private set; }
    public GameObject UIButtonPrefab;
    static Tile redTile;
    static Tile greenTile;
    private Actions currentAction;
    private TileMode tileMode;
    private Vector3Int startTile;
    public Scene MapScene { get; private set; }

    public static MapController Instance { get; private set; }

    void Start() {
        Instance = this;

        atlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        SetMap(MapTypes.Normal);

        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;

        redTile = LoadTile(Utility.Tiles.Red);
        greenTile = LoadTile(Utility.Tiles.Green);

        tileMode = TileMode.nothing;

        InitializeMapButtons();

    }

    public void Update() {//this is for adding invlid tiles and plantable tiles, should never be accesible to the user
        if (Input.GetKeyDown(KeyCode.A) && Input.GetKey(KeyCode.LeftControl)) {
#if UNITY_EDITOR
            tileMode = (TileMode)(((int)tileMode + 1) % Enum.GetValues(typeof(TileMode)).Length);
            NotificationManager.Instance.SendNotification($"Mode set to {tileMode}", NotificationManager.Icons.InfoIcon);
            Debug.Log($"Mode set to {tileMode}");
            if (tileMode == TileMode.nothing) {
                // unavailableCoordinatesAreVisible = true;
                // plantableCoordinatesAreVisible = true;
                InvalidTilesManager.Instance.ToggleAllCoordinates();
                BuildingController.SetCurrentAction(currentAction);
            }
            else {
                // unavailableCoordinatesAreVisible = false;
                // plantableCoordinatesAreVisible = false;
                InvalidTilesManager.Instance.ToggleAllCoordinates();
                currentAction = BuildingController.CurrentAction;
                BuildingController.SetCurrentAction(Actions.DO_NOTHING);
            }
#endif
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && (tileMode != TileMode.nothing)) {
            startTile = GetMousePositionInTilemap();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            Vector3Int currentCell = GetMousePositionInTilemap();
            Vector3Int[] tileList;
            if (startTile != currentCell) tileList = GetAllCoordinatesInArea(startTile, currentCell).ToArray();
            // else tileList = new Vector3Int[] { currentCell };
            // if (tileMode == TileMode.addingInvalidTiles) foreach (Vector3Int tile in tileList) AddTileToCurrentMapInvalidTiles(tile);
            // else if (tileMode == TileMode.addingPlantableTiles) foreach (Vector3Int tile in tileList) AddTileToCurrentMapPlantableTiles(tile);
            // else if (tileMode == TileMode.removingInvalidTiles) foreach (Vector3Int tile in tileList) RemoveTileFromCurrentMapInvalidTiles(tile);
            // else if (tileMode == TileMode.removingPlantableTiles) foreach (Vector3Int tile in tileList) RemoveTileFromCurrentMapPlantableTiles(tile);
            else if (tileMode == TileMode.showMouseCoordinates) Debug.Log(currentCell);
        }
    }

    private void InitializeMapButtons() {
        Transform mapContent = GetSettingsModal().transform.Find("TabContent").Find("Maps").Find("SelectMap").Find("ScrollArea").Find("Content");
        if (mapContent == null) throw new Exception("Map content not found");

        foreach (MapTypes type in Enum.GetValues(typeof(MapTypes))) {
            GameObject mapButton = Instantiate(UIButtonPrefab, mapContent);
            mapButton.name = type.ToString();
            mapButton.GetComponent<ContentSizeFitter>().enabled = false;
            mapButton.transform.Find("Text").GetComponent<TMP_Text>().text = System.Text.RegularExpressions.Regex.Replace(type.ToString(), "(?<!^)([A-Z])", " $1");
            mapButton.GetComponent<Button>().onClick.AddListener(() => SetMap(type));
        }
    }

    public void SetMap(MapTypes mapType) {
        CurrentMapType = mapType;
        BuildingController.DeleteAllBuildings(true);
        BuildingController.mapSpecialCoordinates.ClearAll();

        MapScene = SceneManager.CreateScene($"Map Scene {mapType}");

        GameObject map = GameObject.FindWithTag("CurrentMap");
        map.name = mapType.ToString() + "Map";
        BuildingController.SetCurrentTilemapTransform(map.transform);
        Vector3Int mapPos = new(-27, -36, 0);
        Sprite mapTexture = atlas.GetSprite(map.name);

        Vector3Int[] spriteArrayCoordinates = GetRectAreaFromPoint(mapPos, (int)mapTexture.textureRect.height / 16, (int)mapTexture.textureRect.width / 16).ToArray();
        Tile[] tiles = SplitSprite(mapTexture);
        SpecialCoordinateRect specialCoordinates = GetSpecialCoordinateSet(map.name);
        BuildingController.mapSpecialCoordinates.ClearAll();


        Tilemap mapTilemap = map.GetComponent<Tilemap>();
        mapTilemap.ClearAllTiles();
        mapTilemap.SetTiles(spriteArrayCoordinates, tiles);
        mapTilemap.CompressBounds();
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        BoundsInt bounds = mapTilemap.cellBounds;
        Vector3Int lowerLeft = new(int.MaxValue, int.MaxValue, 0);
        foreach (var pos in bounds.allPositionsWithin) {
            if (mapTilemap.HasTile(pos)) {
                if (pos.x < lowerLeft.x || (pos.x == lowerLeft.x && pos.y < lowerLeft.y)) {
                    lowerLeft = pos;
                }
            }
        }

        specialCoordinates.AddOffset(lowerLeft);
        BuildingController.mapSpecialCoordinates.AddSpecialTileSet(specialCoordinates);
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        if (mapType != MapTypes.GingerIsland) BuildingController.InitializeMap();

        GameObject grid = new("Grid");
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        map.transform.SetParent(grid.transform);

        SceneManager.MoveGameObjectToScene(grid, MapScene);
    }


    public void SetMap(string mapType) {
        MapTypes mapTypeEnum = MapTypes.Normal;
        try {
            mapTypeEnum = (MapTypes)Enum.Parse(typeof(MapTypes), mapType);
        }
        catch (Exception e) {
            Debug.Log("Map type |" + mapType + "| does not exist" + e);
        }
        SetMap(mapTypeEnum);
    }

    public Vector3Int GetHousePosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            MapTypes.Beach => new Vector3Int(32, 57, 0),
            _ => new Vector3Int(32, 12, 0),
        };
    }

    public Vector3Int GetShippingBinPosition() {//todo add missing positions
        return CurrentMapType switch {
            // MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            // MapController.MapTypes.Beach => new Vector3Int(32, 57, 0),
            _ => new Vector3Int(44, 14, 0),
        };
    }

    public Vector3Int GetGreenhousePosition() {
        return CurrentMapType switch {
            // MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            // MapController.MapTypes.Beach => new Vector3Int(32, 57, 0),
            _ => new Vector3Int(-2, 13, 0),
        };
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
