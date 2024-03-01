using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class GoldClock : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo> {
            new MaterialInfo(10_000_000, Materials.Coins)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}