using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
/// <summary>
/// Methods to handle buildings with multiple tiers
/// </summary>
public class TieredBuilding{
    /// <summary> The current tier of the building, to change it use SetTier() instead </summary>
    public int Tier {get; private set;}
    public int MaxTier {get; private set;}
    private SpriteAtlas Atlas {get; set;}
    private Building Building {get; set;}

    public TieredBuilding(Building building, int maxTier){
        if (building == null) throw new System.Exception($"Building is null");
        Building = building;
        MaxTier = maxTier;
        Atlas = Resources.Load<SpriteAtlas>($"Buildings/{Building.GetType()}Atlas");
        Debug.Assert(Atlas != null, $"Could not load atlas for {Building.GetType()} ({Building.GetType()}Atlas)");
        Debug.Assert(MaxTier > 1, $"You forgot to set MaxTier for {Building.GetType()}, if max tier is 1, you can remove this component");
        SetTier(1);
    }

    public virtual void SetTier(int tier){
        if (tier < 0 || tier > MaxTier) throw new System.ArgumentException($"Tier for {Building.GetType()} must be between 1 and {MaxTier} (got {tier})");
        Tier = tier;
        Building.UpdateTexture(Atlas.GetSprite($"{Building.GetType()}{tier}"));
    }
}
