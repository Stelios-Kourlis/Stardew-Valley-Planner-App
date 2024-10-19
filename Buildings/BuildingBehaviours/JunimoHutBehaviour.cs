using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

[Serializable]
public class JunimoHutBehaviour : BuildingBehaviourExtension {

    private RangeEffectBuilding rangeEffectBuildingComponent;
    private Building building;
    public override void OnStart(Building building) {
        this.building = building;
        rangeEffectBuildingComponent = new(building);
        Debug.Log("building assigned");
    }

    public override void OnDelete() {
        rangeEffectBuildingComponent.HideEffectRange();
    }

    public override void OnDeletePreview() {
        rangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(building.Base.x - 7, building.Base.y - 8, 0), 17, 17).ToArray(), Utility.Tiles.Red);
    }

    public override void OnMouseEnter() {
        rangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(building.Base.x - 7, building.Base.y - 8, 0), 17, 17).ToArray(), Utility.Tiles.Green);
    }

    public override void OnMouseExit() {
        rangeEffectBuildingComponent.HideEffectRange();
    }

    public override void NoPreview() {
        rangeEffectBuildingComponent.HideEffectRange();
    }

    public override void OnPickup() {
        rangeEffectBuildingComponent.HideEffectRange();
    }

    public override void OnPickupPreview() {
        rangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(building.Base.x - 7, building.Base.y - 8, 0), 17, 17).ToArray(), Utility.Tiles.Green);
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        rangeEffectBuildingComponent.HideEffectRange();
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        rangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(lowerLeftCorner.x - 7, lowerLeftCorner.y - 8, 0), 17, 17).ToArray(), Utility.Tiles.Green);
    }

    public override void OnDestroy() {
        rangeEffectBuildingComponent.HideEffectRange();
        rangeEffectBuildingComponent = null;
    }
}
