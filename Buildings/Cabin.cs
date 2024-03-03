using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Cabin : Building, ITieredBuilding {

    enum CabinTypes{//add types
        Wood,
        Plank,
        Stone
    }

    private SpriteAtlas atlas;
    private CabinTypes type = CabinTypes.Stone;
    public int Tier {get; private set;}

    public new void Start(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
        base.Start();
        atlas = Resources.Load("Buildings/CabinAtlas") as SpriteAtlas;
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        Tier = tier;
        UpdateTexture(atlas.GetSprite($"{type}Cabin_{tier}"));
    }

    public void CycleType(){
        type = type switch{
            CabinTypes.Wood => CabinTypes.Plank,
            CabinTypes.Plank => CabinTypes.Stone,
            CabinTypes.Stone => CabinTypes.Wood,
            _ => throw new System.Exception("Invalid Cabin Type")
        };
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){//Completely wrong
        throw new System.NotImplementedException();
        // return tier switch{
        //     1 => new Dictionary<Materials, int>{
        //         {Materials.Coins, 6_000},
        //         {Materials.Wood, 350},
        //         {Materials.Stone, 150}
        //     },
        //     2 => new Dictionary<Materials, int>{
        //         {Materials.Coins, 6_000 + 12_000},
        //         {Materials.Wood, 350 + 450},
        //         {Materials.Stone, 150 + 200}
        //     },
        //     3 => new Dictionary<Materials, int>{
        //         {Materials.Coins, 6_000 + 12_000 + 25_000},
        //         {Materials.Wood, 350 + 450 + 550},
        //         {Materials.Stone, 150 + 200 + 300}
        //     },
        //     _ => throw new System.ArgumentException($"Invalid tier {tier}")
        // };
    }

    public override string GetBuildingData(){
        throw new System.NotImplementedException();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data)
    {
        throw new System.NotImplementedException();
    }
}
