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

public class SpecialCoordinateSet : IEnumerable<Vector3Int>, ICollection<Vector3Int> {
    public readonly string identifier;
    public HashSet<Vector3Int> tiles;
    public readonly TileType type;

    public int Count => tiles.Count;

    public bool IsReadOnly => false;

    public SpecialCoordinateSet(string identifier, IEnumerable<Vector3Int> tiles, TileType type) {
        this.identifier = identifier;
        this.tiles = tiles.ToHashSet();
        this.type = type;
    }

    public SpecialCoordinateSet(string identifier, TileType type) {
        this.identifier = identifier;
        tiles = new HashSet<Vector3Int>();
        this.type = type;
    }

    public SpecialCoordinateSet(string identifier, Vector3Int tiles, TileType type) {
        this.identifier = identifier;
        this.tiles = new HashSet<Vector3Int> { tiles };
        this.type = type;
    }

    public void AddTiles(IEnumerable<Vector3Int> tiles) {
        this.tiles.UnionWith(tiles);
    }

    public IEnumerator<Vector3Int> GetEnumerator() {
        return tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Add(Vector3Int item) {
        tiles.Add(item);
    }

    public void Clear() {
        tiles.Clear();
    }

    public bool Contains(Vector3Int item) {
        return tiles.Contains(item);
    }

    public void CopyTo(Vector3Int[] array, int arrayIndex) {
        throw new System.NotImplementedException();
    }

    public bool Remove(Vector3Int item) {
        return tiles.Remove(item);
    }
}

public class SpecialCoordinatesCollection {
    private readonly List<SpecialCoordinateSet> specialTileSets = new();

    public HashSet<Vector3Int> GetTilesOfType(TileType type) {
        HashSet<Vector3Int> tiles = new();
        foreach (SpecialCoordinateSet specialTileSet in specialTileSets) {
            if (specialTileSet.type == type) tiles.UnionWith(specialTileSet.tiles);
        }
        return tiles;
    }

    public void AddSpecialTileSet(SpecialCoordinateSet specialTileSet) {
        if (specialTileSets.Any(specialTile => specialTile.identifier == specialTileSet.identifier))
            specialTileSets.First(specialTile => specialTile.identifier == specialTileSet.identifier).AddTiles(specialTileSet.tiles);

        else specialTileSets.Add(specialTileSet);

    }

    public void RemoveSpecialTileSet(string identifier) {
        specialTileSets.RemoveAll(specialTileSet => specialTileSet.identifier == identifier);
    }

    public void ClearAll() {
        specialTileSets.Clear();
    }

    public HashSet<Vector3Int> GetAllCoordinates() {
        HashSet<Vector3Int> allCoordinates = new();
        foreach (SpecialCoordinateSet specialTileSet in specialTileSets) {
            allCoordinates.UnionWith(specialTileSet.tiles);
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
