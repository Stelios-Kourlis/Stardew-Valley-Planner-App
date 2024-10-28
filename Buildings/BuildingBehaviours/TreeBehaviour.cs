using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.SpriteManager;
using static Utility.TilemapManager;

public class TreeBehaviour : BuildingBehaviourExtension {
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
        // Building.Width = 1;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        // Building.Tilemap.ClearAllTiles();
        // Vector3Int[] mouseoverEffectArea = GetRectAreaFromPoint(lowerLeftCorner - new Vector3Int(1, 0, 0), Building.Height, Building.Width).ToArray();
        // Building.Tilemap.SetTiles(mouseoverEffectArea, SplitSprite(Building.Sprite));
    }

    public override void OnStart(Building building) {
        Building = building;
    }
}
