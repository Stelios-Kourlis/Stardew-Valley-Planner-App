using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

public class SprinklerBehaviour : BuildingBehaviourExtension {
    RangeEffectBuilding rangeEffectBuilding;
    public override void NoPreview() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnDelete() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnDeletePreview() {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Normal" or "Normal Enricher" => GetCrossAroundPosition(Building.Base).ToArray(),
            "Quality" or "Quality Enricher" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Iridium" or "Iridium Enricher" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Normal Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Quality Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Iridium Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 3).ToArray(),
            _ => throw new System.ArgumentException($"Invalid type {Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType}")
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea, Utility.Tiles.Red);
    }

    public override void OnDestroy() {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnMouseEnter() {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Normal" or "Normal Enricher" => GetCrossAroundPosition(Building.Base).ToArray(),
            "Quality" or "Quality Enricher" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Iridium" or "Iridium Enricher" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Normal Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Quality Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Iridium Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 3).ToArray(),
            _ => throw new System.ArgumentException($"Invalid type {Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType}")
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
            "Normal" or "Normal Enricher" => GetCrossAroundPosition(Building.Base).ToArray(),
            "Quality" or "Quality Enricher" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Iridium" or "Iridium Enricher" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Normal Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 1).ToArray(),
            "Quality Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 2).ToArray(),
            "Iridium Pressure Nozzle" => GetAreaAroundPosition(Building.Base, 3).ToArray(),
            _ => throw new System.ArgumentException($"Invalid type {Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType}")
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea);
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        rangeEffectBuilding.HideEffectRange();
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        Vector3Int[] coverageArea = Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType switch {
            "Normal" or "Normal Enricher" => GetCrossAroundPosition(lowerLeftCorner).ToArray(),
            "Quality" or "Quality Enricher" => GetAreaAroundPosition(lowerLeftCorner, 1).ToArray(),
            "Iridium" or "Iridium Enricher" => GetAreaAroundPosition(lowerLeftCorner, 2).ToArray(),
            "Normal Pressure Nozzle" => GetAreaAroundPosition(lowerLeftCorner, 1).ToArray(),
            "Quality Pressure Nozzle" => GetAreaAroundPosition(lowerLeftCorner, 2).ToArray(),
            "Iridium Pressure Nozzle" => GetAreaAroundPosition(lowerLeftCorner, 3).ToArray(),
            _ => throw new System.ArgumentException($"Invalid type {Building.GetComponent<MultipleTypeBuildingComponent>().CurrentType}")
        };
        rangeEffectBuilding.ShowEffectRange(coverageArea);
    }

    public override void OnStart(Building building) {
        Building = building;
        rangeEffectBuilding = new(building);
    }
}
