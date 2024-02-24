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

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 1_000},
            {Materials.Stone, 75},
        };
    }
}