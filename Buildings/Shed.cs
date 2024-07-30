using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using UnityEngine.U2D;

public class Shed : Building {
    public TieredBuildingComponent TieredBuildingComponent { get; private set; }
    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Shed";
        BaseHeight = 3;
        base.OnAwake();

        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(2);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();

    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return TieredBuildingComponent.Tier switch {
            1 => new List<MaterialCostEntry>{
                new(15000, Materials.Coins),
                new(300, Materials.Wood),
            },
            2 => new List<MaterialCostEntry>{
                new(35000, Materials.Coins),
                new(850, Materials.Wood),
                new(300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }

    public string GetExtraData() {
        return $"{TieredBuildingComponent.Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
    }

    public void OnMouseRightClick() {
        ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
