using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EarthObelisk : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 500_000},
            {Materials.IridiumBar, 10},
            {Materials.EarthCrystal, 10}
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }
        
}
