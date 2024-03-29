using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building {
    public override string TooltipMessage => "";

    public override void OnAwake(){
        name = GetType().Name;
        baseHeight = 2;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PAINT
        };
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(10000, Materials.Coins),
            new MaterialInfo(100, Materials.Hardwood),
            new MaterialInfo(5, Materials.IronBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}
