using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class GoldClock : Building {

    public override void OnAwake(){
        buildingName = "Gold Clock";
        BaseHeight = 2;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo> {
            new(10_000_000, Materials.Coins)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}