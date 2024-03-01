using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Well : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 3;
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
    return new List<MaterialInfo>(){
        new MaterialInfo(1000, Materials.Coins),
        new MaterialInfo(75, Materials.Stone),
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}