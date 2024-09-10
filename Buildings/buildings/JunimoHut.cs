
using System.Collections.Generic;
using UnityEngine;
using static Utility.TilemapManager;

public class JunimoHut : Building, IExtraActionBuilding {


    public RangeEffectBuilding RangeEffectBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

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
        RangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(position.x - 7, position.y - 8, 0), 17, 17).ToArray());
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        RangeEffectBuildingComponent.HideEffectRange();
    }

    public void OnMouseEnter() {
        Vector3Int lowerLeftCorner = BaseCoordinates[0];
        RangeEffectBuildingComponent.ShowEffectRange(GetRectAreaFromPoint(new Vector3Int(lowerLeftCorner.x - 7, lowerLeftCorner.y - 8, 0), 17, 17).ToArray());
    }

    public void OnMouseExit() {
        RangeEffectBuildingComponent.HideEffectRange();
    }
}