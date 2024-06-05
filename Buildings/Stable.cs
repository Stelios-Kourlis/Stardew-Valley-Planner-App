using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building, IInteractableBuilding {
    private InteractableBuildingComponent interactableBuildingComponent;

    public ButtonTypes[] BuildingInteractions => interactableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => interactableBuildingComponent.ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Stable";
        BaseHeight = 2;
        interactableBuildingComponent = new InteractableBuildingComponent(this, new ButtonTypes[]{
            ButtonTypes.PAINT
        });
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
