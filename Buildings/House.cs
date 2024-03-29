using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding {

    private SpriteAtlas atlas;
    public int Tier {get; private set;} = 0;
    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        baseHeight = 6;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        };
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>("Buildings/HouseAtlas");
        if (Tier == 0) ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 1 || tier > 4) throw new System.ArgumentException($"Tier must be between 1 and 4 (got {tier})");
        Tier = tier;
        if (Tier == 4) Tier = 3;//Tier 3 and 4 share the same outside texture
        UpdateTexture(atlas.GetSprite($"House{tier}"));
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

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return Tier switch{
            1 => new List<MaterialInfo>{},
            2 => new List<MaterialInfo>{
                new MaterialInfo(10_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(60_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
                new MaterialInfo(150, Materials.Hardwood),
            },
            4 => new List<MaterialInfo>{
                new MaterialInfo(160_000, Materials.Coins),
                new MaterialInfo(450, Materials.Wood),
                new MaterialInfo(150, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        // Debug.Log($"Recreating house at {x},{y}");
        OnAwake();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }
}
