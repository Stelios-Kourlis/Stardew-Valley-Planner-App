using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {

    protected override void Init() {
        name = GetType().Name;
        baseHeight = 1;
        materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 250},
            {Materials.Wood, 150}
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }
}