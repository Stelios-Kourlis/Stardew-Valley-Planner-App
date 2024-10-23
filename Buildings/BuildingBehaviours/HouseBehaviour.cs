using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using UnityEditor;

public class HouseBehaviour : BuildingBehaviourExtension {

    private GameObject mailboxTilemapObject;
    private Sprite mailboxSprite;
    public override void NoPreview() {
        if (Building.CurrentBuildingState == Building.BuildingState.PLACED) mailboxTilemapObject.GetComponent<Tilemap>().color = Building.OPAQUE;
        else mailboxTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public override void OnDelete() {
        Building.Destroy(mailboxTilemapObject);
        InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet("GreenhousePorch");
    }

    public override void OnDeletePreview() {
        mailboxTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnDestroy() {
        Building.Destroy(mailboxTilemapObject);
        InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet("GreenhousePorch");
    }

    public override void OnMouseEnter() {
        return;
    }

    public override void OnMouseExit() {
        return;
    }

    public override void OnPickup() {
        mailboxTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet("HouseMailbox");
    }

    public override void OnPickupPreview() {
        mailboxTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        Vector3Int mailboxBottomRight = lowerLeftCorner + new Vector3Int(Building.Width, 0, 0);
        Vector3Int[] mailboxCoordinates = GetRectAreaFromPoint(mailboxBottomRight, 2, 1).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = InvalidTilesManager.Instance.CurrentCoordinateSet.GetAllCoordinatesOfType(TileType.Invalid);
        if (unavailableCoordinates.Contains(mailboxBottomRight)) return;
        mailboxTilemapObject.GetComponent<Tilemap>().color = Building.OPAQUE;
        mailboxTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        mailboxTilemapObject.GetComponent<Tilemap>().SetTiles(mailboxCoordinates, SplitSprite(mailboxSprite));
        InvalidTilesManager.Instance.CurrentCoordinateSet.AddSpecialTileSet(new("HouseMailbox", new HashSet<Vector3Int>() { mailboxBottomRight }, TileType.Invalid));
        mailboxTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = Building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        Vector3Int mailboxBottomRight = lowerLeftCorner + new Vector3Int(Building.Width, 0, 0);
        Vector3Int[] mailboxCoordinates = GetRectAreaFromPoint(mailboxBottomRight, 2, 1).ToArray();
        mailboxTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        mailboxTilemapObject.GetComponent<Tilemap>().SetTiles(mailboxCoordinates, SplitSprite(mailboxSprite));
        mailboxTilemapObject.GetComponent<Tilemap>().color = Building.Tilemap.color;
    }

    public override void OnStart(Building building) {
        Building = building;
        mailboxSprite = Resources.Load<Sprite>("Buildings/Mailbox");
        mailboxTilemapObject = CreateTilemapObject(building.transform, 0, "Porch");
    }

    public override bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) {
        Vector3Int mailboxBase = position + new Vector3Int(Building.Width, 0, 0);
        if (InvalidTilesManager.Instance.AllInvalidTiles.Contains(mailboxBase)) {
            errorMessage = "Not enough space for mailbox";
            return false;
        }
        errorMessage = "";
        return true;
    }
}
