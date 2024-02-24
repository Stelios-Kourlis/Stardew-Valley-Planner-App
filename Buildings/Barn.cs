using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Barn : Building, ITieredBuilding {
    private SpriteAtlas atlas;
    private int tier = 1;

    public new void Start(){
        baseHeight = 4;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
        base.Start();
        atlas = Resources.Load("Buildings/BarnAtlas") as SpriteAtlas;
        UpdateTexture(atlas.GetSprite($"BarnA_{tier}"));
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"BarnA_{tier}"));
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return tier switch{
            1 => new Dictionary<Materials, int>{
                {Materials.Coins, 6_000},
                {Materials.Wood, 350},
                {Materials.Stone, 150}
            },
            2 => new Dictionary<Materials, int>{
                {Materials.Coins, 6_000 + 12_000},
                {Materials.Wood, 350 + 450},
                {Materials.Stone, 150 + 200}
            },
            3 => new Dictionary<Materials, int>{
                {Materials.Coins, 6_000 + 12_000 + 25_000},
                {Materials.Wood, 350 + 450 + 550},
                {Materials.Stone, 150 + 200 + 300}
            },
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        base.RecreateBuildingForData(x, y);
        ChangeTier(int.Parse(data[0]));

    }
}
