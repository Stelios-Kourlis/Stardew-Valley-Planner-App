using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Barn : Building, ITieredBuilding {
    private SpriteAtlas atlas;
    private int tier = 0;

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
        if (tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"BarnA_{tier}"));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(6_000, Materials.Coins),
                new MaterialInfo(350, Materials.Wood),
                new MaterialInfo(150, Materials.Stone)
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000, Materials.Coins),
                new MaterialInfo(350 + 450, Materials.Wood),
                new MaterialInfo(150 + 200, Materials.Stone)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(6_000 + 12_000 + 25_000, Materials.Coins),
                new MaterialInfo(350 + 450 + 550, Materials.Wood),
                new MaterialInfo(150 + 200 + 300, Materials.Stone)
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
