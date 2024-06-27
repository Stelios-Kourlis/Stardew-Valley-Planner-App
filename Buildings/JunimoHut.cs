
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

public class JunimoHut : Building, IRangeEffectBuilding, IExtraActionBuilding {


    public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }

    public override void OnAwake() {
        BuildingName = "Junimo Hut";
        BaseHeight = 2;
        base.OnAwake();
        RangeEffectBuildingComponent = new RangeEffectBuilding(this);
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>(){
        new(20000, Materials.Coins),
        new(200, Materials.Stone),
        new(9, Materials.Starfruit),
        new(100, Materials.Fiber)
    };
    }

    protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(new Vector3Int(position.x - 7, position.y - 8, 0), 17, 17).ToArray());
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        RangeEffectBuildingComponent.HideEffectRange();
    }

    // protected override void OnMouseEnter() { //TODO: Fix this
    //     Vector3Int lowerLeftCorner = BaseCoordinates[0];
    //     RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(new Vector3Int(lowerLeftCorner.x - 7, lowerLeftCorner.y - 8, 0), 17, 17).ToArray());
    // }

    // protected override void OnMouseExit() {
    //     RangeEffectBuildingComponent.HideEffectRange();
    // }

    public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);
    public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
}