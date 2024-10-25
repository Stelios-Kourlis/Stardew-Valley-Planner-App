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
        Ranching

    }

    private SpriteAtlas mapAtlas;
    public MapTypes CurrentMapType { get; private set; }
    public GameObject UIButtonPrefab;
    public Scene MapScene { get; private set; }

    public static MapController Instance { get; private set; }

    void Start() {
        Instance = this;
        InitializeMapButtons();

        mapAtlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        SetMap(MapTypes.Normal);


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
        Vector3Int mapPos = new(-27, -36, 0);
        Sprite mapTexture = mapAtlas.GetSprite(map.name);

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

        BuildingController.SetCurrentTilemapTransform(map.transform);
        if (mapType != MapTypes.GingerIsland) BuildingController.InitializeMap();

        GameObject grid = new("Grid");
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        map.transform.SetParent(grid.transform);

        SceneManager.MoveGameObjectToScene(grid, MapScene);
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
            MapTypes.Beach => new Vector3Int(44, 59, 0),
            _ => new Vector3Int(44, 14, 0),
        };
    }

    public Vector3Int GetGreenhousePosition() {
        return CurrentMapType switch {
            // MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            MapTypes.Beach => new Vector3Int(-13, 54, 0),
            _ => new Vector3Int(-2, 13, 0),
        };
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
