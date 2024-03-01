using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PAINT
        };
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(10000, Materials.Coins),
            new MaterialInfo(100, Materials.Hardwood),
            new MaterialInfo(5, Materials.IronBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}
