using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DesertObelisk : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
    }

    public new void Start(){
        Init();
        base.Start();
    } 

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>{
                {Materials.Coins, 1_000_000},
                {Materials.IridiumBar, 20},
                {Materials.Coconut, 10},
                {Materials.CactusFruit, 10}
            };

    }
}
