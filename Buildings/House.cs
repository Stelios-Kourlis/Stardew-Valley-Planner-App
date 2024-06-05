using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding {
    public TieredBuilding TieredBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }
    public int Tier => TieredBuildingComponent.Tier;

    public ButtonTypes[] BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

    public int MaxTier => TieredBuildingComponent.MaxTier;

    public override void OnAwake() {
        BaseHeight = 6;
        BuildingName = "House";
        InteractableBuildingComponent = new InteractableBuildingComponent(this, new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        });
        base.OnAwake();
        TieredBuildingComponent = new TieredBuilding(this, 4);
    }

    public void SetTier(int tier) {
        TieredBuildingComponent.SetTier(tier);
        if (tier == 4) UpdateTexture(TieredBuildingComponent.Atlas.GetSprite($"House3"));//4 has same sprite as 3
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return Tier switch {
            1 => new List<MaterialInfo> { },
            2 => new List<MaterialInfo>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            },
            3 => new List<MaterialInfo>{
                new(65_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            4 => new List<MaterialInfo>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public string AddToBuildingData() {
        return $"{Tier}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetTier(int.Parse(data[0]));
    }
}
