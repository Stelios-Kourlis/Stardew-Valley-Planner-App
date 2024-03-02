using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Coop : Building, ITieredBuilding {
    private SpriteAtlas atlas;
    private int tier = 0;

    public new void Start(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT,
            ButtonTypes.ADD_ANIMAL
        };
        base.Start();
        atlas = Resources.Load("Buildings/CoopAtlas") as SpriteAtlas;
        if (tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"CoopAtlas_{tier}"));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(4_000, Materials.Coins),
                new MaterialInfo(300, Materials.Wood),
                new MaterialInfo(100, Materials.Stone)
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(4_000 + 10_000, Materials.Coins),
                new MaterialInfo(300 + 400, Materials.Wood),
                new MaterialInfo(100 + 150, Materials.Stone)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(4_000 + 10_000 + 20_000, Materials.Coins),
                new MaterialInfo(300 + 400 + 500, Materials.Wood),
                new MaterialInfo(100 + 150 + 200, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }
}
