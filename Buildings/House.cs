using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building {

    public enum MarriageCandidate {
        Emily,
        Haley,
        Leah,
        Maru,
        Penny,
        Abigail,
        Alex,
        Elliott,
        Harvey,
        Sam,
        Sebastian,
        Shane,
        Crobus
    }

    public TieredBuildingComponent TieredBuildingComponent { get; private set; }

    public EnterableBuildingComponent EnterableBuildingComponent { get; private set; }

    public HashSet<ButtonTypes> BuildingInteractions => gameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;

    public GameObject ButtonParentGameObject => gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;
    private MarriageCandidate? spouse = null;

    public override void OnAwake() {
        BaseHeight = 6;
        BuildingName = "House";
        base.OnAwake();
        TieredBuildingComponent = gameObject.AddComponent<TieredBuildingComponent>().SetMaxTier(3);
        EnterableBuildingComponent = gameObject.AddComponent<EnterableBuildingComponent>();
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        // if (tier == 4) UpdateTexture(TieredBuildingComponent.Atlas.GetSprite($"House3"));//4 has same sprite as 3
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return TieredBuildingComponent.Tier switch {
            1 => new List<MaterialCostEntry> { new("Free") },
            2 => new List<MaterialCostEntry>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            },
            3 => new List<MaterialCostEntry>{
                new(10_000 + 65_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            4 => new List<MaterialCostEntry>{
                new(10_000 + 65_000 + 100_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }

    public string GetExtraData() {
        return $"{TieredBuildingComponent.Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetTier(int.Parse(data[0]));
    }

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();

    public void OnMouseRightClick() {
        if (!BuildingController.isInsideBuilding.Key) ButtonParentGameObject.SetActive(!ButtonParentGameObject.activeSelf);
    }
}
