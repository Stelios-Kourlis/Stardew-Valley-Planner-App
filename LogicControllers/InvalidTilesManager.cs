using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public enum TileType {
    Invalid,
    Plantable,
    NeutralTreeDisabled, // there are for use in interiors where although the tile  is neutral, trees cant be planted
    Neutral,
}

public class SpecialCoordinate {
    public Vector3Int position;
    public TileType type;

    public SpecialCoordinate(Vector3Int position, TileType type) {
        this.position = position;
        this.type = type;
    }
}

public class SpecialCoordinateRect {
    public readonly string identifier;
    HashSet<SpecialCoordinate> specialCoordinates;

    public SpecialCoordinateRect(string identifier, HashSet<SpecialCoordinate> specialCoordinates) {
        this.identifier = identifier;
        this.specialCoordinates = specialCoordinates;
    }

    public SpecialCoordinateRect(string identifier, HashSet<Vector3Int> coordinates, TileType type) {
        this.identifier = identifier;
        specialCoordinates = new HashSet<SpecialCoordinate>();
        foreach (Vector3Int specialCoordinate in coordinates) specialCoordinates.Add(new SpecialCoordinate(specialCoordinate, type));
    }

    public HashSet<SpecialCoordinate> GetSpecialCoordinates() {
        return specialCoordinates;
    }

    public void AddOffset(Vector3Int offset) {
        foreach (SpecialCoordinate specialCoordinate in specialCoordinates) specialCoordinate.position += offset;
    }

    public HashSet<SpecialCoordinate> GetSpecialCoordinates(TileType type) {
        HashSet<SpecialCoordinate> coordinates = new();
        foreach (SpecialCoordinate specialCoordinate in specialCoordinates) if (specialCoordinate.type == type) coordinates.Add(specialCoordinate);
        return coordinates;
    }
}

public class SpecialCoordinatesCollection {
    private readonly List<SpecialCoordinateRect> specialTileSets = new();

    public HashSet<Vector3Int> GetAllCoordinatesOfType(TileType type) {
        HashSet<Vector3Int> tiles = new();
        HashSet<Vector3Int> invalidList = new();
        foreach (SpecialCoordinateRect specialTileSet in specialTileSets.Reverse<SpecialCoordinateRect>()) {
            var allCoords = specialTileSet.GetSpecialCoordinates();
            var coordsOfCorrentType = specialTileSet.GetSpecialCoordinates(type);

            foreach (SpecialCoordinate specialCoordinate in coordsOfCorrentType) tiles.Add(specialCoordinate.position);
            foreach (SpecialCoordinate specialCoordinate in allCoords) invalidList.Add(specialCoordinate.position);
        }
        return tiles;
    }

    public void AddSpecialTileSet(SpecialCoordinateRect specialTileSet) {
        if (specialTileSet.GetSpecialCoordinates().Count == 0) throw new Exception($"Tried to add tileset with no tiles");
        specialTileSets.Add(specialTileSet);
    }

    public void RemoveSpecialTileSet(string identifier) {
        if (specialTileSets.Any(specialTileSet => specialTileSet.identifier == identifier)) Debug.Log($"Removed {identifier}");
        Debug.Log(specialTileSets.Count);
        specialTileSets.RemoveAll(specialTileSet => specialTileSet.identifier == identifier);
        Debug.Log(specialTileSets.Count);
    }

    public void ClearAll() {
        specialTileSets.Clear();
    }

    public int Count() {
        return specialTileSets.Count;
    }

}

public class InvalidTilesManager : MonoBehaviour {

    [SerializeField] private Tile redTileSprite;
    [SerializeField] private Tile greenTileSprite;
    private bool unavailableCoordinatesAreVisible = false;
    private bool plantableCoordinatesAreVisible = false;
    public static InvalidTilesManager Instance { get; private set; }
    public HashSet<Vector3Int> AllInvalidTiles {
        get {
            return BuildingController.isInsideBuilding.Key ? BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetAllCoordinatesOfType(TileType.Invalid) : BuildingController.specialCoordinates.GetAllCoordinatesOfType(TileType.Invalid);
            // if (BuildingController.isInsideBuilding.Key) return BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetAllCoordinatesOfType(TileType.Invalid);
            // else return BuildingController.specialCoordinates.GetAllCoordinatesOfType(TileType.Invalid);
        }
    }
    public HashSet<Vector3Int> AllPlantableTiles {
        get {
            // return BuildingController.isInsideBuilding.Key ? BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetAllCoordinatesOfType(TileType.Plantable) : BuildingController.specialCoordinates.GetAllCoordinatesOfType(TileType.Plantable);
            HashSet<Vector3Int> plantableCoordinates;
            if (BuildingController.isInsideBuilding.Key) plantableCoordinates = BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetAllCoordinatesOfType(TileType.Plantable);
            else plantableCoordinates = BuildingController.specialCoordinates.GetAllCoordinatesOfType(TileType.Plantable);
            var invalidCoordinates = AllInvalidTiles;
            plantableCoordinates.RemoveWhere(coord => invalidCoordinates.Contains(coord));
            return plantableCoordinates;
        }
    }

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EnterableBuildingComponent.EnteredOrExitedBuilding += UpdateAllCoordinates;
        BuildingController.anyBuildingPositionChanged += UpdateAllCoordinates;
    }
    public void ToggleMapUnavailableCoordinates() {
        Tilemap unavailableCoordinatesTilemap = GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>();
        unavailableCoordinatesTilemap.ClearAllTiles();
        if (unavailableCoordinatesAreVisible) {
            foreach (Vector3Int coordinate in AllInvalidTiles) unavailableCoordinatesTilemap.SetTile(coordinate, null);
        }
        else {
            foreach (Vector3Int coordinate in AllInvalidTiles) unavailableCoordinatesTilemap.SetTile(coordinate, redTileSprite);
        }
        unavailableCoordinatesAreVisible = !unavailableCoordinatesAreVisible;
    }

    public void ToggleMapPlantableCoordinates() {
        Tilemap plantableCoordinatesTilemap = GameObject.FindWithTag("PlantableTilemap").GetComponent<Tilemap>();
        plantableCoordinatesTilemap.ClearAllTiles();
        if (plantableCoordinatesAreVisible) {
            foreach (Vector3Int coordinate in AllPlantableTiles) plantableCoordinatesTilemap.SetTile(coordinate, null);
        }
        else {
            foreach (Vector3Int coordinate in AllPlantableTiles) plantableCoordinatesTilemap.SetTile(coordinate, greenTileSprite);
        }
        plantableCoordinatesAreVisible = !plantableCoordinatesAreVisible;
    }

    public void UpdatePlantableCoordinates() {
        ToggleMapPlantableCoordinates();
        ToggleMapPlantableCoordinates(); //easiest way to update the tiles
    }

    public void UpdateUnavailableCoordinates() {
        ToggleMapUnavailableCoordinates();
        ToggleMapUnavailableCoordinates(); //easiest way to update the tiles
    }

    public void UpdateAllCoordinates() {
        UpdatePlantableCoordinates();
        UpdateUnavailableCoordinates();
    }

    public void ToggleAllCoordinates() {
        ToggleMapUnavailableCoordinates();
        ToggleMapPlantableCoordinates();
    }
}
