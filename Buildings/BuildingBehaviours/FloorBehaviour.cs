using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;

public class FloorBehaviour : BuildingBehaviourExtension {
    private GameObject floorTilemap;
    public override void NoPreview() {
        return;
    }

    public override void OnDelete() {
        return;
    }

    public override void OnDeletePreview() {
        return;
    }

    public override void OnDestroy() {
        return;
    }

    public override void OnMouseEnter() {
        return;
    }

    public override void OnMouseExit() {
        return;
    }

    public override void OnPickup() {
        return;
    }

    public override void OnPickupPreview() {
        return;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        Building.TilemapRenderer.sortingOrder -= 10;
        // floorTilemap.GetComponent<Tilemap>().SetTile(lowerLeftCorner, SplitSprite(Building.Sprite)[0]);
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        Building.TilemapRenderer.sortingOrder -= 10;
    }

    public override void OnStart(Building building) {
        Building = building;

        // floorTilemap = building.transform.parent.Find("FloorTilemap").gameObject;
        // if (floorTilemap == null) {
        //     floorTilemap = new GameObject("FloorTilemap");
        //     floorTilemap.transform.SetParent(building.transform.parent);
        //     floorTilemap.AddComponent<Tilemap>();
        //     floorTilemap.AddComponent<TilemapRenderer>();
        // }
        // building.Tilemap = floorTilemap.GetComponent<Tilemap>();
    }
}
