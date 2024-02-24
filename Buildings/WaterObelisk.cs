using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterObelisk : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 3;
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 500_000},
            {Materials.IridiumBar, 5},
            {Materials.Clam, 10},
            {Materials.Coral, 10}
        };
    }
}