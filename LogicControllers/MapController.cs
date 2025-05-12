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
using System.Threading.Tasks;
using System.Diagnostics;
using Unity.Burst.Intrinsics;

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
    private float mapChangeProgressPrecent;
    private bool backMapLoaded = false, frontMapLoaded = false, isChangingMap = false;
    [SerializeField] private GameObject progressModalPrefab;
    [SerializeField] private GameObject map, mapForeground;

    public static MapController Instance { get; private set; }

    void Awake() {
        Instance = this;
        InitializeMapButtons();
    }

    void Start() {
        StartCoroutine(SetMap(MapTypes.Normal));
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
            mapButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SetMap(type)));
        }
    }

    public IEnumerator SetMap(MapTypes mapType) {

        IEnumerator ShowMapChangeProgress() {
            GameObject progressModal = Instantiate(progressModalPrefab, GetCanvasGameObject().transform);
            progressModal.transform.Find("ProgressText").GetComponent<TMP_Text>().text = mapChangeProgressPrecent / 2 + (backMapLoaded ? 50 : 0) + "%";
            while (!frontMapLoaded) {
                progressModal.transform.Find("Bar").Find("BarFill").GetComponent<RectTransform>().sizeDelta = new(mapChangeProgressPrecent * 3 + (backMapLoaded ? 300 : 0), progressModal.transform.Find("Bar").Find("BarFill").GetComponent<RectTransform>().sizeDelta.y);
                progressModal.transform.Find("ProgressText").GetComponent<TMP_Text>().text = mapChangeProgressPrecent / 2 + (backMapLoaded ? 50 : 0) + "%";
                yield return null;
            }
            Destroy(progressModal);
        }

        IEnumerator PopulateTilemapFromFileData(Tilemap tilemap, TextAsset file) {
            tilemap.ClearAllTiles();
            string[] mapData = file.text.Split(",\n", StringSplitOptions.RemoveEmptyEntries);

            Dictionary<Vector3Int, int> tileDataDict = new(); //each coordinate with the tile that should be there

            foreach (string line in mapData) { //Parse the tile data from the file, fast no need to worry about performance here
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
            Array.Sort(tiles, (tile1, tile2) => { //Sort tiles by number
                int num1 = int.Parse(tile1.name.Replace("Tile", ""));
                int num2 = int.Parse(tile2.name.Replace("Tile", ""));
                return num1.CompareTo(num2);
            });

            int tilesPlaced = 0;
            List<Tile> tilesToPlace = new();
            List<Vector3Int> tilePositions = new();
            int targetFrameRate = 30; //fps
            long maxFrameTimeMs = 1000 / targetFrameRate;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (KeyValuePair<Vector3Int, int> tileData in tileDataDict) { //place the tiles in the tilemap
                tilesToPlace.Add(tiles[tileData.Value]);
                tilePositions.Add(tileData.Key);
                // tilemap.SetTile(tileData.Key, tiles[tileData.Value]);
                tilesPlaced++;
                // if (tilesPlaced % 100 == 0) UnityEngine.Debug.Log(tilesPlaced);
                if (stopwatch.ElapsedMilliseconds >= maxFrameTimeMs || tileData.Key == tileDataDict.Last().Key) { //next frame
                    stopwatch.Restart();
                    tilemap.SetTiles(tilePositions.ToArray(), tilesToPlace.ToArray());
                    tilePositions.Clear();
                    tilesToPlace.Clear();
                    mapChangeProgressPrecent = (float)tilesPlaced / tileDataDict.Count * 100;
                    yield return null;
                }
            }

            if (tilePositions.Count > 0) {
                tilemap.SetTiles(tilePositions.ToArray(), tilesToPlace.ToArray());
                tilePositions.Clear();
                tilesToPlace.Clear();
            }


            yield return null;
            tiles = null;
            // tilemap.SetTiles(tileDataDict.Keys.ToArray(), tilesToPlace.ToArray());
            Resources.UnloadUnusedAssets();

            int minX = tileDataDict.Keys.Min(v => v.x);
            int maxX = tileDataDict.Keys.Max(v => v.x);
            int minY = tileDataDict.Keys.Min(v => v.y);
            int maxY = tileDataDict.Keys.Max(v => v.y);
            Vector3Int origin = new(minX, minY, 0);
            Vector3Int size = new(maxX - minX + 1, maxY - minY + 1, 1);

            tilemap.size = size;
            tilemap.origin = origin;
            tilemap.ResizeBounds();
        }

        if (CurrentMapType == mapType && MapScene.name != null) { UnityEngine.Debug.Log("Error Loading Map"); yield break; }
        if (isChangingMap) {
            NotificationManager.Instance.SendNotification("Map change aleady in progress", NotificationManager.Icons.ErrorIcon);
            yield break;
        }

        isChangingMap = true;
        Scene oldScene = MapScene;
        MapScene = SceneManager.CreateScene($"Map Scene {mapType}");

        GameObject grid = new("Grid");
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        map.transform.SetParent(grid.transform);
        mapForeground.transform.SetParent(grid.transform);

        SceneManager.MoveGameObjectToScene(grid, MapScene);
        if (oldScene.name != null) SceneManager.UnloadSceneAsync(oldScene);


        CurrentMapType = mapType;
        BuildingController.DeleteAllBuildings(true);
        BuildingController.mapSpecialCoordinates.ClearAll();
        if (BuildingController.playerLocation.isInsideBuilding) BuildingController.playerLocation.enterableBuildingComponent.ExitBuildingInterior();

        UndoRedoController.ClearLogs();

        map.name = mapType.ToString() + "Map";

        // UnityEngine.Debug.Log($"Loading {mapType} map");
        InputHandler.Instance.keybindsShouldRegister = false;
        CameraController.Instance.LockCamera();
        backMapLoaded = false;
        frontMapLoaded = false;
        StartCoroutine(ShowMapChangeProgress());
        mapForeground.SetActive(false);
        TextAsset backData = Resources.Load<TextAsset>($"MapData/{mapType}MapBackTileData");
        yield return PopulateTilemapFromFileData(map.GetComponent<Tilemap>(), backData);
        map.GetComponent<Tilemap>().ResizeBounds();
        map.GetComponent<Tilemap>().CompressBounds();
        backMapLoaded = true;
        Resources.UnloadAsset(backData);
        TextAsset frontData = Resources.Load<TextAsset>($"MapData/{mapType}MapFrontTileData");
        mapForeground.SetActive(true);
        yield return PopulateTilemapFromFileData(mapForeground.GetComponent<Tilemap>(), frontData);
        frontMapLoaded = true;
        Resources.UnloadAsset(frontData);


        SpecialCoordinateRect specialCoordinates = GetSpecialCoordinateSet(map.name);
        BuildingController.mapSpecialCoordinates.ClearAll();
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        BuildingController.mapSpecialCoordinates.AddSpecialTileSet(specialCoordinates);
        InvalidTilesManager.Instance.UpdateAllCoordinates();

        BuildingController.SetCurrentTilemapTransform(map.transform);
        BuildingController.InitializeMap();

        InputHandler.Instance.keybindsShouldRegister = true;
        CameraController.Instance.UnlockCamera();
        isChangingMap = false;
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
            MapTypes.GingerIsland => new Vector3Int(90, 69, 0),
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
            MapTypes.Beach => new Vector3Int(34, 93, 0),
            MapTypes.FourCorners => new Vector3Int(30, 43, 0),
            MapTypes.Ranching => new Vector3Int(88, 20, 0),
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
