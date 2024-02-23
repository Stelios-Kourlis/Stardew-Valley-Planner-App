using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding {//todo There is a T4 house with the cellar

    private SpriteAtlas atlas;
    private int tier;

    protected override void Init(){
        baseHeight = 6;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        };
    }

    public new void Start(){
        Init();
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/HouseAtlas");
        PickupBuilding = NoAction;
        DeleteBuilding = NoAction;
        EditPreview = NoAction;
        DeletePreview = NoAction;
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 1 || tier > 4) throw new System.ArgumentException($"Tier must be between 1 and 4 (got {tier})");
        this.tier = tier;
        if (tier == 4) tier = 3;//Tier 3 and 4 share the same outside texture
        UpdateTexture(atlas.GetSprite($"HouseT{tier}"));
    }

    private void NoAction(){
        return; //Cant pickup/delete house
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
