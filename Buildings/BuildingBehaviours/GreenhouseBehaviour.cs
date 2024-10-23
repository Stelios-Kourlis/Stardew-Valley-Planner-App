using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using UnityEditor;

public class GreenhouseBehaviour : BuildingBehaviourExtension {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;
    public override void NoPreview() {
        if (Building.CurrentBuildingState == Building.BuildingState.PLACED) porchTilemapObject.GetComponent<Tilemap>().color = Building.OPAQUE;
        else porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public override void OnDelete() {
        MonoScript.Destroy(porchTilemapObject);
    }

    public override void OnDeletePreview() {
        porchTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnDestroy() {
        MonoScript.Destroy(porchTilemapObject);
    }

    public override void OnMouseEnter() {
        return;
    }

    public override void OnMouseExit() {
        return;
    }

    public override void OnPickup() {
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet("GreenhousePorch");
    }

    public override void OnPickupPreview() {
        porchTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        Vector3Int porchBottomRight = lowerLeftCorner + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetRectAreaFromPoint(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = InvalidTilesManager.Instance.CurrentCoordinateSet.GetAllCoordinatesOfType(TileType.Invalid);
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) return;
        porchTilemapObject.GetComponent<Tilemap>().color = Building.OPAQUE;
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        InvalidTilesManager.Instance.CurrentCoordinateSet.AddSpecialTileSet(new("GreenhousePorch", porchCoordinates.ToHashSet(), TileType.Invalid));
        porchTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = Building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        Vector3Int porchBottomRight = lowerLeftCorner + new Vector3Int(2, -2, 0);
        Vector3Int[] porchCoordinates = GetRectAreaFromPoint(porchBottomRight, 2, 3).ToArray();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnStart(Building building) {
        Building = building;
        porchSprite = Resources.Load<Sprite>("Buildings/GreenhousePorch");
        porchTilemapObject = CreateTilemapObject(building.transform, 0, "Porch");
    }

    public override bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) {
        var porchArea = GetRectAreaFromPoint(new Vector3Int(position.x + 2, position.y - 2, position.z), 2, 3);
        if (porchArea.Intersect(InvalidTilesManager.Instance.AllInvalidTiles).Any()) {
            errorMessage = "Not enough space for Porch";
            return false;
        }
        errorMessage = "";
        return true;
    }
}
