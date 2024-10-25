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
        Combat,
        FourCorners,
        Beach,
        GingerIsland,
        Ranching

    }

    [SerializeField] private SpriteAtlas mapAtlas;
    public MapTypes CurrentMapType { get; private set; }
    public GameObject UIButtonPrefab;
    public Scene MapScene { get; private set; }

    public static MapController Instance { get; private set; }

    void Start() {
        Instance = this;
        InitializeMapButtons();

        // mapAtlas = Resources.Load<SpriteAtlas>("Maps/MapAtlas");
        // MapScene = null;
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
            mapButton.transform.Find("Text").GetComponent<TMP_Text>().fontSize = 50;
            mapButton.GetComponent<Button>().onClick.AddListener(() => SetMap(type));
        }
    }

    public void SetMap(MapTypes mapType) {
        Debug.Log(MapScene.name);
        if (MapScene.name != null) SceneManager.UnloadSceneAsync(MapScene);
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
            MapTypes.Ranching => new Vector3Int(49, 18, 0),
            _ => new Vector3Int(32, 12, 0),
        };
    }

    public Vector3Int GetShippingBinPosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(44, 29, 0),
            MapTypes.Beach => new Vector3Int(44, 59, 0),
            MapTypes.GingerIsland => new Vector3Int(63, 34, 0),
            MapTypes.Ranching => new Vector3Int(61, 20, 0),
            _ => new Vector3Int(44, 14, 0),
        };
    }

    public Vector3Int GetGreenhousePosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(9, 9, 0),
            MapTypes.Beach => new Vector3Int(-13, 54, 0),
            MapTypes.Ranching => new Vector3Int(10, 14, 0),
            _ => new Vector3Int(-2, 13, 0),
        };
    }
    public Vector3Int GetPetBowlPosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(22, 2, 0),
            MapTypes.Beach => new Vector3Int(51, 51, 0),
            MapTypes.Ranching => new Vector3Int(64, 23, 0),
            _ => new Vector3Int(26, 20, 0),
        };
    }

    public Vector3Int GetCoopPosition(out List<Vector3Int> fencePositions) {
        fencePositions = new List<Vector3Int>(){
            new(35,31,0),
            new(35,30,0),
            new(35,29,0),
            new(35,28,0),
            new(35,27,0),
            new(35,26,0),
            new(35,24,0),
            new(35,23,0),
            new(35,22,0),
            new(35,21,0),
            new(35,20,0),
            new(35,19,0),
            new(35,18,0),
            new(34,18,0),
            new(33,18,0),
            new(32,18,0),
            new(31,18,0),
            new(30,18,0),
            new(29,18,0),
            new(28,18,0),
            new(27,18,0),
            new(26,18,0),
            new(25,18,0),
            new(24,18,0),
            new(23,18,0),
            new(22,18,0),
            new(21,18,0),
            new(20,18,0),
            new(20,19,0),
            new(20,20,0),
            new(20,21,0),
            new(20,22,0)
        };
        return new Vector3Int(27, 27, 0);
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
