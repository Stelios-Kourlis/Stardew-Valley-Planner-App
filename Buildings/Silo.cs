using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Silo : Building {
    public override string TooltipMessage => "";

    public override void OnAwake(){
        baseHeight = 4;
        base.OnAwake();
    }
    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(100, Materials.Coins),
            new MaterialInfo(100, Materials.Stone),
            new MaterialInfo(10, Materials.Clay),
            new MaterialInfo(5, Materials.CopperBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}
