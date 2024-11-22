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
using Utility;

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

    // [SerializeField] private SpriteAtlas mapAtlas;
    public MapTypes CurrentMapType { get; private set; }
    public GameObject UIButtonPrefab;
    public Scene MapScene { get; private set; }

    public static MapController Instance { get; private set; }

    void Awake() {
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

        void PopulateTilemapFromFileData(Tilemap tilemap, TextAsset file) {
            tilemap.ClearAllTiles();
            string[] mapData = file.text.Split(",\n", StringSplitOptions.RemoveEmptyEntries);
            Dictionary<Vector3Int, int> tileDataDict = new();

            foreach (string line in mapData) {
                string[] parts = line.Split(new[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3) {
                    int x = int.Parse(parts[0].Trim());
                    int y = int.Parse(parts[1].Trim());
                    int value = int.Parse(parts[2].Trim());
                    Vector3Int key = new(x, y, 0);
                    tileDataDict[key] = value;
                }
            }

            Tile[] tiles = Resources.LoadAll<Tile>($"Tiles");
            foreach (KeyValuePair<Vector3Int, int> tileData in tileDataDict) {
                tilemap.SetTile(tileData.Key, tiles.First(tile => tile.name == $"Tile{tileData.Value}"));
            }
            tiles = null;
            Resources.UnloadUnusedAssets();

            tilemap.CompressBounds();
        }

        if (CurrentMapType == mapType && MapScene.name != null) return;

        if (MapScene.name != null) SceneManager.UnloadSceneAsync(MapScene);

        CurrentMapType = mapType;
        BuildingController.DeleteAllBuildings(true);
        BuildingController.mapSpecialCoordinates.ClearAll();
        if (BuildingController.isInsideBuilding.Key) BuildingController.isInsideBuilding.Value.EditBuildingInterior();

        MapScene = SceneManager.CreateScene($"Map Scene {mapType}");
        UndoRedoController.ClearLogs();

        GameObject map = GameObject.FindWithTag("CurrentMap");
        map.name = mapType.ToString() + "Map";

        TextAsset backData = Resources.Load<TextAsset>($"MapData/{mapType}MapBackTileData");
        PopulateTilemapFromFileData(map.GetComponent<Tilemap>(), backData);
        Resources.UnloadAsset(backData);
        TextAsset frontData = Resources.Load<TextAsset>($"MapData/{mapType}MapFrontTileData");
        PopulateTilemapFromFileData(GameObject.FindGameObjectWithTag("MapForeground").GetComponent<Tilemap>(), frontData);
        Resources.UnloadAsset(frontData);


        SpecialCoordinateRect specialCoordinates = GetSpecialCoordinateSet(map.name);
        BuildingController.mapSpecialCoordinates.ClearAll();
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        BuildingController.mapSpecialCoordinates.AddSpecialTileSet(specialCoordinates);
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        BuildingController.SetCurrentTilemapTransform(map.transform);
        if (mapType != MapTypes.GingerIsland) BuildingController.InitializeMap();

        GameObject grid = new("Grid");
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        map.transform.SetParent(grid.transform);
        GameObject.FindGameObjectWithTag("MapForeground").transform.SetParent(grid.transform);

        SceneManager.MoveGameObjectToScene(grid, MapScene);
    }

    public Vector3Int GetHousePosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(59, 63, 0),
            MapTypes.Beach => new Vector3Int(59, 93, 0),
            MapTypes.Ranching => new Vector3Int(76, 54, 0),
            _ => new Vector3Int(59, 48, 0),
        };
    }

    public Vector3Int GetShippingBinPosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(71, 65, 0),
            MapTypes.Beach => new Vector3Int(71, 95, 0),
            MapTypes.GingerIsland => new Vector3Int(90, 70, 0),
            MapTypes.Ranching => new Vector3Int(88, 56, 0),
            _ => new Vector3Int(71, 50, 0),
        };
    }

    public Vector3Int GetGreenhousePosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(36, 45, 0),
            MapTypes.Beach => new Vector3Int(14, 90, 0),
            MapTypes.Ranching => new Vector3Int(37, 50, 0),
            _ => new Vector3Int(25, 49, 0),
        };
    }
    public Vector3Int GetPetBowlPosition() {
        return CurrentMapType switch {
            MapTypes.FourCorners => new Vector3Int(49, 38, 0),
            MapTypes.Beach => new Vector3Int(78, 87, 0),
            MapTypes.Ranching => new Vector3Int(91, 59, 0),
            _ => new Vector3Int(53, 56, 0),
        };
    }

    public Vector3Int GetCavePosition() {
        return CurrentMapType switch {
            MapTypes.Normal => new Vector3Int(34, 58, 0),
            _ => new Vector3Int(34, 58, 0)
        };
    }

    public Vector3Int GetCoopPosition(out List<Vector3Int> fencePositions) {
        fencePositions = new List<Vector3Int>(){
            new(62,67,0),
            new(62,66,0),
            new(62,65,0),
            new(62,64,0),
            new(62,63,0),
            new(62,62,0),
            new(62,61,0),
            new(62,60,0),
            new(62,59,0),
            new(62,58,0),
            new(62,57,0),
            new(62,56,0),
            new(62,55,0),
            new(61,55,0),
            new(60,55,0),
            new(59,55,0),
            new(58,55,0),
            new(57,55,0),
            new(56,55,0),
            new(55,55,0),
            new(54,55,0),
            new(53,55,0),
            new(52,55,0),
            new(51,55,0),
            new(50,55,0),
            new(49,55,0),
            new(48,55,0),
            new(47,55,0),
            new(47,56,0),
            new(47,57,0),
            new(47,58,0),
            new(47,59,0)
        };
        return new Vector3Int(54, 63, 0);
    }

    public MapTypes GetCurrentMapType() { return CurrentMapType; }

}
