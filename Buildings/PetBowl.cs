using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetBowl : Building, IMultipleTypeBuilding<PetBowl.Types>{
    public enum Types{
        PetBowl,
        StonePetBowl,
        HayPetBowl
    }
    public override string TooltipMessage => $"{Type}";

    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent { get; set; }

    public Types Type => MultipleTypeBuildingComponent.Type;

    public override void OnAwake(){
        BaseHeight = 2;
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new(){
            new(5_000, Materials.Coins),
            new(25, Materials.Hardwood)
        };
    }

    public override string GetBuildingData(){
        return $"{base.GetBuildingData()}|{Type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x, y, 0));
        SetType((Types)System.Enum.Parse(typeof(Types), data[0]));
    }

    public void CycleType() => MultipleTypeBuildingComponent.CycleType();

    public void SetType(Types type) => MultipleTypeBuildingComponent.SetType(type);

    public GameObject[] CreateButtonsForAllTypes() => MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
}
