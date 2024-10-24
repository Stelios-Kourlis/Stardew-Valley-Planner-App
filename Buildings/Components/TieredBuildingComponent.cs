using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.U2D;
using static Utility.ClassManager;

[Serializable]
public class BuildingTier {
    public int tier;
    public List<MaterialCostEntry> costToUpgradeToThatTier;
}

[RequireComponent(typeof(Building))]
public class TieredBuildingComponent : BuildingComponent {

    /// <summary> The current tier of the building, to change it use SetTier() instead </summary>
    [field: SerializeField] public int Tier { get; set; } = 1;
    public int MaxTier => tiers.Count();
    // private SpriteAtlas atlas;
    public Action<int> tierChanged;
    public BuildingTier[] tiers;


    public TieredBuildingComponent SetTierData(BuildingTier[] tiers) {
        this.tiers = tiers;
        for (int tier = 1; tier <= MaxTier; tier++) {
            string tierStr = tier switch {
                1 => "ONE",
                2 => "TWO",
                3 => "THREE",
                4 => "FOUR",
                _ => "INVALID"
            };
            gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding((ButtonTypes)Enum.Parse(typeof(ButtonTypes), $"TIER_{tierStr}"));
        }
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        SetTier(1);
    }

    public List<MaterialCostEntry> GetMaterialsNeeded() {
        return tiers[Tier - 1].costToUpgradeToThatTier;
    }


    public virtual void SetTier(int tier) {
        Tier = tier;
        Building.UpdateTexture(Building.Atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
        tierChanged?.Invoke(tier);
    }

    public void Load(BuildingScriptableObject bso) {
        SetTierData(bso.tiers);
    }

    public override BuildingData.ComponentData Save() {
        return new(typeof(TieredBuildingComponent), new() { new JProperty("Tier", Tier) });
    }

    public override void Load(BuildingData.ComponentData data) {
        SetTier(data.GetComponentDataPropertyValue<int>("Tier"));
    }
}
