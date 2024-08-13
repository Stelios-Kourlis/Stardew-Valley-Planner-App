using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class SlimeHutch : Building, IExtraActionBuilding {
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Slime Hutch";
        BaseHeight = 4;
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>().AddInteriorInteractions(new HashSet<ButtonTypes>()); //no interior interactions
        base.OnAwake();
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        EnterableBuildingComponent.AddBuildingInterior();
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>(){
            new(10000, Materials.Coins),
            new(500, Materials.Stone),
            new(10, Materials.RefinedQuartz),
            new(1, Materials.IridiumBar)
        };
    }

    public void OnMouseRightClick() {
        var buttonParent = InteractableBuildingComponent.ButtonParentGameObject;
        buttonParent.SetActive(!buttonParent.activeSelf);
    }
}
