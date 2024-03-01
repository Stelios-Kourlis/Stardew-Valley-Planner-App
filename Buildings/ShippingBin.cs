using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 1;
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(250, Materials.Coins),
            new MaterialInfo(150, Materials.Wood)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}