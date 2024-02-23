using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Shed : Building, ITieredBuilding {

    private SpriteAtlas atlas;
    private int tier;
    

    protected override void Init() {
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.PAINT,
            ButtonTypes.ENTER,
        };
    }

    public new void Start(){
        Init();
        base.Start();
        atlas = Resources.Load("Buildings/ShedAtlas") as SpriteAtlas;
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        this.tier = tier;
        if (tier < 0 || tier > 2) throw new System.ArgumentException($"Tier must be between 1 and 2 (got {tier})");
        UpdateTexture(atlas.GetSprite($"ShedT{tier}"));
    }
    
}
