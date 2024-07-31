using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.U2D;
using static Utility.ClassManager;

public class TieredBuildingComponent : MonoBehaviour {

    /// <summary> The current tier of the building, to change it use SetTier() instead </summary>
    [field: SerializeField] public int Tier { get; set; } = 1;
    [field: SerializeField] public int MaxTier { get; private set; } = 1;
    private SpriteAtlas atlas;
    private Building Building => gameObject.GetComponent<Building>();
    public Action<int> tierChanged;

    public TieredBuildingComponent SetMaxTier(int maxTier) {
        MaxTier = maxTier;
        for (int tier = 1; tier <= MaxTier; tier++) {
            string tierStr = tier switch {
                1 => "ONE",
                2 => "TWO",
                3 => "THREE",
                4 => "ONE", //todo placeholder for lack of icon for tier 4
                _ => "INVALID"
            };
            gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding((ButtonTypes)Enum.Parse(typeof(ButtonTypes), $"TIER_{tierStr}"));
        }
        return this;
    }

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType()}Atlas");
        Debug.Assert(atlas != null, $"Could not load atlas for {Building.GetType()} ({Building.GetType()}Atlas)");
        SetTier(1);
    }


    public virtual void SetTier(int tier) {
        if (tier < 0 || tier > MaxTier) throw new System.ArgumentException($"Tier for {Building.GetType()} must be between 1 and {MaxTier} (got {tier})");
        Tier = tier;
        tierChanged?.Invoke(tier);
        Building.UpdateTexture(atlas.GetSprite($"{gameObject.GetComponent<InteractableBuildingComponent>().GetBuildingSpriteName()}"));
    }
}
