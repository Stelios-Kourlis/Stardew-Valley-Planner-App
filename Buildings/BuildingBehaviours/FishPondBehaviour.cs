using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPondBehaviour : BuildingBehaviourExtension {
    FishPondComponent fishPondComponent;
    public override void NoPreview() {
        if (Building.CurrentBuildingState == Building.BuildingState.PLACED) fishPondComponent.UpdateTilemapColors();
        else {
            fishPondComponent.ClearDecoTilemap();
            fishPondComponent.ClearWaterTilemap();
        }
    }

    public override void OnDelete() {
        fishPondComponent.ClearDecoTilemap();
        fishPondComponent.ClearWaterTilemap();
    }

    public override void OnDeletePreview() {
        fishPondComponent.UpdateTilemapColors();
    }

    public override void OnDestroy() {
        fishPondComponent.ClearDecoTilemap();
        fishPondComponent.ClearWaterTilemap();
    }

    public override void OnMouseEnter() {
        return;
    }

    public override void OnMouseExit() {
        return;
    }

    public override void OnPickup() {
        fishPondComponent.ClearDecoTilemap();
        fishPondComponent.ClearWaterTilemap();
    }

    public override void OnPickupPreview() {
        fishPondComponent.UpdateTilemapColors();
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        fishPondComponent.SetDecoTilemapLocation(lowerLeftCorner);
        fishPondComponent.SetWaterTilemapLocation(lowerLeftCorner);
        fishPondComponent.UpdateTilemapColors();
        if (fishPondComponent.fish != Fish.PLACE_FISH) fishPondComponent.SetFish(fishPondComponent.fish);
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        fishPondComponent.SetDecoTilemapLocation(lowerLeftCorner);
        fishPondComponent.SetWaterTilemapLocation(lowerLeftCorner);
        fishPondComponent.UpdateTilemapColors();
    }

    public override void OnStart(Building building) {
        fishPondComponent = building.gameObject.GetComponent<FishPondComponent>();
        Building = building;
    }
}
