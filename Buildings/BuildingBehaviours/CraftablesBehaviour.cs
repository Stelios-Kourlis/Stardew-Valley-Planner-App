using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

public class CraftablesBehaviour : BuildingBehaviourExtension {
    RangeEffectBuilding rangeEffectBuilding;
    static int miniObeliskCount;
    public override void NoPreview() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnDelete() {
        rangeEffectBuilding.HideEffectRange();
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MiniObelisk") miniObeliskCount -= 1;
    }

    public override void OnDeletePreview() {
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MushroomLog") rangeEffectBuilding.ShowEffectRange(GetAreaAroundPosition(Building.Base, 3).ToArray(), Utility.Tiles.Red);
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "Beehouse") rangeEffectBuilding.ShowEffectRange(GetRangeOfBeehouse(Building.Base).ToArray(), Utility.Tiles.Red);
    }

    public override void OnDestroy() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnMouseEnter() {
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MushroomLog") rangeEffectBuilding.ShowEffectRange(GetAreaAroundPosition(Building.Base, 3).ToArray());
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "Beehouse") rangeEffectBuilding.ShowEffectRange(GetRangeOfBeehouse(Building.Base).ToArray());
    }

    public override void OnMouseExit() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnPickup() {
        rangeEffectBuilding.HideEffectRange();
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MiniObelisk") miniObeliskCount -= 1;
    }

    public override void OnPickupPreview() {
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MushroomLog") rangeEffectBuilding.ShowEffectRange(GetAreaAroundPosition(Building.Base, 3).ToArray());
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "Beehouse") rangeEffectBuilding.ShowEffectRange(GetRangeOfBeehouse(Building.Base).ToArray());
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        rangeEffectBuilding.HideEffectRange();
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MiniObelisk") miniObeliskCount += 1;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "MushroomLog") rangeEffectBuilding.ShowEffectRange(GetAreaAroundPosition(lowerLeftCorner, 3).ToArray());
        if (Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType == "Beehouse") rangeEffectBuilding.ShowEffectRange(GetRangeOfBeehouse(lowerLeftCorner).ToArray());
    }

    public override void OnStart(Building building) {
        Building = building;
        rangeEffectBuilding = new(building);
    }

    public override bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) {
        if (miniObeliskCount == 2) {
            errorMessage = "Can only have a max of 2 Mini Obelisks";
            return false;
        }
        errorMessage = "";
        return true;
    }
}
