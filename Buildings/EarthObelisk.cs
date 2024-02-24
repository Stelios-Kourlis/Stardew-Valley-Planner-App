using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EarthObelisk : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 3;
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>{
                {Materials.Coins, 500_000},
            {Materials.IridiumBar, 10},
            {Materials.EarthCrystal, 10}
            };

    }
        
}
