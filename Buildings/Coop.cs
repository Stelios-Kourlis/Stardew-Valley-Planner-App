using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Coop : Building, ITieredBuilding {
    private SpriteAtlas atlas;

    protected override void Init(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
    }

    public new void Start(){
        Init();
        base.Start();
        atlas = Resources.Load("Buildings/CoopAtlas") as SpriteAtlas;
        ChangeTier(2);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        UpdateTexture(atlas.GetSprite($"CoopAtlas_{tier}"));
    }
}
