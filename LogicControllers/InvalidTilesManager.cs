using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {
    Invalid,
    Plantable,
    NeutralTreeDisabled, // there are for use in interiors where although the tile  is neutral, trees cant be planted
    Neutral,
}

public class SpecialCoordinateRect {
    public readonly string identifier;
    private readonly TileType[,] tiles;
    public Vector3Int offset = Vector3Int.zero;
    public int Width => tiles.GetLength(1);
    public int Height => tiles.GetLength(0);

    public SpecialCoordinateRect(string identifier, TileType[,] tiles) {
        this.identifier = identifier;
        this.tiles = tiles;
    }

    public SpecialCoordinateRect(string identifier, TileType[,] tiles, Vector3Int offset) {
        this.identifier = identifier;
        this.tiles = tiles;
        this.offset = offset;
    }

    public HashSet<Vector3Int> ToHashSet() {
        HashSet<Vector3Int> coordinates = new();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                coordinates.Add(new Vector3Int(x + offset.x, y + offset.y, 0));
            }
        }
        return coordinates;
    }

    public HashSet<Vector3Int> ToHashSet(TileType type) {
        HashSet<Vector3Int> coordinates = new();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (tiles[x, y] == type) coordinates.Add(new Vector3Int(x + offset.x, y + offset.y, 0));
            }
        }
        return coordinates;
    }
}

public class SpecialCoordinatesCollection {
    private readonly List<SpecialCoordinateRect> specialTileSets = new();

    public HashSet<Vector3Int> GetTilesOfType(TileType type) {
        HashSet<Vector3Int> tiles = new();
        HashSet<Vector3Int> invalidList = new();
        foreach (SpecialCoordinateRect specialTileSet in specialTileSets.Reverse<SpecialCoordinateRect>()) {
            HashSet<Vector3Int> specialTiles = specialTileSet.ToHashSet(type);
            foreach (Vector3Int tile in specialTiles) {
                if (tiles.Contains(tile)) invalidList.Add(tile);
                else if (!invalidList.Contains(tile)) tiles.Add(tile);
            }
        }
        return tiles;
    }

    public void AddSpecialTileSet(SpecialCoordinateRect specialTileSet) {
        specialTileSets.Add(specialTileSet);

    }

    public void RemoveSpecialTileSet(string identifier) {
        specialTileSets.RemoveAll(specialTileSet => specialTileSet.identifier == identifier);
    }

    public void ClearAll() {
        specialTileSets.Clear();
    }

    public HashSet<Vector3Int> GetAllCoordinates() {
        HashSet<Vector3Int> allCoordinates = new();
        HashSet<Vector3Int> invalidList = new();
        foreach (SpecialCoordinateRect specialTileSet in specialTileSets) {
            HashSet<Vector3Int> specialTiles = specialTileSet.ToHashSet();
            foreach (Vector3Int tile in specialTiles) {
                if (allCoordinates.Contains(tile)) invalidList.Add(tile);
                else if (!invalidList.Contains(tile)) allCoordinates.Add(tile);
            }
        }
        return allCoordinates;
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
            if (BuildingController.isInsideBuilding.Key) return BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetTilesOfType(TileType.Invalid);
            else return BuildingController.specialCoordinates.GetTilesOfType(TileType.Invalid);
        }
    }
    public HashSet<Vector3Int> AllPlantableTiles {
        get {
            if (BuildingController.isInsideBuilding.Key) return BuildingController.isInsideBuilding.Value.InteriorSpecialTiles.GetTilesOfType(TileType.Plantable);
            else return BuildingController.specialCoordinates.GetTilesOfType(TileType.Plantable);
        }
    }

    public void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EnterableBuildingComponent.EnteredOrExitedBuilding += UpdateAllCoordinates;
        BuildingController.anyBuildingPositionChanged += UpdateAllCoordinates;
    }
    public void ToggleMapUnavailableCoordinates() {
        // if (BuildingController.isInsideBuilding.Key) { ToggleMapUnavailableCoordinatesForBuildingInside(); return; }
        SpecialCoordinatesCollection currentCoordinates = BuildingController.isInsideBuilding.Key ? BuildingController.isInsideBuilding.Value.InteriorSpecialTiles : BuildingController.specialCoordinates;
        Tilemap unavailableCoordinatesTilemap = GameObject.FindWithTag("InvalidTilemap").GetComponent<Tilemap>();
        unavailableCoordinatesTilemap.ClearAllTiles();
        if (unavailableCoordinatesAreVisible) {
            foreach (Vector3Int coordinate in currentCoordinates.GetTilesOfType(TileType.Invalid)) unavailableCoordinatesTilemap.SetTile(coordinate, null);
        }
        else {
            foreach (Vector3Int coordinate in currentCoordinates.GetTilesOfType(TileType.Invalid)) unavailableCoordinatesTilemap.SetTile(coordinate, redTileSprite);
        }
        unavailableCoordinatesAreVisible = !unavailableCoordinatesAreVisible;
    }

    public void ToggleMapPlantableCoordinates() {
        SpecialCoordinatesCollection currentCoordinates = BuildingController.isInsideBuilding.Key ? BuildingController.isInsideBuilding.Value.InteriorSpecialTiles : BuildingController.specialCoordinates;
        Tilemap plantableCoordinatesTilemap = GameObject.FindWithTag("PlantableTilemap").GetComponent<Tilemap>();
        plantableCoordinatesTilemap.ClearAllTiles();
        if (plantableCoordinatesAreVisible) {
            foreach (Vector3Int coordinate in currentCoordinates.GetTilesOfType(TileType.Plantable)) plantableCoordinatesTilemap.SetTile(coordinate, null);
        }
        else {
            foreach (Vector3Int coordinate in currentCoordinates.GetTilesOfType(TileType.Plantable)) plantableCoordinatesTilemap.SetTile(coordinate, greenTileSprite);
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
