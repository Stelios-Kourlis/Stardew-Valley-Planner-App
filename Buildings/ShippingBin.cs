using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {
    public override string TooltipMessage => "";

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 1;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new(250, Materials.Coins),
            new(150, Materials.Wood)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}