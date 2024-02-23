using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IslandObelisk : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
    }

    public new void Start(){
        Init();
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 1_000_000},
            {Materials.IridiumBar, 10},
            {Materials.DragonTooth, 10},
            {Materials.Banana, 10}
        };
    }
}