using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class House : Building, ITieredBuilding {
    public TieredBuilding TieredBuildingComponent {get; private set;}
    public int Tier => TieredBuildingComponent.Tier;

    public override void OnAwake(){
        BaseHeight = 6;
        buildingName = "House";
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER
        };
        base.OnAwake();
        TieredBuildingComponent = new TieredBuilding(this, 4);
    }

    public void SetTier(int tier){
        TieredBuildingComponent.SetTier(tier);
        if (tier == 4) UpdateTexture(TieredBuildingComponent.Atlas.GetSprite($"House3"));//4 has same sprite as 3
    }

//     protected override void Pickup(){
//         return; //Cant pickup house
//     }

    public override void Delete(){
        return; //Cant delete house
    }

    protected override void DeletePreview(){
        return; //Cant delete house
    }

//     protected override void PickupPreview(){
//         return; //Cant pickup house
//     }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return Tier switch{
            1 => new List<MaterialInfo>{},
            2 => new List<MaterialInfo>{
                new(10_000, Materials.Coins),
                new(450, Materials.Wood),
            },
            3 => new List<MaterialInfo>{
                new(65_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            4 => new List<MaterialInfo>{
                new(160_000, Materials.Coins),
                new(450, Materials.Wood),
                new(100, Materials.Hardwood),
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        SetTier(int.Parse(data[0]));
    }
}
