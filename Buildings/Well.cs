using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Well : Building {
    public override string TooltipMessage => "";

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 3;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
    return new List<MaterialInfo>(){
        new(1000, Materials.Coins),
        new(75, Materials.Stone),
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}