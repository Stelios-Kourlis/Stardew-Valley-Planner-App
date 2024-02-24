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

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int> {
            {Materials.Coins, 10_000_000}
        };
    }
}