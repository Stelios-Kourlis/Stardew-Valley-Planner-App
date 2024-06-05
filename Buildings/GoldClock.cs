using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class GoldClock : Building {

    public override void OnAwake() {
        BuildingName = "Gold Clock";
        BaseHeight = 2;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return new List<MaterialInfo> {
            new(10_000_000, Materials.Coins)
        };
    }
}