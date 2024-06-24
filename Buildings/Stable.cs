using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building, IInteractableBuilding {
    private InteractableBuildingComponent interactableBuildingComponent;

    public List<ButtonTypes> BuildingInteractions => interactableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => interactableBuildingComponent.ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Stable";
        BaseHeight = 2;
        // interactableBuildingComponent = new InteractableBuildingComponent(this);
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return new List<MaterialInfo>(){
            new(10000, Materials.Coins),
            new(100, Materials.Hardwood),
            new(5, Materials.IronBar)
        };
    }
}
