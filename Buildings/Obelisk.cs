using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class Obelisk : MultipleTypeBuilding<Obelisk.Types>{

    public enum Types{
        NO_TYPE,
        WaterObelisk,
        DesertObelisk,
        IslandObelisk,
        EarthObelisk
    }
    public override string TooltipMessage => "Right Click To Cycle Through Obelisks";

    public override void OnAwake(){
        baseHeight = 3;
        if (CurrentType == Types.NO_TYPE) CurrentType = Types.WaterObelisk;
        base.OnAwake(); 
        defaultSprite = atlas.GetSprite("WaterObelisk");
    }
    
    public override List<MaterialInfo> GetMaterialsNeeded(){
        return Type switch{
            Types.WaterObelisk => new List<MaterialInfo>{
                new MaterialInfo(500000, Materials.Coins),
                new MaterialInfo(5, Materials.IridiumBar),
                new MaterialInfo(10, Materials.Clam),
                new MaterialInfo(10, Materials.Coral)
            },
            Types.DesertObelisk => new List<MaterialInfo>{
                new MaterialInfo(1000000, Materials.Coins),
                new MaterialInfo(20, Materials.IridiumBar),
                new MaterialInfo(10, Materials.Coconut),
                new MaterialInfo(10, Materials.CactusFruit)
            },
            Types.IslandObelisk => new List<MaterialInfo>{
                new MaterialInfo(1000000, Materials.Coins),
                new MaterialInfo(10, Materials.IridiumBar),
                new MaterialInfo(10, Materials.DragonTooth),
                new MaterialInfo(10, Materials.Banana)
            },
            Types.EarthObelisk => new List<MaterialInfo>{
                new MaterialInfo(500000, Materials.Coins),
                new MaterialInfo(10, Materials.IridiumBar),
                new MaterialInfo(10, Materials.EarthCrystal)
            },
            _ => new List<MaterialInfo>()
        };
    }
    
    protected override void OnMouseRightClick(){
        CycleType();
    }
    
    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{(int) Type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        SetType((Types) int.Parse(data[0]));
    }
}
