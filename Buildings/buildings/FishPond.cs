using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Linq;


public class FishPond : Building, IExtraActionBuilding {
    public FishPondComponent FishPondComponent => gameObject.GetComponent<FishPondComponent>();
    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;
    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Fish Pond";
        BaseHeight = 5;
        base.OnAwake();
        gameObject.AddComponent<FishPondComponent>();


    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry> {
            new(5_000, Materials.Coins),
            new(200, Materials.Wood),
            new(5, Materials.Seaweed),
            new(5, Materials.GreenAlgae)
        };
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        FishPondComponent.SetDecoTilemapLocation(position);
        FishPondComponent.SetWaterTilemapLocation(position);
    }

    public void PerformExtraActionsOnPickup() {
        FishPondComponent.ClearDecoTilemap();
        FishPondComponent.ClearWaterTilemap();
    }

    public void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        FishPondComponent.SetDecoTilemapLocation(position); //just copy the color of the tilemap
        FishPondComponent.SetDecoTilemapColor(Tilemap.color);
        FishPondComponent.SetWaterTilemapLocation(position);
        FishPondComponent.SetWaterTilemapColor(Tilemap.color);
        // decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        // decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;

        // waterTilemapObject.GetComponent<Tilemap>().SetTiles(GetAreaAroundPosition(position, Height, Width).ToArray(), SplitSprite(atlas.GetSprite("FishPondBottom")));
        // waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    public void PerformExtraActionsOnPickupPreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        if (BaseCoordinates.Contains(currentCell)) {
            FishPondComponent.SetDecoTilemapColor(SEMI_TRANSPARENT);
            FishPondComponent.SetWaterTilemapColor(SEMI_TRANSPARENT);
        }
        else {
            FishPondComponent.SetDecoTilemapColor(OPAQUE);
            FishPondComponent.SetWaterTilemapColor(OPAQUE);
        }
    }

    public void PerformExtraActionsOnDeletePreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        if (BaseCoordinates.Contains(currentCell)) {
            FishPondComponent.SetDecoTilemapColor(SEMI_TRANSPARENT_INVALID);
            FishPondComponent.SetWaterTilemapColor(SEMI_TRANSPARENT_INVALID);
        }
        else {
            FishPondComponent.SetDecoTilemapColor(OPAQUE);
            FishPondComponent.SetWaterTilemapColor(OPAQUE);
        }
    }
}
