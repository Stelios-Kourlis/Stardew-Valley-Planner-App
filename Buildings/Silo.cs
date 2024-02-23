using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Silo : Building {
    public new void Start(){
        Init();
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 100},
            {Materials.Stone, 100},
            {Materials.Clay, 10},
            {Materials.CopperBar, 5}
        };
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
    }
}
