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
    Wall,
    NeutralBuildingDisabled, //There are tiles where craftables can be placed but buildings cant
}

public class SpecialCoordinate {
    public Vector3Int position;
    public TileType type;

    public SpecialCoordinate(Vector3Int position, TileType type) {
        this.position = position;
        this.type = type;
    }
}

[Serializable]
public class SpecialCoordinateRect {
    [SerializeField] public string identifier;
    private readonly HashSet<SpecialCoordinate> specialCoordinates;

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

[Serializable]
public class SpecialCoordinatesCollection {
    [SerializeField] private List<SpecialCoordinateRect> specialTileSets = new();

    public HashSet<Vector3Int> GetAllCoordinatesOfType(TileType type) {
        HashSet<Vector3Int> tiles = new();
        HashSet<Vector3Int> invalidList = new();

        foreach (SpecialCoordinateRect specialTileSet in specialTileSets) {
            var allCoords = specialTileSet.GetSpecialCoordinates();
            var coordsOfCorrentType = specialTileSet.GetSpecialCoordinates(type);

            foreach (SpecialCoordinate specialCoordinate in coordsOfCorrentType) if (!invalidList.Contains(specialCoordinate.position)) tiles.Add(specialCoordinate.position);
            foreach (SpecialCoordinate specialCoordinate in allCoords) invalidList.Add(specialCoordinate.position);
        }
        return tiles;
    }

    public void AddSpecialTileSet(SpecialCoordinateRect specialTileSet) {
        if (specialTileSet.GetSpecialCoordinates().Count == 0) return;
        specialTileSets.Insert(0, specialTileSet);
    }

    public void RemoveSpecialTileSet(string identifier) {
        specialTileSets.RemoveAll(specialTileSet => specialTileSet.identifier == identifier);
    }

    public void ClearAll() {
        specialTileSets.Clear();
    }

    /// <summary>
    /// Get the type of a tile
    /// </summary>
    /// <param name="position"></param>
    /// <returns>The type of the tile requested, null if no tile is found at that position</returns>
    public TileType? GetTypeOfTile(Vector3Int position) {
        foreach (SpecialCoordinateRect specialTileSet in specialTileSets) {
            var specialSet = specialTileSet.GetSpecialCoordinates();
            if (specialSet.Any(tile => tile.position == position)) return specialSet.First(tile => tile.position == position).type;
        }
        return null;
    }

    public bool IsOfTypeAtAnyLevel(Vector3Int position, TileType type) {
        SpecialCoordinate coordinate = new(position, type);
        foreach (SpecialCoordinateRect specialTileSet in specialTileSets) {
            if (specialTileSet.GetSpecialCoordinates().Contains(coordinate)) return true;
        }
        return false;
    }
    public int Count() {
        return specialTileSets.Count;
    }

}

public class InvalidTilesManager : MonoBehaviour {

#pragma warning disable IDE0044 // Add readonly modifier

    [SerializeField] private Tile redTileSprite;
    [SerializeField] private Tile greenTileSprite;

#pragma warning restore IDE0044 // Add readonly modifier
    private bool unavailableCoordinatesAreVisible = false;
    private bool plantableCoordinatesAreVisible = false;
    public static InvalidTilesManager Instance { get; private set; }
    public SpecialCoordinatesCollection CurrentCoordinateSet {
        get {
            return BuildingController.isInsideBuilding.Key ? BuildingController.isInsideBuilding.Value.InteriorSpecialTiles : BuildingController.mapSpecialCoordinates;
        }
    }
    public HashSet<Vector3Int> AllInvalidTiles {
        get {
            return CurrentCoordinateSet.GetAllCoordinatesOfType(TileType.Invalid);
        }
    }
    public HashSet<Vector3Int> AllPlantableTiles {
        get {
            HashSet<Vector3Int> plantableCoordinates = CurrentCoordinateSet.GetAllCoordinatesOfType(TileType.Plantable);
            HashSet<Vector3Int> invalidCoordinates = AllInvalidTiles;
            plantableCoordinates.RemoveWhere(coord => invalidCoordinates.Contains(coord));
            return plantableCoordinates;
        }
    }

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EnterableBuildingComponent.EnteredOrExitedAnyBuilding += UpdateAllCoordinates;
        BuildingController.anyBuildingPositionChanged += UpdateAllCoordinates;
    }

    public TileType? GetTypeOfTile(Vector3Int position) {
        position.z = 0;
        return CurrentCoordinateSet.GetTypeOfTile(position);
    }

    public bool IsOfTypeAtAnyLevel(Vector3Int position, TileType type) {
        return CurrentCoordinateSet.IsOfTypeAtAnyLevel(position, type);
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
