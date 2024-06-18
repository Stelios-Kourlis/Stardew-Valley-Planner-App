using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.U2D;
using static Utility.ClassManager;

public class TieredBuildingComponent : MonoBehaviour {

    /// <summary> The current tier of the building, to change it use SetTier() instead </summary>
    public int Tier { get; set; } = 1;
    public int MaxTier { get; private set; }
    private SpriteAtlas atlas;
    private Building Building => gameObject.GetComponent<Building>();
    private bool buildingHasOtherInterfaces;

    public TieredBuildingComponent SetMaxTier(int maxTier) {
        MaxTier = maxTier;
        return this;
    }

    public void Awake() {
        if (Building is not ITieredBuilding) throw new System.ArgumentException($"Building {Building.GetType()} does not implement ITieredBuilding");
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        buildingHasOtherInterfaces = BuildingHasMoreThanOneBuildingInterface(Building, typeof(ITieredBuilding));
        atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType()}Atlas");
        Debug.Assert(atlas != null, $"Could not load atlas for {Building.GetType()} ({Building.GetType()}Atlas)");
        // if (buildingHasOtherInterfaces) return;
        // SetTier(1);
    }

    public virtual void SetTier(int tier) {
        if (tier < 0 || tier > MaxTier) throw new System.ArgumentException($"Tier for {Building.GetType()} must be between 1 and {MaxTier} (got {tier})");
        Tier = tier;
        if (buildingHasOtherInterfaces) return;
        // Debug.Log($"Building {Building.GetType()} has no itherfaces other than {GetType()}");
        Building.UpdateTexture(atlas.GetSprite($"{Building.GetType()}{tier}"));
    }
}
