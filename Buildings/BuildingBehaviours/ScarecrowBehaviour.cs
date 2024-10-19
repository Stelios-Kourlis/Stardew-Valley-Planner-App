using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

public class ScarecrowBehaviour : BuildingBehaviourExtension {
    RangeEffectBuilding rangeEffectBuilding;
    public override void OnStart(Building building) {
        Building = building;
        rangeEffectBuilding = new(building);
    }

    public override void NoPreview() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnDelete() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnDeletePreview() {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Deluxe" => GetRangeOfDeluxeScarecrow(Building.Base).ToArray(),
            _ => GetRangeOfScarecrow(Building.Base).ToArray()
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea, Utility.Tiles.Red);
    }

    public override void OnDestroy() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnMouseEnter() {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Deluxe" => GetRangeOfDeluxeScarecrow(Building.Base).ToArray(),
            _ => GetRangeOfScarecrow(Building.Base).ToArray()
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea);
    }

    public override void OnMouseExit() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnPickup() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnPickupPreview() {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Deluxe" => GetRangeOfDeluxeScarecrow(Building.Base).ToArray(),
            _ => GetRangeOfScarecrow(Building.Base).ToArray()
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea);
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Deluxe" => GetRangeOfDeluxeScarecrow(lowerLeftCorner).ToArray(),
            _ => GetRangeOfScarecrow(lowerLeftCorner).ToArray()
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea);
    }

}
