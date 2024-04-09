using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class Shed : Building, ITieredBuilding {
    public TieredBuilding TieredBuildingComponent {get; private set;}
    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        // TieredBuildingComponent = gameObject.AddComponent<TieredBuilding>();
        TieredBuildingComponent = new TieredBuilding(this, 2);
        BaseHeight = 3;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.PAINT,
            ButtonTypes.ENTER,
        };
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return TieredBuildingComponent.Tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(15000, Materials.Coins),
                new MaterialInfo(300, Materials.Wood),
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(35000, Materials.Coins),
                new MaterialInfo(850, Materials.Wood),
                new MaterialInfo(300, Materials.Stone)
            },
            _ => throw new System.ArgumentException($"Invalid tier {TieredBuildingComponent.Tier}")
        };
    }
    
    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{TieredBuildingComponent.Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        TieredBuildingComponent.SetTier(int.Parse(data[0]));
    }
}
