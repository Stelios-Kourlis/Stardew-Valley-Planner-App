using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding {

    private SpriteAtlas atlas;
    private int tier;

    public new void Start(){
        baseHeight = 6;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        };
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/HouseAtlas");
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 1 || tier > 4) throw new System.ArgumentException($"Tier must be between 1 and 4 (got {tier})");
        this.tier = tier;
        if (tier == 4) tier = 3;//Tier 3 and 4 share the same outside texture
        UpdateTexture(atlas.GetSprite($"HouseT{tier}"));
    }

    protected override void Pickup(){
        return; //Cant pickup house
    }

    public override void Delete(){
        return; //Cant delete house
    }

    protected override void DeletePreview(){
        return; //Cant delete house
    }

    protected override void PickupPreview(){
        return; //Cant pickup house
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return tier switch{
            1 => new Dictionary<Materials, int>{},
            2 => new Dictionary<Materials, int>{
                {Materials.Coins, 10_000},
                {Materials.Wood, 450},
            },
            3 => new Dictionary<Materials, int>{
                {Materials.Coins, 10_000 + 50_000},
                {Materials.Wood, 450},
                {Materials.Hardwood, 150},
            },
            4 => new Dictionary<Materials, int>{
                {Materials.Coins, 10_000 + 50_000 + 100_000},
                {Materials.Wood, 450},
                {Materials.Hardwood, 150},
            },
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
    }
}
