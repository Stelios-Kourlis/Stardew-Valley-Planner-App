using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Silo : Building {
    public override string TooltipMessage => "";

    public override void OnAwake(){
        BaseHeight = 3;
        base.OnAwake();
    }
    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new(100, Materials.Coins),
            new(100, Materials.Stone),
            new(10, Materials.Clay),
            new(5, Materials.CopperBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}
