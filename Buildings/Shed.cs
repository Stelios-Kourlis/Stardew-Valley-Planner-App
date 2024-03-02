using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Shed : Building, ITieredBuilding {

    private SpriteAtlas atlas;
    private int tier = 0;

    public new void Start(){
        baseHeight = 3;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.PAINT,
            ButtonTypes.ENTER,
        };
        base.Start();
        atlas = Resources.Load("Buildings/ShedAtlas") as SpriteAtlas;
        if (tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 1 || tier > 2) throw new System.ArgumentException($"Tier must be between 1 and 2 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"ShedT{tier}"));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(15000, Materials.Coins),
                new MaterialInfo(300, Materials.Wood),
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(35000, Materials.Coins),
                new MaterialInfo(850, Materials.Wood),
                new MaterialInfo(300, Materials.Stone)
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
        // Debug.Log($"Changed tier to {tier}");
    }
    
}
