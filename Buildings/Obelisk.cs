using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class Obelisk : Building, IMultipleTypeBuilding<Obelisk.Types>{

    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent {get; private set;}

    public enum Types{
        WaterObelisk,
        DesertObelisk,
        IslandObelisk,
        EarthObelisk
    }
    public override string TooltipMessage => "Right Click To Cycle Through Obelisks";

    public Types Type => MultipleTypeBuildingComponent.Type;

    public override void OnAwake(){
        BaseHeight = 3;
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        // Debug.Log($"1: {sprite == null}");
        base.OnAwake(); 
        // Debug.Log($"2: {sprite == null}");
    }
    
    public override List<MaterialInfo> GetMaterialsNeeded(){
        return MultipleTypeBuildingComponent.Type switch{
            Types.WaterObelisk => new System.Collections.Generic.List<MaterialInfo>{
                new(500000, Materials.Coins),
                new(5, Materials.IridiumBar),
                new(10, Materials.Clam),
                new(10, Materials.Coral)
            },
            Types.DesertObelisk => new List<MaterialInfo>{
                new(1000000, Materials.Coins),
                new(20, Materials.IridiumBar),
                new(10, Materials.Coconut),
                new(10, Materials.CactusFruit)
            },
            Types.IslandObelisk => new List<MaterialInfo>{
                new(1000000, Materials.Coins),
                new(10, Materials.IridiumBar),
                new(10, Materials.DragonTooth),
                new(10, Materials.Banana)
            },
            Types.EarthObelisk => new List<MaterialInfo>{
                new(500000, Materials.Coins),
                new(10, Materials.IridiumBar),
                new(10, Materials.EarthCrystal)
            },
            _ => new List<MaterialInfo>()
        };
    }
    
    protected override void OnMouseRightClick(){
        MultipleTypeBuildingComponent.CycleType();
    }
    
    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{(int) MultipleTypeBuildingComponent.Type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        MultipleTypeBuildingComponent.SetType((Types) int.Parse(data[0]));
    }

    public GameObject[] CreateButtonsForAllTypes(){
        return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    }

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Types type) => MultipleTypeBuildingComponent.SetType(type);
}
