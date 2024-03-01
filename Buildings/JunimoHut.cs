using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JunimoHut : Building {
    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
    return new List<MaterialInfo>(){
        new MaterialInfo(20000, Materials.Coins),
        new MaterialInfo(200, Materials.Stone),
        new MaterialInfo(9, Materials.Starfruit),
        new MaterialInfo(100, Materials.Fiber)
    };
}

    protected override void PlacePreview(){//todo add range
        base.PlacePreview();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}