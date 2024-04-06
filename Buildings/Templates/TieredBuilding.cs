using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public abstract class TieredBuilding : Building{

    public int Tier {get; protected set;}
    public int MaxTier {get; protected set;}
    protected SpriteAtlas atlas {get; private set;}

    public override void OnAwake(){
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>($"Buildings/{GetType()}Atlas");
        Debug.Assert(atlas != null, $"Could not load atlas for {GetType()} ({GetType()}Atlas)");
        name = GetType().Name;
        if (Tier == 0) ChangeTier(1);
    }
    public virtual void ChangeTier(int tier){
        if (tier < 0 || tier > MaxTier) throw new System.ArgumentException($"Tier for {GetType()} must be between 1 and {MaxTier} (got {tier})");
        Tier = tier;
        UpdateTexture(atlas.GetSprite($"{GetType()}{tier}"));
    }
}
