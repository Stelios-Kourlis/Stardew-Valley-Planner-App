using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Well : Building {
    protected override void Init() {
        name = GetType().Name;
        baseHeight = 3;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 1_000},
            {Materials.Stone, 75},
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }
}