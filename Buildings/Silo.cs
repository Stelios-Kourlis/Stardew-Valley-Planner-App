using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Silo : Building {
    public new void Start(){
        Init();
        base.Start();
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 100},
            {Materials.Stone, 100},
            {Materials.Clay, 10},
            {Materials.CopperBar, 5}
        };
    }
}
