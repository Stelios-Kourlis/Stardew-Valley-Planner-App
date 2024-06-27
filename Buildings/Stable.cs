using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building {
    private InteractableBuildingComponent InteractableBuildingComponent { get; set; }

    public List<ButtonTypes> BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Stable";
        BaseHeight = 2;
        base.OnAwake();
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>(){
            new(10000, Materials.Coins),
            new(100, Materials.Hardwood),
            new(5, Materials.IronBar)
        };
    }
}
