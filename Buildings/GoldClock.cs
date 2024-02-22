using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;

public class GoldClock : Building {
    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 10_000_000}
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }
}