using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Barn : Building, ITieredBuilding {
    private SpriteAtlas atlas;
    private int tier;

    protected override void Init(){
        baseHeight = 4;
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
        atlas = Resources.Load("Buildings/BarnAtlas") as SpriteAtlas;
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        this.tier = tier;
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        UpdateTexture(atlas.GetSprite($"BarnA_{tier}"));
    }

    // protected override Dictionary<Materials, int> GetMaterialsNeeded(){
    //     return tier switch{
    //         1 => new Dictionary<Materials, int>{
    //             {Materials.Coins, 6_000},
    //             {Materials.Wood, 350},
    //             {Materials.Stone, 150}
    //         },
    //         2 => new Dictionary<Materials, int>{
    //             {Materials.Coins, 6_000 + 12_000},
    //             {Materials.Wood, 350 + 450},
    //             {Materials.Stone, 150 + 200}
    //         },
    //         3 => new Dictionary<Materials, int>{
    //             {Materials.Coins, 6_000 + 12_000 + 25_000},
    //             {Materials.Wood, 350 + 450 + 550},
    //             {Materials.Stone, 150 + 200 + 300}
    //         },
    //         _ => throw new System.ArgumentException($"Invalid tier {tier}")
    //     };
    // }
}
